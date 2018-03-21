using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace NFX.Wave.Server
{
  interface IHttpContext
  {
    IHttpConnection   Connection { get; }
    IHttpRequest  Request    { get; }
    IHttpResponse Response   { get; }
    IPrincipal    Principal  { get; }

    bool IAborted{ get;}

    /// <summary>
    /// Aborts the connection
    /// </summary>
    void Abort();
  }
}
