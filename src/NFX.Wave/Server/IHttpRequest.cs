using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Provides access to the underlying listener http request
  /// </summary>
  public interface IHttpRequest
  {
    string Scheme     { get; }
    string Method     { get; }

    IHttpHeaders  Headers { get;}
    IHttpQuery    Query { get;}
    IHttpCookies  Cookies { get; }

    Stream Body { get; }
    string ContentType { get; }
    long?  ContentLength { get; }
  }
}
