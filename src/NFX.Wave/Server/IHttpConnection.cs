using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Provides connection information such as IP endpoints
  /// </summary>
  public interface IHttpConnection
  {
    string ID          { get; }
    IPAddress RemoteIP { get; }
    int RemotePort     { get; }
    IPAddress LocalIP  { get; }
    int LocalPort      { get; }
    bool IsKeepAlive   { get; }
    bool IsHttps       { get; }
    bool IsLocal       { get; }


    X509Certificate2 GetClientCertificate();
    Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = default(CancellationToken));
  }
}
