using System;
using System.IO;
using System.Drawing;

using NFX.Graphics;

namespace NFX.PAL.Graphics
{

  public interface IPALCanvasAsset : IDisposable { }

  public interface IPALCanvasBrush : IPALCanvasAsset { }
  public interface IPALCanvasPen   : IPALCanvasAsset { }
  public interface IPALCanvasFont  : IPALCanvasAsset { }

  public interface IPALCanvas : IDisposable
  {
    InterpolationMode Interpolation{ get; set;}
  }
}
