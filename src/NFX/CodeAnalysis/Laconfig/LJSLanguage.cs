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

using NFX.CodeAnalysis.Source;

namespace NFX.CodeAnalysis.Laconfig
{
  /// <summary>
  /// Represents Laconic Java Script language which is based on Laconfig Lexer
  /// </summary>
  public sealed class LJSLanguage : Language
  {
    public static readonly LJSLanguage Instance = new LJSLanguage();

    private LJSLanguage() : base() {}


    public override LanguageFamily Family
    {
        get { return LanguageFamily.StructuredConfig; }
    }

    public override IEnumerable<string> FileExtensions
    {
        get
        {
            yield return "ljs";
        }
    }

    //LJS uses Laconfig Lexer but a different parser
    public override ILexer MakeLexer(IAnalysisContext context, SourceCodeRef srcRef, ISourceText source, MessageList messages = null, bool throwErrors = false)
    {
        return new LaconfigLexer(context, srcRef, source, messages, throwErrors);
    }
  }
}
