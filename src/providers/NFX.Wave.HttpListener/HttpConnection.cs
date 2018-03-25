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
  public struct HttpConnection : IHttpConnection
  {
    public string ID => throw new NotImplementedException();

    public IPAddress RemoteIP => throw new NotImplementedException();

    public int RemotePort => throw new NotImplementedException();

    public IPAddress LocalIP => throw new NotImplementedException();

    public int LocalPort => throw new NotImplementedException();

    public X509Certificate GetClientCertificate()
    {
      throw new NotImplementedException();
    }

    public Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      throw new NotImplementedException();
    }
  }
}
