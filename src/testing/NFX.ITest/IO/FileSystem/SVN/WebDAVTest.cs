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
using System.IO;
using NFX.IO.FileSystem.SVN;
using NFX.Scripting;
using NFX.Web;

namespace NFX.ITest.IO.FileSystem.SVN
{
  [Runnable]
  public class WebDAVTest: ExternalCfg
  {
    [Run]
    public void ItemProperties()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        Aver.IsTrue(root.Version.AsInt() > 0);
        Aver.IsTrue(root.CreationDate > DateTime.MinValue);
        Aver.IsTrue(root.LastModificationDate > DateTime.MinValue);

        var maxVersionChild = root.Children.OrderByDescending(c => c.Version).First();

        Console.WriteLine("First Child: " + maxVersionChild);

        Aver.IsTrue(maxVersionChild.Version.AsInt() > 0);
        Aver.IsTrue(maxVersionChild.CreationDate > DateTime.MinValue);
        Aver.IsTrue(maxVersionChild.LastModificationDate > DateTime.MinValue);

        Aver.AreEqual(root.Version, maxVersionChild.Version);
      }
    }

    [Run]
    public void DirectoryChildren()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        var children = root.Children;

        Aver.IsNotNull(children);
        Aver.AreEqual(3, children.Count());
        Aver.IsTrue(children.Any(c => c.Name == "trunk"));

        var firstChild = children.First();
        Aver.AreObjectsEqual(root, firstChild.Parent);
      }
    }

    [Run]
    public void NavigatePathFolder()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        WebDAV.Directory nested = root.NavigatePath("/trunk/Source/NFX") as WebDAV.Directory;

        Aver.IsNotNull(nested);
        Aver.AreEqual("NFX", nested.Name);
        Aver.AreEqual("/trunk/Source/NFX", nested.Path);

        Aver.AreEqual("Source", nested.Parent.Name);
        Aver.AreEqual("/trunk/Source", nested.Parent.Path);

        Aver.AreEqual("trunk", nested.Parent.Parent.Name);
        Aver.AreEqual("/trunk", nested.Parent.Parent.Path);
      }
    }

    [Run]
    public void NavigatePathFile()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        WebDAV.File nested = root.NavigatePath("/trunk/Source/NFX/LICENSE.txt") as WebDAV.File;

        Aver.IsNotNull(nested);
        Aver.AreEqual("LICENSE.txt", nested.Name);

        using (MemoryStream s = new MemoryStream())
        {
          nested.GetContent(s);
          Aver.IsTrue(s.Length > 0);
        }
      }
    }

    [Run]
    public void ContentType()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        var file1 = root.NavigatePath("/trunk/Source/NFX/LICENSE.txt");
        var file2 = root.NavigatePath("/trunk/Source/NFX.Wave/Templatization/StockContent/Embedded/flags/ad.png");

        Aver.AreEqual(0, string.Compare("text/xml; charset=\"utf-8\"", file1.ContentType, true));
        Aver.AreEqual(0, string.Compare("application/octet-stream", file2.ContentType, true));
      }
    }

    [Run]
    public void FileContent()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        WebDAV.File file = root.Children.First(c => c is WebDAV.File) as WebDAV.File;

        using (MemoryStream ms = new MemoryStream())
        {
          file.GetContent(ms);
          Aver.IsTrue(ms.Length > 0);
        }
      }
    }

    [Run]
    public void GetVersions()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        IList<WebDAV.Version> versions = WebDAV.GetVersions(SVN_ROOT, SVN_UNAME, SVN_UPSW).ToList();

        Aver.IsNotNull(versions);
        Aver.IsTrue(versions.Count() > 0);

        WebDAV.Version v1 = versions.First();

        Aver.IsNotNull(v1);
        Aver.IsTrue(v1.Creator.IsNotNullOrEmpty());
        Aver.IsTrue(v1.Comment.IsNotNullOrEmpty());
        Aver.AreEqual("1", v1.Name);
        Aver.IsTrue(v1.Date > DateTime.MinValue);
      }
    }

    [Run]
    public void DifferentDirectoryVersions()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        IList<WebDAV.Version> versions = WebDAV.GetVersions(SVN_ROOT, SVN_UNAME, SVN_UPSW).ToList();

        WebDAV.Version v1513 = versions.First(v => v.Name == "1513");
        WebDAV.Version v1523 = versions.First(v => v.Name == "1523");

        var client1513 = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW, version: v1513);

        var client1523 = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW, version: v1523);

        WebDAV.Directory root1513 = client1513.Root;
        WebDAV.Directory root1523 = client1523.Root;

        WebDAV.Directory nested1513 = root1513.NavigatePath("trunk/Source/NFX.Web/IO/FileSystem") as WebDAV.Directory;
        WebDAV.Directory nested1523 = root1523.NavigatePath("trunk/Source/NFX.Web/IO/FileSystem") as WebDAV.Directory;

        Aver.IsNull(nested1513["SVN"]);
        Aver.IsNotNull(nested1523["SVN"]);
      }
    }

    [Run]
    public void DifferentFileVersions()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        IList<WebDAV.Version> versions = WebDAV.GetVersions(SVN_ROOT, SVN_UNAME, SVN_UPSW).ToList();

        WebDAV.Version v1530 = versions.First(v => v.Name == "1530");
        WebDAV.Version v1531 = versions.First(v => v.Name == "1531");

        var client1530 = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW, version: v1530);

        var client1531 = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW, version: v1531);

        WebDAV.Directory root1530 = client1530.Root;
        WebDAV.Directory root1531 = client1531.Root;

        WebDAV.File file1530 = root1530.NavigatePath("trunk/Source/NFX.Web/IO/FileSystem/SVN/WebDAV.cs") as WebDAV.File;
        WebDAV.File file1531 = root1531.NavigatePath("trunk/Source/NFX.Web/IO/FileSystem/SVN/WebDAV.cs") as WebDAV.File;

        using (MemoryStream ms1530 = new MemoryStream())
        {
          using (MemoryStream ms1531 = new MemoryStream())
          {
            file1530.GetContent(ms1530);
            file1531.GetContent(ms1531);

            Aver.AreNotEqual(ms1530.Length, ms1531.Length);
          }
        }
      }
    }

    [Run]
    public void NonExistingItem()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Directory root = client.Root;

        var children = root.Children;

        var nonexistingChild = root[children.OrderBy(c => c.Name.Length).First().Name + "_"];

        Aver.IsNull(nonexistingChild);
      }
    }

    [Run]
    public void GetHeadRootVersion()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        WebDAV.Version lastVersion = client.GetHeadRootVersion();
        Aver.IsNotNull(lastVersion);
      }
    }

    [Run]
    public void EscapeDir()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        {
          var d = client.Root.NavigatePath("trunk/Source/Testing/NUnit/NFX.ITest/IO/FileSystem/SVN") as WebDAV.Directory;
          Aver.IsNotNull(d.Children.FirstOrDefault(c => c.Name == "Esc Folder+"));
        }

        {
          var d = client.Root.NavigatePath("trunk/Source/Testing/NUnit/NFX.ITest/IO/FileSystem/SVN/Esc Folder+") as WebDAV.Directory;
          Aver.IsNotNull(d);
          Aver.AreEqual("Esc Folder+", d.Name);
        }
      }
    }

    [Run]
    public void EscapeFile()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        var f = client.Root.NavigatePath("trunk/Source/Testing/NUnit/NFX.ITest/IO/FileSystem/SVN/Esc Folder+/Escape.txt") as WebDAV.File;
        var ms = new MemoryStream();
        f.GetContent(ms);
        var s = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        Console.WriteLine(s);
        Aver.AreEqual("Escape file+content", s);
      }
    }

    [Run]
    public void GetManyFiles()
    {
      using(new NFX.ApplicationModel.ServiceBaseApplication(null, LACONF.AsLaconicConfig()))
      {
        var client = new WebDAV(SVN_ROOT, 0, SVN_UNAME, SVN_UPSW);

        var d = client.Root.NavigatePath("trunk/Source/NFX.Wave/Templatization/StockContent/Embedded/flags") as WebDAV.Directory;

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        foreach (WebDAV.File f in d.Children)
        {
          MemoryStream ms = new MemoryStream();
          f.GetContent(ms);

          Console.WriteLine("{0} done".Args(f.Name));
        }
        stopwatch.Stop();

        Console.WriteLine(stopwatch.Elapsed);
      }
    }

    [Run]
    [Aver.Throws(typeof(System.Net.WebException), MsgMatch= Aver.ThrowsAttribute.MatchType.Contains)]
    public void FailedFastTimeout()
    {
      var conf = LACONF.AsLaconicConfig();

      using (var app = new NFX.ApplicationModel.ServiceBaseApplication(new string[] {}, conf))
      {
        try
        {
          var client = new WebDAV(SVN_ROOT, 1, SVN_UNAME, SVN_UPSW);

          WebDAV.Directory root = client.Root;
          var children = root.Children;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToMessageWithType());
          throw;
        }
      }
    }
  }
}
