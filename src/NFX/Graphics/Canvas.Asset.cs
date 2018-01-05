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
      public abstract IPALCanvasAsset AssetHandle { get; }
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

      public THandle Handle { get { EnsureObjectNotDisposed();  return m_Handle;} }
      public override IPALCanvasAsset AssetHandle => Handle;
    }


  }
}
