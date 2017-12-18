/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2018 Agnicore Inc. portions ITAdapter Corp. Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
</FILE_LICENSE>*/
using System;
using System.Collections.Generic;

namespace NFX.ApplicationModel
{
    /// <summary>
    /// Provides access to execution context - that groups Application, Request, Response and Session objects.
    /// All objects may be either application-global or thread-level.
    /// Effectively ExecutionContext.Application is the central DI/servi e locator facility per process.
    /// The async code should flow the context by passing it to functors.
    /// </summary>
    /// <remarks>
    /// This pattern is used on purpose based on careful evaluation of various DI frameworks use-cases in various projects,
    /// both server and client-side. The central service/locator hub per process as faciliatetd by the IApplication is the most intuitive ans simple
    /// dependency resolution facility for 90+% of various business applications.
    /// </remarks>
    public static class ExecutionContext
    {
        [ThreadStatic]
        private static object ts_Request;

        [ThreadStatic]
        private static object ts_Response;

        [ThreadStatic]
        private static ISession ts_Session;

        private static Stack<IApplication> s_AppStack = new Stack<IApplication>();
        private static volatile IApplication s_Application;
        private static volatile object s_Request;
        private static volatile object s_Response;
        private static volatile ISession s_Session;


        /// <summary>
        /// Returns global application context
        /// </summary>
        public static IApplication Application
        {
          get { return s_Application ?? NOPApplication.Instance; }
        }

        /// <summary>
        /// Returns Request object for current thread, or if it is null, app-global-level object is returned
        /// </summary>
        public static object Request
        {
          get { return ts_Request ?? s_Request; }
        }

        /// <summary>
        /// Returns Response object for current thread, or if it is null, app-global-level object is returned
        /// </summary>
        public static object Response
        {
          get { return ts_Response ?? s_Response; }
        }

        /// <summary>
        /// Returns Session object for current thread, or if it is null, app-global-level object is returned
        /// </summary>
        public static ISession Session
        {
          get { return ts_Session ?? s_Session ?? NOPSession.Instance; }
        }

        /// <summary>
        /// Returns true when thread-level session object is available and not a NOPSession instance
        /// </summary>
        public static bool HasThreadContextSession
        {
          get { return ts_Session != null && ts_Session.GetType()!=typeof(NOPSession); }
        }

        /// <summary>
        /// Framework internal app bootstrapping method.
        /// Sets root application context
        /// </summary>
        public static void __BindApplication(IApplication application)
        {
          if (application==null || application is NOPApplication)
            throw new NFXException(StringConsts.ARGUMENT_ERROR+"__BindApplication(null|NOPApplication)");

          lock(s_AppStack)
          {
            if (s_AppStack.Contains(application))
              throw new NFXException(StringConsts.ARGUMENT_ERROR+"__BindApplication(duplicate)");

            if (s_Application!=null && !s_Application.AllowNesting)
              throw new NFXException(StringConsts.APP_CONTAINER_NESTING_ERROR.Args(application.GetType().FullName, s_Application.GetType().FullName));

            s_AppStack.Push( s_Application );
            s_Application = application;
          }
        }

        /// <summary>
        /// Framework internal app bootstrapping method.
        /// Resets root application context
        /// </summary>
        public static void __UnbindApplication(IApplication application)
        {
          lock(s_AppStack)
          {
            if (s_Application!=application)  return;
            if (s_AppStack.Count==0) return;

            s_Application = s_AppStack.Pop();
          }
        }

        /// <summary>
        /// Internal framework-only method to bind application-level context
        /// </summary>
        public static void __SetApplicationLevelContext(object request, object response, ISession session)
        {
          s_Request = request;
          s_Response = response;
          s_Session = session;
        }

        /// <summary>
        /// Internal framework-only method to bind thread-level context
        /// </summary>
        public static void __SetThreadLevelContext(object request, object response, ISession session)
        {
          ts_Request = request;
          ts_Response = response;
          ts_Session = session;
        }

        /// <summary>
        /// Internal framework-only method to bind thread-level context
        /// </summary>
        public static void __SetThreadLevelSessionContext(ISession session)
        {
          ts_Session = session;
        }

    }

}
