/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2017 ITAdapter Corp. Inc.
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
using NFX.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFX;
using NFX.Web;
using NFX.Web.Social;

namespace NFX.UTest.Web
{
  [Runnable]
  public class WebSettingsTests
  {
    [Run]
    public void SocialInstantiation()
    {
      var conf = @"nfx { web-settings { social {
        provider {type='NFX.Web.Social.GooglePlus, NFX.Web' client-code='111111111111' client-secret='a1111111111-a11111111111' web-service-call-timeout-ms='20000' keep-alive='false' pipelined='false'}
        provider {type='NFX.Web.Social.Facebook, NFX.Web' client-code='1111111111111111' client-secret='a1111111111111111111111111111111' app-accesstoken='a|111111111111111111111111111111111111111111'}
        provider {type='NFX.Web.Social.Twitter, NFX.Web' client-code='a111111111111111111111' client-secret='a11111111111111111111111111111111111111111'}
        provider {type='NFX.Web.Social.VKontakte, NFX.Web' client-code='1111111' client-secret='a1111111111111111111'}
        provider {type='NFX.Web.Social.LinkedIn, NFX.Web' api-key='a1111111111111' secret-key='a111111111111111'}
} } }".AsLaconicConfig();

      using (new NFX.ApplicationModel.ServiceBaseApplication(new string[] { }, conf))
      {
        var social = WebSettings.SocialNetworks;

        Aver.AreEqual(5, social.Count);
        Aver.AreObjectsEqual(((SocialNetwork)social["GooglePlus"]).GetType(), typeof(GooglePlus));
        Aver.AreObjectsEqual(((SocialNetwork)social["Facebook"]).GetType(), typeof(Facebook));
        Aver.AreObjectsEqual(((SocialNetwork)social["Twitter"]).GetType(), typeof(Twitter));
        Aver.AreObjectsEqual(((SocialNetwork)social["VKontakte"]).GetType(), typeof(VKontakte));
        Aver.AreObjectsEqual(((SocialNetwork)social["LinkedIn"]).GetType(), typeof(LinkedIn));
      }
    }

    [Run]
    public void SocialProviderProperties()
    {
      var conf = @"nfx { web-settings { social {
        provider {type='NFX.Web.Social.GooglePlus, NFX.Web' client-code='111111111111' client-secret='a1111111111-a11111111111' web-service-call-timeout-ms='20000' keep-alive='false' pipelined='false'}
        provider {type='NFX.Web.Social.Facebook, NFX.Web' client-code='1111111111111111' client-secret='a1111111111111111111111111111111' app-accesstoken='a|111111111111111111111111111111111111111111'}
} } }".AsLaconicConfig();

      using (new NFX.ApplicationModel.ServiceBaseApplication(new string[] { }, conf))
      {
        var social = WebSettings.SocialNetworks;

        Aver.AreEqual(2, social.Count);

        var instantiatedGooglePlus = ((GooglePlus)social["GooglePlus"]);
        Aver.AreEqual(20000, instantiatedGooglePlus.WebServiceCallTimeoutMs);
        Aver.IsFalse(instantiatedGooglePlus.KeepAlive);
        Aver.IsFalse(instantiatedGooglePlus.Pipelined);

        var instantiatedFacebook = ((Facebook)social["Facebook"]);
        Aver.AreEqual(SocialNetwork.DEFAULT_TIMEOUT_MS_DEFAULT, instantiatedFacebook.WebServiceCallTimeoutMs);
        Aver.IsTrue(instantiatedFacebook.KeepAlive);
        Aver.IsTrue(instantiatedFacebook.Pipelined);
      }
    }

    [Run]
    public void ServicePointManagerTest()
    {
      var conf = @"
      nfx
      {
        web-settings
        {
          service-point-manager
          {

            check-certificate-revocation-list=true //false
            default-connection-limit=4 //2
            dns-refresh-timeout=100000 //120000
            enable-dns-round-robin=true //False
            expect-100-continue=false //true
            max-service-point-idle-time=90000 //100000
            max-service-points=10 //0
            security-protocol=Tls12 //Ssl3, Tls
            use-nagle-algorithm=true

            service-point
            {
              uri='https://footest.com'

              connection-lease-timeout=2300 // -1
              ConnectionLimit=7 //4
              expect-100-continue=false //True
              max-idle-time=103000 //90000
              receive-buffer-size=127000 //-1
              use-nagle-algorithm=false //True
            }

            service-point
            {
              uri='https://footest_a.com'
            }

            policy
            {
              default-certificate-validation
              {
                case { uri='https://footest.com' trusted=true}
                case { uri='https://footest_a.com'}
              }
            }
          }
        }
      }".AsLaconicConfig();

      using (new NFX.ApplicationModel.ServiceBaseApplication(new string[] {}, conf))
      {
        var spmc = WebSettings.ServicePointManager;

        Aver.IsTrue(System.Net.ServicePointManager.CheckCertificateRevocationList);
        Aver.AreEqual( 4, System.Net.ServicePointManager.DefaultConnectionLimit);
        Aver.AreEqual( 100000, System.Net.ServicePointManager.DnsRefreshTimeout);
        Aver.IsTrue(System.Net.ServicePointManager.EnableDnsRoundRobin);
        Aver.IsFalse(System.Net.ServicePointManager.Expect100Continue);
        Aver.AreEqual( 90000, System.Net.ServicePointManager.MaxServicePointIdleTime);
        Aver.AreEqual( 10, System.Net.ServicePointManager.MaxServicePoints);
        Aver.IsTrue( System.Net.SecurityProtocolType.Tls12 == System.Net.ServicePointManager.SecurityProtocol);
        Aver.IsTrue(System.Net.ServicePointManager.UseNagleAlgorithm);

        var sp_footest_com = System.Net.ServicePointManager.FindServicePoint(new Uri("https://footest.com"));
        Aver.IsFalse(sp_footest_com.Expect100Continue);
        var sp_footest_1 = System.Net.ServicePointManager.FindServicePoint(new Uri("https://footest_1.com"));
        Aver.IsFalse(sp_footest_1.Expect100Continue);
        var sp_footest_a = System.Net.ServicePointManager.FindServicePoint(new Uri("https://footest_a.com"));
        Aver.AreEqual(System.Net.ServicePointManager.Expect100Continue, sp_footest_a.Expect100Continue);

        Aver.AreEqual(2, spmc.ServicePoints.Count());
        var spc_footest_com = spmc.ServicePoints.First();

        Aver.AreEqual( 2300, spc_footest_com.ConnectionLeaseTimeout);
        Aver.AreEqual( 7, spc_footest_com.ConnectionLimit);
        Aver.IsFalse( spc_footest_com.Expect100Continue);
        Aver.AreEqual( 103000, spc_footest_com.MaxIdleTime);
        Aver.AreEqual( 127000, spc_footest_com.ReceiveBufferSize);
        Aver.IsFalse( spc_footest_com.UseNagleAlgorithm);
      }
    }
  }
}
