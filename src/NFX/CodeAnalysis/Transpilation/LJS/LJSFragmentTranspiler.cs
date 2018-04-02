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
    public LJSFragmentTranspiler(LJSUnitTranspilationContext context, LJSParser parser,  MessageList messages = null, bool throwErrors = false)
             : base(context, parser, messages, throwErrors)
    {
    }

    public override Language Language => Parser.Language;
    public LJSUnitTranspilationContext UnitContext => (LJSUnitTranspilationContext)base.Context;

    public override string MessageCodeToString(int code)
    {
      return Parser.MessageCodeToString(code);
    }

    protected override void DoTranspile()
    {
      var ljsTree = Parser.ResultContext.ResultObject;
      ljsTree.TranspiledContent = DoTranspileTree(ljsTree);
    }

    protected virtual string DoTranspileTree(LJSTree tree)
    {
      var output = new StringBuilder();
      DoTranspileNode(output, 0, null, tree.Root);
      return output.ToString();
    }

    protected virtual void DoTranspileNode(StringBuilder output, int indentLevel, string idParent, LJSNode node)
    {
      if (node is LJSSectionNode nSection)
      {
        var id = DoEmitSectionNode(output, indentLevel, idParent, nSection);
        foreach(var child in nSection.Children)
          DoTranspileNode(output, indentLevel+1, id, child);
      }
      else if (node is LJSAttributeNode nAttr)
      {
        DoEmitAttributeNode(output, indentLevel, idParent, nAttr);
      }
      else if (node is LJSContentNode nContent)
      {
        DoEmitContentNode(output, indentLevel, idParent, nContent);
      }
      else if (node is LJSScriptNode nScript)
      {
        DoEmitScriptNode(output, indentLevel, idParent, nScript);
      }
      else
       EmitMessage(MessageType.Error,
                   -1,//todo Give proper error code
                   new Source.SourceCodeRef(UnitContext.UnitName),
                  token: node.StartToken,
                  text: "LJSNode type is unsupported: {0}".Args(node.GetType().DisplayNameWithExpandedGenericArgs()));

      output.AppendLine();
    }

    protected virtual void DoPad(StringBuilder output, int indentLevel)
    {
      for(var i=0; i<indentLevel*UnitContext.IndentWidth; i++) output.Append(' ');
    }

    protected virtual string DoEmitScriptNode(StringBuilder output, int indentLevel, string idParent, LJSScriptNode node)
    {
      DoPad(output, indentLevel);              // See  EscapeJSLiteral()
      output.AppendLine(node.Script);//script gets output as-is always on a separate line
      return null;
    }

    protected virtual string DoEmitAttributeNode(StringBuilder output, int indentLevel, string idParent, LJSAttributeNode node)
    {
      var aid = UnitContext.GenerateID();       // See  EscapeJSLiteral()
      //proverit na ? js expression i escape js listeral
      DoPad(output, indentLevel);
      output.AppendFormat("let {0} = {0}.createAttribute('{1}', '{2}');\n", aid, idParent, node.Name, node.Value);
      return aid;
    }

    protected virtual string DoEmitSectionNode(StringBuilder output, int indentLevel, string idParent, LJSSectionNode node)
    {
      var sid = UnitContext.GenerateID();         // See  EscapeJSLiteral()
      //proverit na ? js expression
      //proverit na nazvanie componenta
      //proverit node.GeneratorPragma na alias
      DoPad(output, indentLevel);
      output.AppendFormat("let {0} = {0}.createElement('{1}', '{2}');\n", sid, idParent, node.Name);//Escape java string literal?
      return sid;
    }

    protected virtual string DoEmitContentNode(StringBuilder output, int indentLevel, string idParent, LJSContentNode node)
    {
      var cid = UnitContext.GenerateID();         // See  EscapeJSLiteral()
      //proverit na ? js expression
      //proverit na nazvanie componenta
      //proverit node.GeneratorPragma na alias
      DoPad(output, indentLevel);
      output.AppendFormat("let {0} = {0}.createTextElement('{1}', '{2}');\n", cid, idParent, node.Name);
      return cid;
    }

  }
}
