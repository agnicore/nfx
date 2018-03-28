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

using NFX.Environment;

namespace NFX.CodeAnalysis.Laconfig
{
  /// <summary>
  /// Represents Laconic Java Script parser result - the Laconic Java Script Document Object Model
  /// </summary>
  public sealed class LJSData : ObjectResultAnalysisContext<LJSTree>
  {

      public LJSData(LJSTree dom) : base(null)
      {
          m_ResultObject = dom;
      }


      public override Language Language
      {
          get { return LaconfigLanguage.Instance; }
      }

      public override string MessageCodeToString(int code)
      {
          return ((LaconfigMsgCode)code).ToString();
      }
  }

  /// <summary>
  /// Represents the result of parsing of LJS content into Laconic Java Script Document Object Model
  /// </summary>
  public sealed class LJSTree //wrapper class reserved for tree-wide attrs (if any)
  {
    /// <summary>Tree root </summary>
    public LJSSectionNode Root { get; internal set; }

    /// <summary> Attaches arbitrary data, such as the one used by the generator </summary>
    public object Data{ get; set;}
  }

  public abstract class LJSNode
  {
    /// <summary> The first laconic content token that starts this node </summary>
    public  LaconfigToken StartToken { get; internal set; }

    /// <summary> Parent node that this node is in</summary>
    public  LJSSectionNode   Parent { get; internal set; }

    /// <summary> Node name - name of attribute or section </summary>
    public  string Name { get; internal set; }

    /// <summary> Attaches arbitrary data, such as the one used by the generator </summary>
    public object Data { get; set; }
  }

  public sealed class LJSAttributeNode : LJSNode
  {
    /// <summary>The value of attribute node</summary>
    public string Value { get; internal set; }
  }

  public sealed class LJSSectionNode : LJSNode
  {
    /// <summary>
    /// The name assigned to this section node like div='pragma1'{} to be used by the script/generator,
    /// most likely used for assigning a deterministic variable name in java script to the element
    /// </summary>
    public string GeneratorPragma { get; internal set; }
    /// <summary> All nodes in order of declaration: sections, content, script, attributes</summary>
    public LJSNode[] Children { get; internal set; }
  }

  /// <summary> Represents textual content block, such as:   div{ content block text }</summary>
  public sealed class LJSContentNode : LJSNode
  {
    /// <summary>Textual content</summary>
    public string Content { get; internal set; }
  }

  /// <summary> Represents script textual content block, such as: # let x =1;</summary>
  public sealed class LJSScriptNode : LJSNode
  {
    /// <summary>Textual script content</summary>
    public string Script { get; internal set; }
  }




}
