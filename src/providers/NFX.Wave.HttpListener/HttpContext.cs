using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;

using NFX.Wave.Server;

namespace NFX.Wave.HttpListener
{
  public sealed class HttpContext : DisposableObject, IHttpContext
  {

    internal HttpContext(HttpListenerContext target)
    {
      Target = target;
      m_Connection = new HttpConnection(this);
      m_Request    = new HttpRequest   (this);
      m_Response   = new HttpResponse  (this);
    }

    protected override void Destructor()
    {
      base.Destructor();
    }

    internal readonly HttpListenerContext Target;


    private HttpConnection m_Connection;
    private HttpRequest    m_Request;
    private HttpResponse   m_Response;

    public IHttpConnection Connection => m_Connection;
    public IHttpRequest    Request    => m_Request;
    public IHttpResponse   Response   => m_Response;
    public IPrincipal      Principal  => Target.User;

    public bool IsAborted => throw new NotImplementedException();

    public void Abort()
    {
      Target.Response.Abort();
    }
  }
}
