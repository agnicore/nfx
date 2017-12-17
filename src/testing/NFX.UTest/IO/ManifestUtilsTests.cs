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
using System.IO;

using NFX.Scripting;

using NFX.IO.FileSystem;
using NFX.IO.FileSystem.Local;
using NFX.IO.FileSystem.Packaging;
using NFX.Security;

namespace NFX.UTest.IO
{
    [Runnable]  
    public class ManifestUtilsTests
    {
      const string EXPECTED =   @"
package
{ 
  dir
  {
    name=SubDir1 
    file
    {
      name=Bitmap1.bmp
      size=500066
      csum=1248671023 
    }
    file
    {
      name='Some Text File With Spaces.txt'
      size=105
      csum=1399856254 
    }
  }
  dir
  {
    name=SubDir2 
    dir
    {
      name=a 
      file
      {
        name=Icon1.ico
        size=10134
        csum=2741792907 
      }
    }
    dir
    {
      name=b 
      file
      {
        name=About.txt
        size=76
        csum=3632532468 
      }
    }
  }
  file
  {
    name=Gagarin.txt
    size=4596
    csum=788225902 
  }
  file
  {
    name=TextFile1.txt
    size=21
    csum=1846610132 
  }
}
        ";
        
        
        public static string Get_TEZT_PATH()
        {
          return Path.Combine(System.Environment.GetEnvironmentVariable("AGNICORE_HOME"), "NFX", "Output","Debug","UTEZT_DATA");
        }

        [Run]
        public void Generate_1()
        {
            using(var fs = new LocalFileSystem("L1"))
            {
              var session = fs.StartSession();
              var dir = session[Get_TEZT_PATH()] as FileSystemDirectory; 
              var manifest = ManifestUtils.GeneratePackagingManifest(dir);

              Console.WriteLine(manifest.Configuration.ContentView);

              Aver.AreEqual("SubDir1", manifest[0].AttrByName(ManifestUtils.CONFIG_NAME_ATTR).Value);
              Aver.AreEqual("SubDir2", manifest[1].AttrByName(ManifestUtils.CONFIG_NAME_ATTR).Value);
              Aver.AreEqual("Gagarin.txt", manifest[2].AttrByName(ManifestUtils.CONFIG_NAME_ATTR).Value);
              Aver.AreEqual("TextFile1.txt", manifest[3].AttrByName(ManifestUtils.CONFIG_NAME_ATTR).Value);

               Aver.AreEqual(500066, manifest[0].Children.First(c=>c.IsSameNameAttr("Bitmap1.bmp")).AttrByName(ManifestUtils.CONFIG_SIZE_ATTR).ValueAsInt());
               Aver.AreEqual(1248671023, manifest[0].Children.First(c=>c.IsSameNameAttr("Bitmap1.bmp")).AttrByName(ManifestUtils.CONFIG_CSUM_ATTR).ValueAsInt());

               Aver.AreEqual(105, manifest[0].Children.First(c=>c.IsSameNameAttr("Some Text File With Spaces.txt")).AttrByName(ManifestUtils.CONFIG_SIZE_ATTR).ValueAsInt());
               Aver.AreEqual(1399856254, manifest[0].Children.First(c=>c.IsSameNameAttr("Some Text File With Spaces.txt")).AttrByName(ManifestUtils.CONFIG_CSUM_ATTR).ValueAsInt());
            }   
        }

        [Run]
        public void Compare_1()
        {
            using(var fs = new LocalFileSystem("L1"))
            {
              var session = fs.StartSession();
              var dir = session[Get_TEZT_PATH()] as FileSystemDirectory; 
              var manifest1 = ManifestUtils.GeneratePackagingManifest(dir);
              var manifest2 = ManifestUtils.GeneratePackagingManifest(dir);

              Aver.IsTrue( manifest1.HasTheSameContent(manifest2) );
            }   
        }

        [Run]
        public void Compare_2()
        {
            using(var fs = new LocalFileSystem("L1"))
            {
              var session = fs.StartSession();
              var dir = session[Get_TEZT_PATH()] as FileSystemDirectory; 
              var manifest1 = ManifestUtils.GeneratePackagingManifest(dir);
              var manifest2 = NFX.Environment.LaconicConfiguration.CreateFromString(EXPECTED).Root;

              Aver.IsTrue( manifest1.HasTheSameContent(manifest2) );
            }   
        }



        [Run]
        public void Compare_3()
        {
            using(var fs = new LocalFileSystem("L1"))
            {
              var session = fs.StartSession();
              var dir = session[Get_TEZT_PATH()] as FileSystemDirectory; 
              var manifest1 = ManifestUtils.GeneratePackagingManifest(dir);
              var manifest2 = ManifestUtils.GeneratePackagingManifest(dir);

              manifest2[0].Children.First(c=>c.IsSameNameAttr("Some Text File With Spaces.txt")).AttrByName(ManifestUtils.CONFIG_SIZE_ATTR).Value = "123";

              Aver.IsFalse( manifest1.HasTheSameContent(manifest2) );
            }   
        }

        [Run]
        public void Compare_4()
        {
            using(var fs = new LocalFileSystem("L1"))
            {
              var session = fs.StartSession();
              var dir = session[Get_TEZT_PATH()] as FileSystemDirectory; 
              var manifest1 = ManifestUtils.GeneratePackagingManifest(dir);
              var manifest2 = ManifestUtils.GeneratePackagingManifest(dir);

              manifest2[2].Delete();

              Aver.IsFalse( manifest1.HasTheSameContent(manifest2) );
            }   
        }



    }
}
