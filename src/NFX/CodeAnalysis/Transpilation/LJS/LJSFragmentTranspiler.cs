using System;
using System.Collections.Generic;
using System.Text;

using NFX.CodeAnalysis.Laconfig;
using NFX.Environment;

namespace NFX.CodeAnalysis.Transpilation.LJS
{
  /// <summary>
  /// Transpiles a single LJS fragment
  /// </summary>
  public class LJSFragmentTranspiler : Transpiler<LJSParser>
  {
    /// <summary>
    /// Helper facade that assembles processing pipeline and transpiles an LJS fragment into a string
    /// within the specified unit context
    /// </summary>
    public static string TranspileFragmentToString(Source.ISourceText source, LJSUnitTranspilationContext ctxUnit)
    {
      //1 Assemble pipeline
      var lexer = new LaconfigLexer(ctxUnit, source);
      var ctxFragment = new LJSData(ctxUnit);
      var parser = new LJSParser(ctxFragment, lexer);
      //make and configure parser-fragment transpiler in the unit scope
      var transpiler = ctxUnit.MakeAndConfigureTranspiler(parser);

      //2 parse into fragment context
      parser.Parse();

      //3 transpile into fragment context under whole unit scope
      transpiler.Transpile();

      var result = ctxFragment.ResultObject.TranspiledContent;
      return result;
    }




    public LJSFragmentTranspiler(LJSUnitTranspilationContext context, LJSParser parser,  MessageList messages = null, bool throwErrors = false)
             : base(context, parser, messages, throwErrors)
    {
    }


    //[Config] public string TranspilerPropertyExample{get; set;}

    public override Language Language => Parser.Language;
    public LJSUnitTranspilationContext UnitContext => (LJSUnitTranspilationContext)base.Context;

    public override string MessageCodeToString(int code)
    {
      return Parser.MessageCodeToString(code);
    }

    protected override void DoTranspile()
    {
      var ljsTree = Parser.ResultContext.ResultObject;
      ljsTree.TranspiledContent = transpile(ljsTree.Root);
    }

    private string transpile(LJSSectionNode tree)
    {
      var sb = new StringBuilder();
      tree.Print(sb, 0);
      return sb.ToString();
    }
  }
}
