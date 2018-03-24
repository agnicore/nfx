using System;
using System.Collections.Generic;
using System.Text;
using SYSHL = System.Net.HttpListener;

using NFX;
using NFX.Environment;
using NFX.Wave;
using NFX.Wave.Server;
using System.Threading;

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

  }
}
