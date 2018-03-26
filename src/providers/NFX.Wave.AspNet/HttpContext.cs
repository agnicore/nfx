using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AspHttpContext = Microsoft.AspNetCore.Http.HttpContext;

using NFX.Wave.Server;

namespace NFX.Wave.AspNet
{
  /// <summary>
  /// Provides IHttpContext implementation based on AspNet middleware context
  /// </summary>
  public sealed class HttpContext : DisposableObject, IHttpContext, IHttpConnection, IHttpRequest, IHttpResponse
  {

    internal HttpContext(AspHttpContext target)
    {
      Target = target;
    }

    protected override void Destructor()
    {
      base.Destructor();
    }

    internal readonly AspHttpContext Target;


    IHttpConnection IHttpContext.Connection => this;
    IHttpRequest    IHttpContext.Request    => this;
    IHttpResponse   IHttpContext.Response   => this;
    IPrincipal      IHttpContext.Principal  => Target.User;

    public bool IsAborted => Target.RequestAborted.IsCancellationRequested;

    public void Abort()
    {
      Target.Abort();
    }

    #region IHttpConnection
      string    IHttpConnection.ID          => Target.Connection.Id;
      IPAddress IHttpConnection.RemoteIP    => Target.Connection.RemoteIpAddress;
      int       IHttpConnection.RemotePort  => Target.Connection.RemotePort;
      IPAddress IHttpConnection.LocalIP     => Target.Connection.LocalIpAddress;
      int       IHttpConnection.LocalPort   => Target.Connection.LocalPort;


      X509Certificate2 IHttpConnection.GetClientCertificate() => Target.Connection.ClientCertificate;

      Task<X509Certificate2> IHttpConnection.GetClientCertificateAsync(CancellationToken cancellationToken)
         => Target.Connection.GetClientCertificateAsync(cancellationToken);
    #endregion

    #region IHttpRequest

    #endregion

    #region IHttpResponse

    #endregion
  }
}
