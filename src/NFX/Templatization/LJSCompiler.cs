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
using System.Text.RegularExpressions;

using NFX.Environment;
using NFX.CodeAnalysis.Transpilation.LJS;
using NFX.CodeAnalysis.Source;

namespace NFX.Templatization
{
  /// <summary>
  /// Compiles templates based of text files that use Laconic Java Script language syntax.
  /// Note: This compiler supersedes TextJSTemplateCompiler
  /// </summary>
  /// <example>
  ///   <code>
  ///   /***@ relative-include-file-path ***/
  ///   function() {
  ///     /***
  ///     div=idName{ //comment
  ///       attr1=value1
  ///       attr2=value2 attr3="value 3"
  ///       # //java script block
  ///       # let x = 0;
  ///       span{literal content of span}
  ///     }
  ///     ***/
  ///   }
  ///   </code>
  /// </example>
  public class LJSCompiler : TemplateCompiler
  {
    #region CONSTS
      /// <summary>
      /// Include files:   /***@ path ***/
      /// </summary>
      public static readonly Regex RX_FILE_INCLUDE = new Regex(@"\/\*{3}\@(.*?)\*{3}\/", RegexOptions.Singleline);

      /// <summary>
      /// Block content fragment:   /***  content ***/
      /// </summary>
      public static readonly Regex RX_BLOCK_FRAGMENT = new Regex(@"\/\*{3}(.*?)\*{3}\/", RegexOptions.Singleline);


    #endregion


    #region .ctor
    public LJSCompiler() : base() { }

    public LJSCompiler(params ITemplateSource<string>[] sources) : base(sources) { }

    public LJSCompiler(IEnumerable<ITemplateSource<string>> sources) : base(sources) { }
    #endregion

    #region Fields
     private IConfigSectionNode m_TranspilerConfig;
    #endregion

    #region Properties
    public override string LanguageName                => CoreConsts.JS_LANGUAGE;
    public override string LanguageSourceFileExtension => CoreConsts.JS_EXTENSION;

    [Config("trans|transpiler|config|conf")]
    public IConfigSectionNode TranspilerConfig
    {
      get { return m_TranspilerConfig; }
      set
      {
        EnsureNotCompiled();
        m_TranspilerConfig = value;
      }
    }
    #endregion

    #region Protected


    protected override void DoCompileTemplateSource(CompileUnit unit)
    {
      var icname =  unit.TemplateSource.InferClassName();
      var rawSourceToProcess =  processUnitRelativeFileIncludePragmas(unit);

      var ctxUnit =  FactoryUtils.MakeAndConfigure<LJSUnitTranspilationContext>(m_TranspilerConfig,
                                                                                typeof(LJSUnitTranspilationContext),
                                                                                new []{ unit.TemplateSource.GetName(64) });

      string transpiledUnitSource;

      transpiledUnitSource = transpileFragment(ctxUnit, rawSourceToProcess,RX_BLOCK_FRAGMENT); //  /***  content ***/

      unit.CompiledSource = transpiledUnitSource;
    }

    protected override void DoCompileCode()
    {
      throw new NotSupportedException("{0} does not support code compilation".Args(nameof(LJSCompiler)));
    }
    #endregion

    #region .pvt

    //evaluates CompileUnit include pragmas - putting them all together in one string the pattern:
    // /***@ relative-file-path ***/  gets expanded into file content
    private string processUnitRelativeFileIncludePragmas(CompileUnit unit)
    {
      var source = unit.TemplateSource.GetSourceContent().ToString().Trim();
      var path = string.Empty;
      try
      {
        return RX_FILE_INCLUDE.Replace(source, m => {
          path = m.Groups[1].AsString().Trim();
          return unit.TemplateSource.GetRelativeContent(path).AsString();
        });
      }
      catch (Exception error)
      {
        throw new TemplateParseException(StringConsts.TEMPLATE_JS_COMPILER_INCLUDE_ERROR.Args(path, error.Message), error);
      }
    }

    private string transpileFragment(LJSUnitTranspilationContext ctxUnit,
                                     string rawUnitSource,
                                     Regex pattern)
    {
      string location = "";
      try
      {
        var result = pattern.Replace(rawUnitSource, match =>
        {
          var idx = match.Index;
          if (idx>20) idx-=20;
          var snippet = rawUnitSource.Substring(idx).TakeFirstChars(20);
          snippet = snippet.Replace("\r\n", "\\n").Replace("\n","\\n");
          location = "'... {0} ...'  on line: {1}".Args(snippet, rawUnitSource.IndexToLineNumber(match.Index) );

          var rawFragmentSource = match.Groups[1].Value;

          var src = new StringSource(rawFragmentSource);
          return ctxUnit.TranspileFragmentToString(src);
        });
        return result;
      }
      catch (Exception error)
      {
        throw new TemplateParseException(StringConsts.TEMPLATE_LJS_FRAGMENT_TRANSPILER_ERROR.Args(location, error.ToMessageWithType()), error);
      }
    }

    #endregion
  }
}
