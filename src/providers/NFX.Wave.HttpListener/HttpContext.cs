using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NFX.Wave.Server;

namespace NFX.Wave.HttpListener
{
  public sealed class HttpContext : DisposableObject, IHttpContext, IHttpConnection, IHttpRequest, IHttpResponse
  {

    internal HttpContext(HttpListenerContext target)
    {
      Target = target;
    }

    protected override void Destructor()
    {
      base.Destructor();
    }

    internal readonly HttpListenerContext Target;


    IHttpConnection IHttpContext.Connection => this;
    IHttpRequest    IHttpContext.Request    => this;
    IHttpResponse   IHttpContext.Response   => this;
    IPrincipal      IHttpContext.Principal  => Target.User;

    public bool IsAborted => throw new NotImplementedException();

    public void Abort()
    {
      Target.Response.Abort();
    }


    string IHttpConnection.ID             => throw new NotImplementedException();
    IPAddress IHttpConnection.RemoteIP    => Target.Request.RemoteEndPoint.Address;
    int       IHttpConnection.RemotePort  => Target.Request.RemoteEndPoint.Port;

    IPAddress IHttpConnection.LocalIP     => Target.Request.LocalEndPoint.Address;
    int       IHttpConnection.LocalPort   => Target.Request.LocalEndPoint.Port;

    X509Certificate2 IHttpConnection.GetClientCertificate() => Target.Request.GetClientCertificate();

    Task<X509Certificate2> IHttpConnection.GetClientCertificateAsync(CancellationToken cancellationToken = default(CancellationToken))
       => Target.Request.GetClientCertificateAsync();

  }
}
