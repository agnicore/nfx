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
using System.Linq;

using NFX.ApplicationModel;
using NFX.Scripting;
using NFX.Web;
using NFX.Web.Pay;
using NFX.Web.Pay.Stripe;
using static NFX.Aver.ThrowsAttribute;

namespace NFX.ITest.Web.Pay
{
  [Runnable]
  public class StripeTest : NFX.ITest.ExternalCfg
  {
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
    [Aver.Throws(typeof(PaymentStripeException), Message = "declined", MsgMatch = MatchType.Contains)]
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
    [Aver.Throws(typeof(PaymentStripeException), Message = "card number is incorrect", MsgMatch = MatchType.Contains)]
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
    [Aver.Throws(typeof(PaymentStripeException), Message = "expiration year is invalid", MsgMatch = MatchType.Contains)]
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
    [Aver.Throws(typeof(PaymentStripeException), Message = "expiration month is invalid", MsgMatch = MatchType.Contains)]
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
    [Aver.Throws(typeof(PaymentStripeException), Message = "security code is invalid", MsgMatch = MatchType.Contains)]
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

    // dlatushkin 20141201:
    //   refund reason
    //   actualaccountdata: +zip
    //   charge: + other address attributes
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
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
        var ps = PaySystem.Instances["stripe"];
        using (var pss = ps.StartSession())
        {
          PayTestCommon.TransferToCardWithBillingAddressInfo(pss);
        }
      }
    }

    private PaySystem getPaySystem()
    {
      var paymentSection = LACONF.AsLaconicConfig()[WebSettings.CONFIG_WEBSETTINGS_SECTION][NFX.Web.Pay.PaySystem.CONFIG_PAYMENT_PROCESSING_SECTION];

      var stripeSection = paymentSection.Children.First(p => p.AttrByName("name").Value == "Stripe");

      var ps = PaySystem.Make<StripeSystem>(null, stripeSection);

      return ps;
    }
  }
}
