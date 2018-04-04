using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Abstracts server context which provides Connection,Request,Response objects
  /// </summary>
  public interface IHttpContext : IDisposable
  {
    IHttpConnection   Connection { get; }
    IHttpRequest  Request    { get; }
    IHttpResponse Response   { get; }
    IPrincipal    Principal  { get; }

    bool IsAborted{ get;}

    /// <summary>
    /// Aborts the processing
    /// </summary>
    void Abort();
  }
}
