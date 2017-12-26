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
using System.Reflection;

using NFX.Scripting;

namespace NFX.UTest
{
    [Runnable(category: TRUN.BASE_RUNNER, order: -1)]
    public class RunnerHookTests : IRunnableHook, IRunHook
    {
        [Run] public void M01(){ }
        [Run] public void M02(){ }
        [Run] public void M03(){ }
        [Run] public void M04(){ }
        [Run, Run, Run] public void M05(){ }

        private int m_RunnableState;
        private int m_RunPrologueCount;
        private int m_RunEpilogueCount;

        void IRunnableHook.Prologue(Runner runner, FID id)
        {
          Aver.AreEqual(0, m_RunnableState);
          m_RunnableState++;
          Console.WriteLine("Runnable prologue");
        }

        bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
        {
          Aver.AreEqual(1, m_RunnableState);
          Aver.AreEqual(7, m_RunPrologueCount);
          Aver.AreEqual(7, m_RunEpilogueCount);
          Console.WriteLine("Runnable epilogue");
          return false;
        }

        bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
        {
          Aver.AreEqual(1, m_RunnableState);
          m_RunPrologueCount++;
          Console.WriteLine("Method prologue: "+method.Name);
          return false;
        }

        bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
        {
          m_RunEpilogueCount++;
          Console.WriteLine("Method epilogue: "+method.Name);
          return false;
        }
    }

    [Runnable(category: "runner", order: -5999)]
    public class RunnerHookExceptions_RunnableProlog : IRunnableHook
    {
        [Run("!crash-runnable-prologue", "")]
        public void BadMethod(){ }

        void IRunnableHook.Prologue(Runner runner, FID id)
        {
          Aver.Fail("I crashed in Runnable prologue");
        }

        bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
        {
          return false;
        }
    }

    [Runnable(category: "runner", order: -5000)]
    public class RunnerHookExceptions_RunnableEpilogue : IRunnableHook
    {
        [Run("!crash-runnable-epilogue", "")]
        public void BadMethod(){ }

        void IRunnableHook.Prologue(Runner runner, FID id)
        {
        }

        bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
        {
          Aver.Fail("I crashed in Runnable epilogue");
          return false;
        }
    }

    [Runnable(category: "runner", order: -1999)]
    public class RunnerHookExceptions_RunPrologue : IRunHook
    {
        [Run("!crash-run-prologue", "")]
        public void BadMethod(){ }

        bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
        {
          Aver.Fail("I crashed in Run prologue");
          return false;
        }

        bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
        {
          return false;
        }
    }

    [Runnable(category: "runner", order: -1000)]
    public class RunnerHookExceptions_RunEpilogue : IRunHook
    {
        [Run("!crash-run-epilogue", "")]
        public void BadMethod(){ }

        bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
        {
          return false;
        }

        bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
        {
          Aver.Fail("I crashed in Run epilogue");
          return false;
        }
    }


}
