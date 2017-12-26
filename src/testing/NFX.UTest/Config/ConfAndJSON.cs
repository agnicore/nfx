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
using NFX.Scripting;

using NFX.Serialization.JSON;

namespace NFX.UTest.Config
{
  [Runnable(TRUN.BASE)]
  public class ConfAndJSON
  {
    [Run]
    public void ConfigSectionNode_2_JSONDataMap()
    {
      var node = @"opt
                  {
                    detailed-instrumentation=true
                    tables
                    {
                      master { name='tfactory' fields-qty=14}
                      slave { name='tdoor' fields-qty=20 important=true}
                    }
                  }".AsLaconicConfig();
      var map = node.ToJSONDataMap();

      Aver.AreEqual(2, map.Count);
      Aver.IsTrue(map["detailed-instrumentation"].AsString() == "true");

      var tablesMap = (JSONDataMap)map["tables"];

      var master = (JSONDataMap)tablesMap["master"];
      Aver.IsTrue(master["name"].AsString() == "tfactory");
      Aver.IsTrue(master["fields-qty"].AsString() == "14");

      var slave = (JSONDataMap)tablesMap["slave"];
      Aver.IsTrue(slave["name"].AsString() == "tdoor");
      Aver.IsTrue(slave["fields-qty"].AsString() == "20");
      Aver.IsTrue(slave["important"].AsString() == "true");
    }

    [Run]
    public void JSONDataMap_2_ConfigSectionNode()
    {
      var map = (JSONDataMap)@" {
                                  'detailed-instrumentation': true,
                                  tables:
                                  {
                                    master: { name: 'tfactory', 'fields-qty': 14},
                                    slave: { name: 'tdoor', 'fields-qty': 20, important: true}
                                  }
                                }".JSONToDataObject();

      var cfg = map.ToConfigNode();

      Aver.AreEqual(1, cfg.Attributes.Count());
      Aver.AreEqual(1, cfg.Children.Count());

      Aver.IsTrue(cfg.AttrByName("detailed-instrumentation").ValueAsBool());

      var tablesNode = cfg.Children.Single(ch => ch.Name == "tables");

      var master = cfg.NavigateSection("tables/master");
      Aver.AreEqual(2, master.Attributes.Count());
      Aver.IsTrue(master.AttrByName("name").ValueAsString() == "tfactory");
      Aver.IsTrue(master.AttrByName("fields-qty").ValueAsInt() == 14);

      var slave = cfg.NavigateSection("tables/slave");
      Aver.AreEqual(3, slave.Attributes.Count());
      Aver.IsTrue(slave.AttrByName("name").ValueAsString() == "tdoor");
      Aver.IsTrue(slave.AttrByName("fields-qty").ValueAsInt() == 20);
      Aver.IsTrue(slave.AttrByName("important").ValueAsBool());
    }

    [Run]
    public void JSONtoLaconicToJSON()//20170414
    {
       var config = "{r:{}}".AsJSONConfig();
       Console.WriteLine(config.ToLaconicString());
       var json = config.ToJSONString();
       Console.WriteLine(json);
       Aver.AreEqual("{\"r\":{}}", json);
    }
  }
}
