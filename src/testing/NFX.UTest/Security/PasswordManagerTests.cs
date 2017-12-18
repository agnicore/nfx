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

using NFX.Scripting;

using NFX.Security;
using NFX.Serialization.JSON;

namespace NFX.UTest.Security
{
  [Runnable]
  public class PasswordManagerTests : IRunnableHook
  {
    private IPasswordManagerImplementation m_Manager;

    public IPasswordManagerImplementation Manager {get {return m_Manager;} }

    void IRunnableHook.Prologue(Runner runner, FID id)
    {
      m_Manager = new DefaultPasswordManager();
      m_Manager.Start();
    }

    bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
    {
      m_Manager.SignalStop();
      return false;
    }

    [Run]
    public void CalcStrenghtScore()
    {
      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("qwerty");
      var score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(30, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("qwerty123");
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(93, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("aaaaaaaaaaaaaaaaaaaaaaa");
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(32, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@blue+sky=");
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(198, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(299, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer(null);
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(0, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer(string.Empty);
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(0, score);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("   ");
      score = Manager.CalculateStrenghtScore(PasswordFamily.Text, buf);
      Aver.AreEqual(0, score);
    }

    [Run]
    public void CalcStrenghtPercent()
    {
      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("qwerty");
      var pcnt = Manager.CalculateStrenghtPercent(PasswordFamily.Text, buf);
      Aver.AreEqual(12, pcnt);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      pcnt = Manager.CalculateStrenghtPercent(PasswordFamily.Text, buf);
      Aver.AreEqual(100, pcnt);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      pcnt = Manager.CalculateStrenghtPercent(PasswordFamily.Text, buf, DefaultPasswordManager.TOP_SCORE_MAXIMUM);
      Aver.AreEqual(85, pcnt);
    }

    [Run]
    public void AreEquivalent()
    {
      var pm = new DefaultPasswordManager();
      pm.Start();

      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      var hash1 = Manager.ComputeHash(PasswordFamily.Text, buf);
      var hash2 = HashedPassword.FromString(hash1.ToString());

      try
      {
        Aver.IsTrue(pm.AreEquivalent(hash1, hash2));
        Aver.Fail("no exception");
      }
      catch (NFXException e)
      {
        Aver.AreEqual(e.Message, StringConsts.SERVICE_INVALID_STATE +
                                   typeof(DefaultPasswordManager).Name);
      }

      pm.SignalStop();
      pm.WaitForCompleteStop();

      Aver.IsTrue(pm.AreEquivalent(hash1, hash2));

      Aver.IsFalse(pm.AreEquivalent(null, null));

      var hash3 = new HashedPassword("OTH", hash2.Family);
      hash3["hash"] = hash2["hash"];
      hash3["salt"] = hash2["salt"];
      Aver.IsFalse(pm.AreEquivalent(hash1, hash3));

      hash2 = Manager.ComputeHash(PasswordFamily.Text, buf);
      Aver.IsFalse(pm.AreEquivalent(hash1, hash2));

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("qwerty");
      hash2 = Manager.ComputeHash(PasswordFamily.Text, buf);
      Aver.IsFalse(pm.AreEquivalent(hash1, hash2));
    }

    [Run]
    public void Compute_Verify_Pass()
    {
      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("qwerty");
      var hash = Manager.ComputeHash(PasswordFamily.Text, buf);
      bool rehash, check;

      check = Manager.Verify(buf, hash, out rehash);
      Aver.IsTrue(check);

      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      hash = Manager.ComputeHash(PasswordFamily.Text, buf);
      check = Manager.Verify(buf, hash, out rehash);
      Aver.IsTrue(check);

      check = Manager.Verify(buf, HashedPassword.FromString(hash.ToJSON()), out rehash);
      Aver.IsTrue(check);
    }

    [Run]
    public void Compute_Verify_Fail()
    {
      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      bool rehash, check;

     var hash = Manager.ComputeHash(PasswordFamily.Text, buf);
      buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("qwerty");
      check = Manager.Verify(buf, hash, out rehash);
      Aver.IsFalse(check);
    }

    [Run]
    public void Verify_InvalidHash()
    {
      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");
      var hash = Manager.ComputeHash(PasswordFamily.Text, buf);
      bool rehash, check;

      hash["salt"] = null;
      try
      {
        check = Manager.Verify(buf, hash, out rehash);
        Aver.Fail("no exception");
      }
      catch (NFXException e)
      {
        Aver.IsTrue(e.Message.Contains("ExtractPasswordHashingOptions((hash|hash[salt])==null)"));
      }

      hash = null;
      try
      {
        check = Manager.Verify(buf, hash, out rehash);
        Aver.Fail("no exception");
      }
      catch (NFXException e)
      {
        Aver.IsTrue(e.Message.Contains("Verify((password|hash)==null)"));
      }
    }

    [Run]
    public void CheckServiceActive()
    {
      var pm = new DefaultPasswordManager();
      var buf = IDPasswordCredentials.PlainPasswordToSecureBuffer("@8luE+5ky=");

      try
      {
        var hash = pm.ComputeHash(PasswordFamily.Text, buf);
        Aver.Fail("no exception");
      }
      catch (NFXException e)
      {
        Aver.AreEqual(e.Message, StringConsts.SERVICE_INVALID_STATE +
                                   typeof(DefaultPasswordManager).Name);
      }
    }
  }
}
