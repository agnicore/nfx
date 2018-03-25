using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace NFX.Wave.Server
{
  public interface IHttpContext : IDisposable
  {
    IHttpConnection   Connection { get; }
    IHttpRequest  Request    { get; }
    IHttpResponse Response   { get; }
    IPrincipal    Principal  { get; }

    bool IsAborted{ get;}

    /// <summary>
    /// Aborts the connection
    /// </summary>
    void Abort();
  }
}
