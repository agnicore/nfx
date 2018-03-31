using System;
using System.Collections.Generic;
using System.Text;

using NFX.CodeAnalysis.Laconfig;
using NFX.Environment;

namespace NFX.CodeAnalysis.Transpilation.LJS
{
  /// <summary>
  /// Represents a scope of LJS unit transpilation (such as a file). The transpilation is done
  /// fragment-by-fragment, where every fragment is excised from the JS file
  /// </summary>
  public class LJSUnitTranspilationContext : CommonCodeProcessor, IAnalysisContext
  {
    public LJSUnitTranspilationContext() : this( null, null, false)
    {
    }

    public LJSUnitTranspilationContext(IAnalysisContext context, MessageList messages = null, bool throwErrors = false)
     : base( context, messages, throwErrors)
    {
    }

    [Config("t|tr|tran|trans|transpiler")]
    public IConfigSectionNode TranspilerConfig{ get; set;}

    public override Language Language => LJSLanguage.Instance;

    public override string MessageCodeToString(int code) => ((LaconfigMsgCode)code).ToString();

    /// <summary>
    /// Factory method that makes new configured instance of transpiler per supplied configuration
    /// </summary>
    public virtual LJSFragmentTranspiler MakeAndConfigureTranspiler(LJSParser parser)
    {
      var node = TranspilerConfig;

      if (node==null || !node.Exists)
        return new LJSFragmentTranspiler(this, parser, this.Messages, false);

      var result = FactoryUtils.MakeAndConfigure<LJSFragmentTranspiler>(node,
                                                                        typeof(LJSFragmentTranspiler),
                                                                        new object[]{this, parser, this.Messages, false});
      return result;
    }
  }
}
