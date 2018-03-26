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
      ID = FID.Generate();
      Target = target;
    }

    protected override void Destructor()
    {
      base.Destructor();
    }

    internal readonly FID ID;
    internal readonly AspHttpContext Target;


    IHttpConnection IHttpContext.Connection => this;
    IHttpRequest    IHttpContext.Request    => this;
    IHttpResponse   IHttpContext.Response   => this;
    IPrincipal      IHttpContext.Principal  => Target.User;

    public bool IsAborted => Target.RequestAborted;

    public void Abort()
    {
      Target.Abort();
    }

    #region IHttpConnection
      string    IHttpConnection.ID          => ID.ToString();
      IPAddress IHttpConnection.RemoteIP    => Target.Request.RemoteEndPoint.Address;
      int       IHttpConnection.RemotePort  => Target.Request.RemoteEndPoint.Port;
      IPAddress IHttpConnection.LocalIP     => Target.Request.LocalEndPoint.Address;
      int       IHttpConnection.LocalPort   => Target.Request.LocalEndPoint.Port;


      X509Certificate2 IHttpConnection.GetClientCertificate() => Target.Request.GetClientCertificate();

      Task<X509Certificate2> IHttpConnection.GetClientCertificateAsync(CancellationToken cancellationToken = default(CancellationToken))
         => Target.Request.GetClientCertificateAsync();
    #endregion

    #region IHttpRequest

    #endregion

    #region IHttpResponse

    #endregion
  }
}
