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
using NFX.IO.Net.Gate;
using NFX.Log;

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

      public const int ACCEPT_THREAD_GRANULARITY_MS = 250;

      public const ushort DEFAULT_DRAIN_ENTITY_BODY_TIMEOUT_SEC = 120;
      public const ushort DEFAULT_ENTITY_BODY_TIMEOUT_SEC = 120;
      public const ushort DEFAULT_HEADER_WAIT_TIMEOUT_SEC = 120;
      public const ushort DEFAULT_IDLE_CONNECTION_TIMEOUT_SEC = 120;
      public const ushort DEFAULT_REQUEST_QUEUE_TIMEOUT_SEC = 120;
      public const uint   DEFAULT_MIN_SEND_BYTES_PER_SECOND = 150;

    #endregion

    #region .ctor
      public HttpWebListener(WaveServer waveServer) : base(waveServer)
      {
      }
    #endregion

    #region fields
      private int m_KernelHttpQueueLimit = DEFAULT_KERNEL_HTTP_QUEUE_LIMIT;
      private int m_ParallelAccepts = DEFAULT_PARALLEL_ACCEPTS;

      private ushort m_DrainEntityBodyTimeoutSec = DEFAULT_DRAIN_ENTITY_BODY_TIMEOUT_SEC;
      private ushort m_EntityBodyTimeoutSec      = DEFAULT_ENTITY_BODY_TIMEOUT_SEC;
      private ushort m_HeaderWaitTimeoutSec      = DEFAULT_HEADER_WAIT_TIMEOUT_SEC;
      private ushort m_IdleConnectionTimeoutSec  = DEFAULT_IDLE_CONNECTION_TIMEOUT_SEC;
      private ushort m_RequestQueueTimeoutSec    = DEFAULT_REQUEST_QUEUE_TIMEOUT_SEC;
      private uint   m_MinSendBytesPerSecond     = DEFAULT_MIN_SEND_BYTES_PER_SECOND;


      private SYSHL m_Listener;
      private bool m_IgnoreClientWriteErrors = true;
      //private bool m_LogHandleExceptionErrors;

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


      [Config(Default=DEFAULT_DRAIN_ENTITY_BODY_TIMEOUT_SEC)]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB)]
      public ushort DrainEntityBodyTimeoutSec
      {
        get { return m_DrainEntityBodyTimeoutSec; }
        set
        {
          m_DrainEntityBodyTimeoutSec = value;
          if (m_Listener != null && m_Listener.IsListening && !OS.Computer.IsMono)
            m_Listener.TimeoutManager.DrainEntityBody = TimeSpan.FromSeconds(m_DrainEntityBodyTimeoutSec);
        }
      }
      [Config(Default=DEFAULT_ENTITY_BODY_TIMEOUT_SEC)]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB)]
      public ushort EntityBodyTimeoutSec
      {
        get { return m_EntityBodyTimeoutSec; }
        set
        {
          m_EntityBodyTimeoutSec = value;
          if (m_Listener != null && m_Listener.IsListening && !OS.Computer.IsMono)
            m_Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(m_EntityBodyTimeoutSec);
        }
      }
      [Config(Default=DEFAULT_HEADER_WAIT_TIMEOUT_SEC)]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB)]
      public ushort HeaderWaitTimeoutSec
      {
        get { return m_HeaderWaitTimeoutSec; }
        set
        {
          m_HeaderWaitTimeoutSec = value;
          if (m_Listener != null && m_Listener.IsListening && !OS.Computer.IsMono)
            m_Listener.TimeoutManager.HeaderWait = TimeSpan.FromSeconds(m_HeaderWaitTimeoutSec);
        }
      }
      [Config(Default=DEFAULT_IDLE_CONNECTION_TIMEOUT_SEC)]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB)]
      public ushort IdleConnectionTimeoutSec
      {
        get { return m_IdleConnectionTimeoutSec; }
        set
        {
          m_IdleConnectionTimeoutSec = value;
          if (m_Listener != null && m_Listener.IsListening && !OS.Computer.IsMono)
            m_Listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(m_IdleConnectionTimeoutSec);
        }
      }
      [Config(Default=DEFAULT_REQUEST_QUEUE_TIMEOUT_SEC)]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB)]
      public ushort RequestQueueTimeoutSec
      {
        get { return m_RequestQueueTimeoutSec; }
        set
        {
          m_RequestQueueTimeoutSec = value;
          if (m_Listener != null && m_Listener.IsListening && !OS.Computer.IsMono)
            m_Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(m_RequestQueueTimeoutSec);
        }
      }
      [Config(Default=DEFAULT_MIN_SEND_BYTES_PER_SECOND)]
      [ExternalParameter(CoreConsts.EXT_PARAM_GROUP_WEB)]
      public uint MinSendBytesPerSecond
      {
        get { return m_MinSendBytesPerSecond; }
        set
        {
          m_MinSendBytesPerSecond = value;
          if (m_Listener != null && m_Listener.IsListening && !OS.Computer.IsMono)
            m_Listener.TimeoutManager.MinSendBytesPerSecond = m_MinSendBytesPerSecond;
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
        var accessor = ComponentDirector.___InternalAccessor;
        var semaphores = new Semaphore[]{m_AcceptSemaphore, accessor.WorkSemaphore};

        while(Running)
        {
          //Both semaphores get acquired here
          if (!WaitHandle.WaitAll(semaphores, ACCEPT_THREAD_GRANULARITY_MS)) continue;

          if (!m_Listener.IsListening) continue;

          m_Listener.BeginGetContext(callback, null);//the BeginGetContext/EndGetContext is called on a different thread (pool IO background)
                                                     // whereas GetContext() is called on the caller thread
        }
      }


      private void callback(IAsyncResult result)
      {
        SYSHL listener = m_Listener;//capture local copy
        if (listener==null) return;//callback sometime happens when listener is null on shutdown
        if (!listener.IsListening) return;

        var accessor = ComponentDirector.___InternalAccessor;
        var gate = ComponentDirector.Gate;

        //This is called on its own pool thread by HttpListener
        bool gateAccessDenied = false;
        HttpListenerContext listenerContext;
        try
        {
          listenerContext = m_Listener.EndGetContext(result);

          accessor.Stat_IncServerRequest();

          if (gate!=null)
          {
            try
            {
              var action = gate.CheckTraffic(new HTTPIncomingTraffic(listenerContext.Request, ComponentDirector.GateCallerRealIpAddressHeader));
              if (action!=GateAction.Allow)
              {
                //access denied
                gateAccessDenied = true;
                listenerContext.Response.StatusCode = Web.WebConsts.STATUS_403;         //todo - need properties for this
                listenerContext.Response.StatusDescription = Web.WebConsts.STATUS_403_DESCRIPTION;
                listenerContext.Response.Close();

                accessor.Stat_IncServerGateDenial();
                return;
              }
            }
            catch(Exception denyError)
            {
              ComponentDirector.Log(MessageType.Error, denyError.ToMessageWithType(), "callback(deny request)", denyError);
            }
          }
        }
        catch(Exception error)
        {
          if (error is HttpListenerException)
            if ((error as HttpListenerException).ErrorCode==995) return;//Aborted

          ComponentDirector.Log(MessageType.Error, error.ToMessageWithType(), "callback(endGetContext())", error);
          return;
        }
        finally
        {
          if (Running)
          {
              var acceptCount = m_AcceptSemaphore.Release();

              accessor.Stat_ServerAcceptSemaphoreCount(acceptCount);

              if (gateAccessDenied)//if access was denied then no work will be done either
              {
                var workCount = accessor.WorkSemaphore.Release();
                accessor.Stat_ServerWorkSemaphoreCount(workCount);
              }
          }
        }

        //this whole method is on its own thread already for HttpListener
        if (!Running) return;

        try
        {
          IHttpContext ctx = new HttpContext(listenerContext);
          accessor.AcceptRequest(ctx, onThisThread: true);
        }
        catch(Exception error)
        {
          ComponentDirector.Log(MessageType.Error, error.ToMessageWithType(), "callback(endGetContext(AcceptRequest()))", error);
        }
      }
    #endregion
  }
}
