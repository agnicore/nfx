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
using System;
using System.Linq;
using NFX.Scripting;

using NFX.Web.Pay;
using NFX.Web.Pay.Mock;

namespace NFX.UTest.Web.Pay
{
  [Runnable]
  public class FeeTests
  {
    private const string CURRENCY_USD = "usd";
    private const string CURRENCY_EUR = "eur";
    private const string CURRENCY_RUB = "rub";

    private const string NO_CURRENCIES_LACONF = @"
      pay-system
      {
        name='Mock'
        type='NFX.Web.Pay.Mock.MockSystem, NFX.Web'
        auto-start=true
      }
    ";

    private const string LACONF = @"
      pay-system
      {
        name='Mock'
        type='NFX.Web.Pay.Mock.MockSystem, NFX.Web'
        auto-start=true
        currencies 
        { 
          /* For all transaction type */
          usd { fee-flat='0.30' fee-pct='2.8'                     }

          /* All transaction type have the same fee except refund */
          rub { fee-flat='1.5'  fee-pct='2.5'                     }
          rub { fee-flat='5'    fee-pct='10'  tran-type='Refund' }

          /* Only charge is supported */
          eur { fee-flat='0.30' fee-pct='3'   tran-type='Charge' }
        }
      }
    ";

    private const string CHARGE_ONLY_LACONF = @"
      pay-system
      {
        name='Mock'
        type='NFX.Web.Pay.Mock.MockSystem, NFX.Web'
        auto-start=true
        currencies 
        { 
          eur { fee-flat='0.30' fee-pct='3' tran-type='Charge' }
        }
      }
    ";

    [Run]
    public void Fee_NoSupportedCurrencies_ByDefault()
    {
      var ps = getPaySystem(NO_CURRENCIES_LACONF);

      var currencies = ps.SupportedCurrencies;

      Aver.IsNotNull(currencies);
      Aver.AreEqual(0, currencies.Count());
    }

    [Run]
    public void Fee_SupportedCurrencies()
    {
      var ps = getPaySystem(LACONF);

      var currencies = ps.SupportedCurrencies;

      Aver.IsNotNull(currencies);
      Aver.AreEqual(3, currencies.Count());

      Aver.IsTrue(currencies.ToArray().Contains(CURRENCY_USD));
      Aver.IsTrue(currencies.ToArray().Contains(CURRENCY_EUR));
      Aver.IsTrue(currencies.ToArray().Contains(CURRENCY_RUB));
    }

    #warning Should be rewritten
    /*
    [Run]
    [Aver.Throws(typeof(PaymentException))]
    public void Fee_Flat_UnsupportedCurrency()
    {
      var ps = getPaySystem(NO_CURRENCIES_LACONF);
      ps.GetTransactionFee(CURRENCY_USD, TransactionType.Charge);
    }

    [Run]
    [Aver.Throws(typeof(PaymentException))]
    public void Fee_Pct_UnsupportedCurrency()
    {
      var ps = getPaySystem(NO_CURRENCIES_LACONF);
      ps.GetTransactionPct(CURRENCY_USD, TransactionType.Charge);
    }

    [Run]
    public void Fee_Flat_Implicit_Lookup()
    {
      var ps = getPaySystem(LACONF);
      var flat = ps.GetTransactionFee(CURRENCY_USD, TransactionType.Charge);

      Aver.AreEqual(CURRENCY_USD, flat.CurrencyISO);
      Aver.AreEqual(0.30M, flat.Value);
    }

    [Run]
    public void Fee_Pct_Implicit_Lookup()
    {
      var ps = getPaySystem(LACONF);
      var pct = ps.GetTransactionPct(CURRENCY_USD, TransactionType.Charge);

      Aver.AreEqual(2.8 * 10000, pct);
    }

    [Run]
    public void Fee_Explicit_Lookup()
    {
      var ps = getPaySystem(LACONF);

      var type = TransactionType.Refund;
      var fee = ps.GetTransactionFee(CURRENCY_RUB, type);
      var pct = ps.GetTransactionPct(CURRENCY_RUB, type);

      Aver.AreEqual(CURRENCY_RUB, fee.CurrencyISO);
      Aver.AreEqual(5, fee.Value);
      Aver.AreEqual(10 * 10000, pct);
    }

    [Run]
    public void Fee_Implicit_Lookup()
    {
      var ps = getPaySystem(LACONF);

      var type = TransactionType.Charge;
      var fee = ps.GetTransactionFee(CURRENCY_RUB, type);
      var pct = ps.GetTransactionPct(CURRENCY_RUB, type);

      Aver.AreEqual(CURRENCY_RUB, fee.CurrencyISO);
      Aver.AreEqual(1.5, fee.Value);
      Aver.AreEqual(2.5 * 10000, pct);
    }
    */
    [Run]
    public void Fee_TranType_NoSupportedTypes_ByDefault()
    {
      var ps = getPaySystem(NO_CURRENCIES_LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Charge);

      Aver.IsFalse(result);
    }

    [Run]
    public void Fee_TranType_Explicit_WithCurrency()
    {
      var ps = getPaySystem(LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Charge, CURRENCY_EUR);

      Aver.IsTrue(result);
    }

    [Run]
    public void Fee_TranType_Implicit_WithCurrency()
    {
      var ps = getPaySystem(LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Charge, CURRENCY_USD);

      Aver.IsTrue(result);
    }

    [Run]
    public void Fee_TranType_Explicit_NoCurrency()
    {
      var ps = getPaySystem(LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Charge);

      Aver.IsTrue(result);
    }

    [Run]
    public void Fee_TranType_Implicit_NoCurrency()
    {
      var ps = getPaySystem(LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Transfer);

      Aver.IsTrue(result);
    }

    [Run]
    public void Fee_TranType_Unsupported_WithCurrency()
    {
      var ps = getPaySystem(CHARGE_ONLY_LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Transfer, CURRENCY_EUR);

      Aver.IsFalse(result);
    }

    [Run]
    public void Fee_TranType_Unsupported_NoCurrency()
    {
      var ps = getPaySystem(CHARGE_ONLY_LACONF);

      var result = ps.IsTransactionTypeSupported(TransactionType.Transfer);

      Aver.IsFalse(result);
    }

    private PaySystem getPaySystem(string laconf)
    {
      return PaySystem.Make<MockSystem>(null, laconf.AsLaconicConfig());
    }
  }
}
