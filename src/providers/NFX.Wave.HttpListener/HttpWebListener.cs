using System;
using System.Collections.Generic;
using System.Text;
using SYSHL = System.Net.HttpListener;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using NFX;
using NFX.Environment;
using NFX.Wave;
using NFX.Wave.Server;

namespace NFX.Wave.HttpListener
{
  /// <summary>
  /// Provides implementation of WebLsitener based on a classic System.Net.HttpListener
  /// </summary>
  public sealed class HttpWebListener : WebListener
  {
    #region CONSTS

      public const int DEFAULT_KERNEL_HTTP_QUEUE_LIMIT = 1000;
      public const int MIN_KERNEL_HTTP_QUEUE_LIMIT = 16;
      public const int MAX_KERNEL_HTTP_QUEUE_LIMIT = 512 * 1024;

      public const int DEFAULT_PARALLEL_ACCEPTS = 64;
      public const int MIN_PARALLEL_ACCEPTS = 1;
      public const int MAX_PARALLEL_ACCEPTS = 1024;

      public const int DEFAULT_PARALLEL_WORKS = 256;
      public const int MIN_PARALLEL_WORKS = 1;
      public const int MAX_PARALLEL_WORKS = 1024*1024;

      public const int ACCEPT_THREAD_GRANULARITY_MS = 250;

    #endregion

    #region .ctor
      public HttpWebListener(WaveServer waveServer) : base(waveServer)
      {
      }
    #endregion

    #region fields
      private int m_KernelHttpQueueLimit = DEFAULT_KERNEL_HTTP_QUEUE_LIMIT;
      private int m_ParallelAccepts = DEFAULT_PARALLEL_ACCEPTS;
      private int m_ParallelWorks = DEFAULT_PARALLEL_WORKS;


      private SYSHL m_Listener;
      private bool m_IgnoreClientWriteErrors = true;
      private bool m_LogHandleExceptionErrors;

      private Thread m_AcceptThread;
      private Semaphore m_AcceptSemaphore;
    #endregion

    #region Properties
      /// <summary>
      /// When true does not throw exceptions on client channel write
      /// </summary>
      [Config(Default=true)]
      public bool IgnoreClientWriteErrors
      {
        get { return m_IgnoreClientWriteErrors;}
        set
        {
          CheckServiceInactive();
          m_IgnoreClientWriteErrors = value;
        }
      }

      /// <summary>
      /// When true writes errors that get thrown in server catch-all HandleException methods
      /// </summary>
      [Config]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB, CoreConsts.EXT_PARAM_GROUP_INSTRUMENTATION)]
      public bool LogHandleExceptionErrors
      {
        get { return m_LogHandleExceptionErrors;}
        set { m_LogHandleExceptionErrors = value;}
      }

      /// <summary>
      /// Establishes HTTP.sys kernel queue limit
      /// </summary>
      [Config]
      public int KernelHttpQueueLimit
      {
        get { return m_KernelHttpQueueLimit;}
        set
        {
          CheckServiceInactive();
          if (value < MIN_KERNEL_HTTP_QUEUE_LIMIT) value = MIN_KERNEL_HTTP_QUEUE_LIMIT;
           else
            if (value > MAX_KERNEL_HTTP_QUEUE_LIMIT) value = MAX_KERNEL_HTTP_QUEUE_LIMIT;
          m_KernelHttpQueueLimit = value;
        }
      }

      /// <summary>
      /// Specifies how many requests can get accepted from kernel queue in parallel
      /// </summary>
      [Config(Default=DEFAULT_PARALLEL_ACCEPTS)]
      public int ParallelAccepts
      {
        get { return m_ParallelAccepts;}
        set
        {
          CheckServiceInactive();
          if (value < MIN_PARALLEL_ACCEPTS) value = MIN_PARALLEL_ACCEPTS;
           else
            if (value > MAX_PARALLEL_ACCEPTS) value = MAX_PARALLEL_ACCEPTS;
          m_ParallelAccepts = value;
        }
      }
    #endregion

    #region Protected
      protected override void DoStart()
      {
         m_Listener = new SYSHL();

         foreach(var prefix in m_Prefixes)
               m_Listener.Prefixes.Add(prefix);

         m_Listener.Start();

         m_Listener.IgnoreWriteExceptions = m_IgnoreClientWriteErrors;
      }

      protected override void DoSignalStop()
      {
        base.DoSignalStop();
      }

      protected override void DoWaitForCompleteStop()
      {
        base.DoWaitForCompleteStop();
      }
    #endregion

    #region .pvt
     private void acceptThreadSpin()
     {
        var semaphores = new Semaphore[]{m_AcceptSemaphore, m_WorkSemaphore};
        while(Running)
        {
          //Both semaphores get acquired here
          if (!WaitHandle.WaitAll(semaphores, ACCEPT_THREAD_GRANULARITY_MS)) continue;

          if (m_Listener.IsListening)
               m_Listener.BeginGetContext(callback, null);//the BeginGetContext/EndGetContext is called on a different thread (pool IO background)
                                                          // whereas GetContext() is called on the caller thread
        }
     }


     private void callback(IAsyncResult result)
     {
       if (m_Listener==null) return;//callback sometime happens when listener is null on shutdown
       if (!m_Listener.IsListening) return;

       //This is called on its own pool thread by HttpListener
       bool gateAccessDenied = false;
       HttpListenerContext listenerContext;
       try
       {
         listenerContext = m_Listener.EndGetContext(result);

         if (m_InstrumentationEnabled) Interlocked.Increment(ref m_stat_ServerRequest);


         if (m_Gate!=null)
            try
            {
              var action = m_Gate.CheckTraffic(new HTTPIncomingTraffic(listenerContext.Request, GateCallerRealIpAddressHeader));
              if (action!=GateAction.Allow)
              {
                //access denied
                gateAccessDenied = true;
                listenerContext.Response.StatusCode = Web.WebConsts.STATUS_403;         //todo - need properties for this
                listenerContext.Response.StatusDescription = Web.WebConsts.STATUS_403_DESCRIPTION;
                listenerContext.Response.Close();

                if (m_InstrumentationEnabled) Interlocked.Increment(ref m_stat_ServerGateDenial);
                return;
              }
            }
            catch(Exception denyError)
            {
              Log(MessageType.Error, denyError.ToMessageWithType(), "callback(deny request)", denyError);
            }
       }
       catch(Exception error)
       {
          if (error is HttpListenerException)
           if ((error as HttpListenerException).ErrorCode==995) return;//Aborted

          Log(MessageType.Error, error.ToMessageWithType(), "callback(endGetContext())", error);
          return;
       }
       finally
       {
          if (Running)
          {
             var acceptCount = m_AcceptSemaphore.Release();

             if (m_InstrumentationEnabled)
              Thread.VolatileWrite(ref m_stat_ServerAcceptSemaphoreCount, acceptCount);

             if (gateAccessDenied)//if access was denied then no work will be done either
             {
                var workCount = m_WorkSemaphore.Release();
                if (m_InstrumentationEnabled)
                  Thread.VolatileWrite(ref m_stat_ServerWorkSemaphoreCount, workCount);
             }
          }
       }

       //no need to call process() asynchronously because this whole method is on its own thread already
       if (Running)
       {
         // var workContext = MakeContext(listenerContext);
         // m_Dispatcher.Dispatch(workContext);

         IHttpContext ctx = new HttpListenerWaveHttpContext(listenerContext);
         var task = Task.FromResult(ctx);
         this.ComponentDirector.ProcessRequest(task);
       }
     }


    #endregion

  }
}
