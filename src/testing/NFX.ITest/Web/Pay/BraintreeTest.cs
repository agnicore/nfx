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
using NFX.Financial;
using NFX.Scripting;
using NFX.Web.Pay;
using NFX.Web.Pay.Braintree;

namespace NFX.ITest.Web.Pay
{
  [Runnable]
  public class BraintreeTest : IRunnableHook
  {
    public const string LACONF = @"
    nfx
    {
      braintree-server-url='https://api.sandbox.braintreegateway.com'

      starters
      {
        starter
        {
          name='PaySystem'
          type='NFX.Web.Pay.PaySystemStarter, NFX.Web'
          application-start-break-on-exception=true
        }
      }

      web-settings
      {

        payment-processing
        {
          pay-system-host
          {
            name='FakePaySystemHost'
            type='NFX.ITest.Web.Pay.FakePaySystemHost, NFX.ITest'
            pay-system-prefix='BT'
          }

          pay-system
          {
            name='Braintree'
            type='NFX.Web.Pay.Braintree.BraintreeSystem, NFX.Web'
            auto-start=true

            api-uri=$(/$braintree-server-url)

            default-session-connect-params
            {
              name='Braintree'
              type='NFX.Web.Pay.Braintree.BraintreeConnectionParameters, NFX.Web'

              merchant-id=$(~BRAINTREE_SANDBOX_MERCHANT_ID)
              public-key=$(~BRAINTREE_SANDBOX_PUBLIC_KEY)
              private-key=$(~BRAINTREE_SANDBOX_PRIVATE_KEY)
            }
          }
        }
      }
    }";

    private ServiceBaseApplication m_App;

    void IRunnableHook.Prologue(Runner runner, FID id)
    {
      var config = LACONF.AsLaconicConfig(handling: ConvertErrorHandling.Throw);
      m_App = new ServiceBaseApplication(null, config);
    }

    bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
    {
      DisposableObject.DisposeAndNull(ref m_App);
      return false;
    }

    [Run]
    public void ValidNonce()
    {
      Transaction tran = null;
      using (var session = PaySystem.StartSession())
      {
        ((BraintreeSystem)PaySystem).GenerateClientToken(session);

        var fromAccount = new Account("user", FakePaySystemHost.BRAINTREE_WEB_TERM, FakePaySystemHost.BRAINTREE_NONCE);
        var toAccount = Account.EmptyInstance;
        session.StoreAccountData(new ActualAccountData(fromAccount)
          {
            Identity = fromAccount.Identity,
            IsNew = true,
            IsWebTerminal = true,
            AccountID = fromAccount.AccountID,
            FirstName = "Stan",
            LastName = "Ulam",
            Phone = "(333) 777-77-77",
            EMail = "s-ulam@myime.com",
            BillingAddress = new Address { Address1 = "587 KIVA ST", PostalCode = "87544", City = "LOS ALAMOS", Region = "NM", Country = "USA" }
          });
        session.StoreAccountData(new ActualAccountData(toAccount));
        tran = session.Charge(fromAccount, toAccount, new Amount("usd", 99M), capture: false);
      }

      Aver.IsTrue(tran.Status == TransactionStatus.Success);
      Aver.IsTrue(tran.Type == TransactionType.Charge);
      Aver.AreObjectsEqual(tran.Amount, new Amount("usd", 99M));
      Aver.AreObjectsEqual(tran.AmountCaptured, new Amount("usd", 0M));
      Aver.IsTrue(tran.CanCapture);
      Aver.IsFalse(tran.CanRefund);
      Aver.IsTrue(tran.CanVoid);

      tran.Capture();
      Aver.IsTrue(tran.Status == TransactionStatus.Success);
      Aver.AreObjectsEqual(tran.Amount, new Amount("usd", 99M));
      Aver.AreObjectsEqual(tran.AmountCaptured, new Amount("usd", 99M));
      Aver.AreObjectsEqual(tran.AmountRefunded, new Amount("usd", 0M));
      Aver.IsFalse(tran.CanCapture);
      Aver.IsTrue(tran.CanRefund);
      Aver.IsFalse(tran.CanVoid);

      tran.Refund();
      Aver.IsTrue(tran.Status == TransactionStatus.Success);
      Aver.AreObjectsEqual(tran.Amount, new Amount("usd", 99M));
      Aver.AreObjectsEqual(tran.AmountCaptured, new Amount("usd", 99M));
      Aver.AreObjectsEqual(tran.AmountRefunded, new Amount("usd", 99M));
      Aver.IsFalse(tran.CanCapture);
      Aver.IsFalse(tran.CanRefund);
      Aver.IsFalse(tran.CanVoid);
    }

    [Run]
    [Aver.Throws(typeof(PaymentException), Message = "Expired Card", MsgMatch = Aver.ThrowsAttribute.MatchType.Exact)]
    public void DeclinedNonce()
    {
      Transaction tran = null;
      using (var session = PaySystem.StartSession())
      {
        ((BraintreeSystem)PaySystem).GenerateClientToken(session);

        var fromAccount = new Account("user", FakePaySystemHost.BRAINTREE_WEB_TERM, FakePaySystemHost.BRAINTREE_PROCESSOR_DECLINED_VISA_NONCE);
        var toAccount = Account.EmptyInstance;
        session.StoreAccountData(new ActualAccountData(fromAccount)
          {
            Identity = fromAccount.Identity,
            IsNew = true,
            IsWebTerminal = true,
            AccountID = fromAccount.AccountID,
            FirstName = "Stan",
            LastName = "Ulam",
            Phone = "(333) 777-77-77",
            EMail = "s-ulam@myime.com",
            BillingAddress = new Address { Address1 = "587 KIVA ST", PostalCode = "87544", City = "LOS ALAMOS", Region = "NM", Country = "USA" }
          });
        session.StoreAccountData(new ActualAccountData(toAccount));
        tran = session.Charge(fromAccount, toAccount, new Amount("usd", 2004M), capture: false);
      }
    }

    private IPaySystem m_PaySystem;
    public IPaySystem PaySystem
    {
      get
      {
        if(m_PaySystem == null) { m_PaySystem = NFX.Web.Pay.PaySystem.Instances["Braintree"]; }
        return m_PaySystem;
      }
    }
  }
}
