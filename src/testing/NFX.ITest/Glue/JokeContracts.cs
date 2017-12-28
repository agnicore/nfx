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
using System.Data;
using System.Linq;
using System.Text;

using NFX.ApplicationModel;
using NFX.Glue;
using NFX.Glue.Native;
using NFX.Glue.Protocol;
using NFX.Scripting;
using static NFX.Aver.ThrowsAttribute;

using BusinessLogic;

namespace NFX.ITest.Glue
{
    [Runnable]
    public class JokeContracts: JokeTestBase
    {
        [Run]
        [Aver.Throws(typeof(ClientCallException))]//because there is no binding registered
        public void ExpectedExceptionWhenGlueNotConfiguredAndBindingNotRegistered()
        {
            var cl = new JokeContractClient(TestServerSyncNode);
        }

        [Run]
        public void GlueConfiguredByCode()
        {
            //This is an example of how to use Glue without pre-configured app container
            var glue = new NFX.Glue.Implementation.GlueService();
            glue.Start();
            try
            {
                var binding = new SyncBinding(glue, "sync");
                var cl = new JokeContractClient(glue, TestServerSyncNode);
            }
            finally
            {
                glue.WaitForCompleteStop();
            }
        }

        [Run]
        public void GlueConfiguredByCode_MPX()
        {
            //This is an example of how to use Glue without pre-configured app container
            var glue = new NFX.Glue.Implementation.GlueService();
            glue.Start();
            try
            {
                var binding = new MpxBinding(glue, "mpx");
                var cl = new JokeContractClient(glue, TestServerMpxNode);


            }
            finally
            {
                glue.WaitForCompleteStop();
            }
        }


        [Run]
        public void GlueConfiguredByCodeAndMakeCall_Sync()
        {
            //This is an example of how to use Glue without pre-configured app container
            var app = new TestApplication(){ Active = true };
            var glue = new NFX.Glue.Implementation.GlueService(app);
            glue.Start();
            try
            {
                using(var binding = new SyncBinding(glue, "sync"))
                {
                  binding.Start();
                  var cl = new JokeContractClient(glue, TestServerSyncNode);
                  cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

                  var result = cl.Echo("Gello A!");

                  Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
                }
            }
            finally
            {
                glue.WaitForCompleteStop();
            }
        }


        [Run]
        public void GlueConfiguredByCodeAndMakeCall_MPX()
        {
            //This is an example of how to use Glue without pre-configured app container
            var app = new TestApplication(){ Active = true };
            var glue = new NFX.Glue.Implementation.GlueService();
            glue.Start();
            try
            {
                using(var binding = new MpxBinding(glue, "mpx"))
                {
                  binding.Start();
                  var cl = new JokeContractClient(glue, TestServerMpxNode);
                  cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

                  var result = cl.Echo("Gello A!");

                  Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
                }
            }
            finally
            {
                glue.WaitForCompleteStop();
            }
        }



        [Run]
        public void Sync_JokeContract_Echo_ByCode()
        {
            using( JokeHelper.MakeApp())
            {
              var cl = new JokeContractClient(TestServerSyncNode);
                cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

                var result = cl.Echo("Gello A!");

                Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
            }
        }

        [Run]
        public void MPX_JokeContract_Echo_ByCode()
        {
            using( JokeHelper.MakeApp())
            {
              var cl = new JokeContractClient(TestServerMpxNode);
                cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

                var result = cl.Echo("Gello A!");

                Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
            }
        }

        [Run]
        public void Sync_JokeContract_Async_Echo_ByCode()
        {
          using (JokeHelper.MakeApp())
            {
              var cl = new JokeContractClient(TestServerSyncNode);
              cl.Headers.Add(new AuthenticationHeader(TestCredentials));

                var call = cl.Async_Echo("Gello B!");

                var result = call.GetValue<string>();

                Aver.IsTrue(result.StartsWith("Server echoed Gello B!"));
            }
        }

        [Run]
        public void MPX_JokeContract_Async_Echo_ByCode()
        {
          using (JokeHelper.MakeApp())
            {
              var cl = new JokeContractClient(TestServerMpxNode);
              cl.Headers.Add(new AuthenticationHeader(TestCredentials));

                var call = cl.Async_Echo("Gello B!");

                var result = call.GetValue<string>();

                Aver.IsTrue(result.StartsWith("Server echoed Gello B!"));
            }
        }

        [Run]
        [Aver.Throws(typeof(RemoteException), Message="NFX.Security.AuthorizationException", MsgMatch = MatchType.Contains)]
        public void Sync_JokeContract_Expected_Security_Exception()
        {
          using (JokeHelper.MakeApp())
            {
                var cl = new JokeContractClient(TestServerSyncNode);

                var result = cl.Echo("Blah");//throws sec exception
            }
        }

        [Run]
        [Aver.Throws(typeof(RemoteException), Message="NFX.Security.AuthorizationException", MsgMatch = MatchType.Contains)]
        public void MPX_JokeContract_Expected_Security_Exception()
        {
          using (JokeHelper.MakeApp())
            {
                var cl = new JokeContractClient(TestServerMpxNode);

                var result = cl.Echo("Blah");//throws sec exception
            }
        }


    }
}
