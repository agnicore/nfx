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
using NFX.IO.FileSystem.S3.V4.S3V4Sign;
using NFX.Scripting;

namespace NFX.UTest.IO.FileSystem.S3.V4.S3V4Sign
{
  [Runnable]
  public class S3V4URLHelpersTest
  {
    [Run]
    public void EmptyRegion()
    {
      string uri = S3V4URLHelpers.CreateURIString();
      Aver.AreEqual("https://s3.amazonaws.com/", uri);
    }

    [Run]
    public void BucketRoot()
    {
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw");
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/", uri);
    }

    [Run]
    public void FileRoot()
    {
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw", "MyFile.txt");
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFile.txt", uri);
    }

    [Run]
    public void FolderRoot()
    {
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw", "MyFolder/");
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/", uri);
    }

    [Run]
    public void FolderFile()
    {
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw", "MyFolder/MyFile1.txt");
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/MyFile1.txt", uri);
    }

    [Run]
    public void FolderFileParameterEmpty()
    {
      var queryParams = new Dictionary<string, string>() { {"acl", ""}};
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw", "MyFolder/MyFile1.txt", queryParams);
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/MyFile1.txt?acl=", uri);
    }

    [Run]
    public void FolderFileParameters()
    {
      var queryParams = new Dictionary<string, string>() { { "marker", "1" }, { "max-keys", "100" } };
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw", "MyFolder/MyFile1.txt", queryParams);
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/MyFile1.txt?marker=1&max-keys=100", uri);
    }

    [Run]
    public void FolderFileParametersEncoding()
    {
      var queryParams = new Dictionary<string, string>() { { "delimiter", "/" } };
      string uri = S3V4URLHelpers.CreateURIString("us-west-2", "dxw", "MyFolder/", queryParams);
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/?delimiter=%2F", uri);
    }

    [Run]
    public void ParentEmpty()
    {
      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/");
        string parent = uri.GetParentURL();
        Aver.AreEqual(string.Empty, parent);
      }

      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com");
        string parent = uri.GetParentURL();
        Aver.AreEqual(string.Empty, parent);
      }
    }

    [Run]
    public void ParentFile()
    {
      Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFile1.txt");
      string parent = uri.GetParentURL();
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com", parent);
    }

    [Run]
    public void ParentFolder()
    {
      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFolder");
        string parent = uri.GetParentURL();
        Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com", parent);
      }

      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/");
        string parent = uri.GetParentURL();
        Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com", parent);
      }
    }

    [Run]
    public void ParentFolderFile()
    {
      Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/MyFile1.txt");
      string parent = uri.GetParentURL();
      Aver.AreEqual("https://dxw.s3-us-west-2.amazonaws.com/MyFolder", parent);
    }

    [Run]
    public void LocalNameEmpty()
    {
      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/");
        string parent = uri.GetLocalName();
        Aver.AreEqual(string.Empty, parent);
      }

      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com");
        string parent = uri.GetLocalName();
        Aver.AreEqual(string.Empty, parent);
      }
    }

    [Run]
    public void LocalNameFolder()
    {
      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/");
        string parent = uri.GetLocalName();
        Aver.AreEqual("MyFolder", parent);
      }

      {
        Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFolder");
        string parent = uri.GetLocalName();
        Aver.AreEqual("MyFolder", parent);
      }
    }

    [Run]
    public void LocalNameFile()
    {
      Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFile1.txt");
      string parent = uri.GetLocalName();
      Aver.AreEqual("MyFile1.txt", parent);
    }

    [Run]
    public void LocalNameFolderFile()
    {
      Uri uri = new Uri("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/MyFile1.txt");
      string parent = uri.GetLocalName();
      Aver.AreEqual("MyFile1.txt", parent);
    }

    [Run]
    public void ParseDefaultRegion()
    {
      string bucket;
      string region;
      string itemLocalPath;

      S3V4URLHelpers.Parse("https://dxw.s3.amazonaws.com/", out bucket, out region, out itemLocalPath);

      Aver.AreEqual("dxw", bucket);
      Aver.AreEqual(S3V4URLHelpers.US_EAST_1, region);
      Aver.AreEqual(string.Empty, itemLocalPath);
    }

    [Run]
    public void ParseRoot()
    {
      string bucket;
      string region;
      string itemLocalPath;

      S3V4URLHelpers.Parse("https://dxw.s3-us-west-2.amazonaws.com/", out bucket, out region, out itemLocalPath);

      Aver.AreEqual("dxw", bucket);
      Aver.AreEqual("us-west-2", region);
      Aver.AreEqual(string.Empty, itemLocalPath);
    }

    [Run]
    public void ParseFileRoot()
    {
      string bucket;
      string region;
      string itemLocalPath;

      S3V4URLHelpers.Parse("https://dxw.s3-us-west-2.amazonaws.com/MyFile.txt", out bucket, out region, out itemLocalPath);

      Aver.AreEqual("dxw", bucket);
      Aver.AreEqual("us-west-2", region);
      Aver.AreEqual("MyFile.txt", itemLocalPath);
    }

    [Run]
    public void ParseFolderRoot()
    {
      string bucket;
      string region;
      string itemLocalPath;

      S3V4URLHelpers.Parse("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/", out bucket, out region, out itemLocalPath);

      Aver.AreEqual("dxw", bucket);
      Aver.AreEqual("us-west-2", region);
      Aver.AreEqual("MyFolder/", itemLocalPath);
    }

    [Run]
    public void ParseFolderFile()
    {
      string bucket;
      string region;
      string itemLocalPath;

      S3V4URLHelpers.Parse("https://it-adapter.s3-ap-southeast-1.amazonaws.com/MyFolder/MyFile1.txt", out bucket, out region, out itemLocalPath);

      Aver.AreEqual("it-adapter", bucket);
      Aver.AreEqual("ap-southeast-1", region);
      Aver.AreEqual("MyFolder/MyFile1.txt", itemLocalPath);
    }

    [Run]
    public void ParseFolderFileParameters()
    {
      string bucket;
      string region;
      string itemLocalPath;
      IDictionary<string, string> queryParams;

      S3V4URLHelpers.Parse("https://dxw.s3-us-west-2.amazonaws.com/MyFolder/MyFile1.txt?marker=1&max-keys=100",
        out bucket, out region, out itemLocalPath, out queryParams);

      Aver.AreEqual("dxw", bucket);
      Aver.AreEqual("us-west-2", region);
      Aver.AreEqual("MyFolder/MyFile1.txt", itemLocalPath);

      Aver.IsNotNull(queryParams);
      Aver.AreEqual(2, queryParams.Count);
      Aver.IsTrue(queryParams.ContainsKey("marker"));
      Aver.IsTrue(queryParams.ContainsKey("max-keys"));
      Aver.AreEqual("1", queryParams["marker"]);
      Aver.AreEqual("100", queryParams["max-keys"]);
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionInvalidUriEmpty()
    {
      parseException("");
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionInvalidUri()
    {
      parseException("aaa");
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionUriWOS3()
    {
      parseException("https://amazonaws.com");
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionUriWS4()
    {
      parseException("https://s4.amazonaws.com");
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionUriTooManyDomains()
    {
      parseException("https://a1.b2.s3.amazonaws.com");
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionUriInvalidRegion()
    {
      parseException("https://s3_blah.amazonaws.com");
    }

    [Run]
    [Aver.Throws(typeof(NFXException))]
    public void ParseExceptionUriNonHttp()
    {
      parseException("ftp://dxw.s3.amazonaws.com");
    }

    private void parseException(string uri)
    {
      string bucket;
      string region;
      string itemLocalPath;
      IDictionary<string, string> queryParams;

      S3V4URLHelpers.Parse(uri, out bucket, out region, out itemLocalPath, out queryParams);
    }
  }
}
