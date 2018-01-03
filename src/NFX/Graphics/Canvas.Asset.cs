using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  public sealed partial class Canvas : DisposableObject
  {
    /// <summary> All Canvas objects derived from this one </summary>
    public abstract class Asset : DisposableObject, IPALCanvasAsset
    {
      protected abstract IPALCanvasAsset AssetHandle { get; }
    }

    /// <summary> All Canvas objects derived from this one </summary>
    public abstract class Asset<THandle> : Asset where THandle : class, IPALCanvasAsset
    {
      protected Asset(THandle handle)
      {
        m_Handle = handle;
      }

      protected override void Destructor()
      {
        base.Destructor();
        DisposeAndNull(ref m_Handle);
      }

      private THandle m_Handle;

      protected THandle Handle { get => m_Handle; }
      protected override IPALCanvasAsset AssetHandle { get => m_Handle; }
    }


  }
}
