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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFX.Scripting;

using NFX.Serialization.CSV;

namespace NFX.UTest.Serialization
{
  [Runnable]
  public class CSVParserTest
  {
    private bool same(IEnumerable<string> got, params string[] elm) { return got.SequenceEqual(elm); }

    [Run]
    public void ParseCSVRow_SimpleRecord()
    {
      Aver.IsTrue(same("aaa,bbb,ccc,".ParseCSVRow(), "aaa", "bbb", "ccc", ""));
      Aver.IsTrue(same("aaa bbb,ccc,".ParseCSVRow(), "aaa bbb", "ccc", ""));
      Aver.IsTrue(same("  aaa  ,  bbb,ccc  ,  ".ParseCSVRow(), "  aaa  ", "  bbb", "ccc  ", "  "));
      Aver.IsTrue(same("  aaa bbb,ccc ddd  ".ParseCSVRow(), "  aaa bbb", "ccc ddd  "));
    }

    [Run]
    public void ParseCSVRow_SimpleRecordTrim()
    {
      Aver.IsTrue(same("  aaa  ,  bbb,ccc  ,  ".ParseCSVRow(true), "aaa", "bbb", "ccc", ""));
      Aver.IsTrue(same("  aaa bbb,ccc ddd  ".ParseCSVRow(true), "aaa bbb", "ccc ddd"));
    }

    [Run]
    public void ParseCSVRow_QuotedRecord()
    {
      Aver.IsTrue(same("\"aaa\",\"bbb\",\"ccc\",\"\"".ParseCSVRow(), "aaa", "bbb", "ccc", ""));
      Aver.IsTrue(same("\"aaa,bbb\",\"ccc,\",\",ddd\"".ParseCSVRow(), "aaa,bbb", "ccc,", ",ddd"));
      Aver.IsTrue(same(@"  ""aaa""  ,  ""bbb"",  ""  ,  """.ParseCSVRow(), "  \"aaa\"  ", "  \"bbb\"", "  \"  ", "  \""));
      Aver.IsTrue(same(@"""  aaa  "",""  bbb"",""ccc  "",""  """.ParseCSVRow(), "  aaa  ", "  bbb", "ccc  ", "  "));
      Aver.IsTrue(same(@"  ""  aaa  ""  ,  ""  bbb  "",  ""  ""  ,  ""  """.ParseCSVRow(), "  \"  aaa  \"  ", "  \"  bbb  \"", "  \"  \"  ", "  \"  \""));
      Aver.IsTrue(same(@"""aaa  ,  bbb"",""ccc,  "",""  ,ddd""".ParseCSVRow(), "aaa  ,  bbb", "ccc,  ", "  ,ddd"));
      Aver.IsTrue(same(
@"""aaa
bbb"",""
ccc"",""ddd
""".ParseCSVRow(),
@"aaa
bbb",
@"
ccc",
@"ddd
"));
      Aver.IsTrue(same(
@"""aaa
bbb
ccc"",""
""".ParseCSVRow(),
@"aaa
bbb
ccc",
@"
"));
    }

    [Run]
    public void ParseCSVRow_QuotedRecordTrim()
    {
      Aver.IsTrue(same(@"  ""aaa""  ,  ""bbb"",""ccc""  ,  """"  ,  """",""""  ".ParseCSVRow(true), "aaa", "bbb", "ccc", "", "", ""));
      Aver.IsTrue(same(@"""  aaa  "",""  bbb"",""ccc  "",""  """.ParseCSVRow(true), "  aaa  ", "  bbb", "ccc  ", "  "));
      Aver.IsTrue(same(@"  ""  aaa  ""  ,  ""  bbb  "",""  ccc  ""  ,  ""  ""  ,  ""  "",""  ""  ".ParseCSVRow(true), "  aaa  ", "  bbb  ", "  ccc  ", "  ", "  ", "  "));
      Aver.IsTrue(same(@"""aaa  ,  bbb"",""ccc,  "",""  ,ddd""".ParseCSVRow(true), "aaa  ,  bbb", "ccc,  ", "  ,ddd"));
    }

    [Run]
    public void ParseCSVRow_QuotedRecordWithQuote()
    {
      Aver.IsTrue(same("\"\"\"\",\"  \"\"  \",\"  \"\"\",\"\"\"  \"".ParseCSVRow(), "\"", "  \"  ", "  \"", "\"  "));
      Aver.IsTrue(same("\",\"\",\",\"  \"\",  \",\",  \"\"\",\"\"\"  ,\"".ParseCSVRow(), ",\",", "  \",  ", ",  \"", "\"  ,"));
    }

    [Run]
    public void ParseCSVRow_SimpleRecordWithQuote()
    {
      Aver.IsTrue(same("aaa\"bbb,ccc\"".ParseCSVRow(), "aaa\"bbb", "ccc\""));
    }

    [Run]
    public void ParseCSVRow_BreakOnLineBreaks()
    {
      Aver.IsTrue(same(
@"aaa,bbb,ccc,
ddd".ParseCSVRow(), "aaa", "bbb", "ccc", ""));
      Aver.IsTrue(same(
@"aaa,bbb,ccc
ddd".ParseCSVRow(), "aaa", "bbb", "ccc"));
      Aver.IsTrue(same("aaa,bbb,ccc,\nddd".ParseCSVRow(), "aaa", "bbb", "ccc", ""));
      Aver.IsTrue(same("aaa,bbb,ccc\r\nddd".ParseCSVRow(), "aaa", "bbb", "ccc"));
    }

    [Run]
    public void ParseCSV()
    {
      var csv =
@"aaa,aaa,aaa,
aaa aaa,aaa,,
  aaa  ,  aaa,aaa  ,
  aaa aaa,aaa aaa  ,,
""aaa"",""aaa"",""aaa"",""""
""aaa,aaa"",""aaa,"","",aaa""
  ""aaa""  ,  ""aaa"",  ""aaa  ,aaa  ""
""  aaa  "",""  aaa"",""aaa  "",""  ""
  ""  aaa  ""  ,  ""  aaa  "",  "" aaa ""  ,  "" aaa ""
""aaa  ,  aaa"",""aaa,  "",""  ,aaa""
""aaa
aaa"",""
aaa"",""aaa
""
""aaa
aaa
aaa"",""
""";
      foreach (var row in csv.ParseCSV(trim: true, skipHeader: true, columns: 3, skipIfMore: true, addIfLess: true))
      {
        var count = 0;
        foreach (var value in row)
        {
          Aver.IsTrue(value.IsNullOrWhiteSpace() || value.Contains("aaa"));
          count++;
        }
        Console.WriteLine(count);
      }
      foreach (var row in csv.ParseCSV(skipHeader: true, columns: 3, skipIfMore: true, addIfLess: true))
      {
        var count = 0;
        foreach (var value in row)
        {
          Aver.IsTrue(value.IsNullOrWhiteSpace() || value.Contains("aaa"));
          count++;
        }
        Console.WriteLine(count);
      }
      foreach (var row in csv.ParseCSV(columns: 3, skipIfMore: true, addIfLess: true))
      {
        var count = 0;
        foreach (var value in row)
        {
          Aver.IsTrue(value.IsNullOrWhiteSpace() || value.Contains("aaa"));
          count++;
        }
        Console.WriteLine(count);
      }
    }
  }
}
