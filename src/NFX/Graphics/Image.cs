using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace NFX.Graphics
{
  /// <summary>
  /// Represents a 2d graphical in-memory Image
  /// </summary>
  public sealed class Image : DisposableObject
  {
    public static Image FromFile(string fileName)
    {
      return new Image(0,0){};
    }

    public static Image FromBytes(byte[] image)
    {
      return new Image(0,0){};
    }

    public static Image FromStream(Stream stream)
    {
      return new Image(0,0){};
    }


    public Image(int width, int height)
    {

    }

    protected override void Destructor()
    {
      base.Destructor();
      //destroy handle
    }



    public int Width
    {
     get{ return 0;}
    }

    public int Height
    {
     get{ return 0;}
    }

    public int HResolution{ get => 0;}

    public int VResolution{ get => 0;}


    public void SetResolution(int hDpi, int vDpi)
    {

    }

    public void MakeTransparent()
    {

    }


    //Lock bits

    public Color GetPixel(int x, int y)
    {
      return Color.White;
    }

    public void SetPixel(int x, int y, Color color)
    {
    }

    public void Save(Stream stream, ImageFormat format)
    {

    }



  }
}
