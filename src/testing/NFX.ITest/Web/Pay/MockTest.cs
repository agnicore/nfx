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
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using NFX.ApplicationModel;
using NFX.Environment;
using NFX.Scripting;
using NFX.Web;
using NFX.Web.Pay;
using NFX.Web.Pay.Mock;
using static NFX.Aver.ThrowsAttribute;

namespace NFX.ITest.Web.Pay
{
  [Runnable]
  public class MockTest: NFX.ITest.ExternalCfg
  {
    #region Tests

      //[Run]
      //public void MakePaySystem()
      //{
      //  var paymentSection = Configuration.ProviderLoadFromString(LACONF, Configuration.CONFIG_LACONIC_FORMAT)
      //    .Root.Navigate("/web-settings/payment-processing") as ConfigSectionNode;

      //  var mockSection = paymentSection.Children.First(p => p.AttrByName("name").Value == "Mock");

      //  Console.WriteLine(mockSection);

      //  var ps = PaySystem.Make<MockSystem>(null, mockSection);

      //  Console.WriteLine(ps);
      //}

      [Run]
      public void Charge()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeCommon(sess);
          }
        }
      }

      [Run]
      [Aver.Throws(typeof(PaymentMockException), Message = "declined", MsgMatch = MatchType.Contains)]
      public void ChargeCardDeclined()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeCardDeclined(sess);
          }
        }
      }

      [Run]
      [Aver.Throws(typeof(PaymentMockException), Message = "is incorrect", MsgMatch = MatchType.Contains)]
      public void ChargeCardLuhnErr()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeCardLuhnErr(sess);
          }
        }
      }

      [Run]
      [Aver.Throws(typeof(PaymentMockException), Message = "Invalid card expiration", MsgMatch = MatchType.Contains)]
      public void ChargeCardExpYearErr()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeCardExpYearErr(sess);
          }
        }
      }

      [Run]
      [Aver.Throws(typeof(PaymentMockException), Message = "Invalid card expiration", MsgMatch = MatchType.Contains)]
      public void ChargeCardExpMonthErr()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeCardExpMonthErr(sess);
          }
        }
      }

      [Run]
      [Aver.Throws(typeof(PaymentMockException), Message = "Invalid card CVC", MsgMatch = MatchType.Contains)]
      public void ChargeCardVCErr()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeCardVCErr(sess);
          }
        }
      }

      [Run]
      public void ChargeWithBillingAddressInfo()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = getPaySystem();

          using (var sess = ps.StartSession())
          {
            PayTestCommon.ChargeWithBillingAddressInfo(sess);
          }
        }
      }

      [Run]
      public void CaptureImplicitTotal()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.CaptureImplicitTotal(pss);
          }
        }
      }

      [Run]
      public void CaptureExplicitTotal()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.CaptureExplicitTotal(pss);
          }
        }
      }

      [Run]
      public void CapturePartial()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.CapturePartial(pss);
          }
        }
      }

      [Run]
      public void RefundFullImplicit()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.RefundFullImplicit(pss);
          }
        }
      }

      [Run]
      public void RefundFullExplicit()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.RefundFullExplicit(pss);
          }
        }
      }

      [Run]
      public void RefundFullTwoParts()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.RefundFullTwoParts(pss);
          }
        }
      }

      [Run]
      public void TransferToBank()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.TransferToBank(pss);
          }
        }
      }

      [Run]
      public void TransferToCard()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.TransferToCard(pss);
          }
        }
      }

      [Run]
      public void TransferToCardWithBillingAddressInfo()
      {
        var conf = LACONF.AsLaconicConfig();

        using (new ServiceBaseApplication(null, conf))
        {
          var ps = PaySystem.Instances["mock"];
          using (var pss = ps.StartSession())
          {
            PayTestCommon.TransferToCardWithBillingAddressInfo(pss);
          }
        }
      }


    #endregion
    #region .pvt/implementation

      private PaySystem getPaySystem()
      {
        var paymentSection = LACONF.AsLaconicConfig()[WebSettings.CONFIG_WEBSETTINGS_SECTION][NFX.Web.Pay.PaySystem.CONFIG_PAYMENT_PROCESSING_SECTION];

        var stripeSection = paymentSection.Children.First(p => p.AttrByName("name").Value == "Mock");

        var ps = PaySystem.Make<MockSystem>(null, stripeSection);

        return ps;
      }

    #endregion

  }
}
