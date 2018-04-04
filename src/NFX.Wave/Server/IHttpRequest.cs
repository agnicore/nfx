using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Provides access to the underlying listener http request
  /// </summary>
  public interface IHttpRequest
  {
    string Scheme { get; }
    string Method { get; }
    IUri   URI    { get; }

    IHttpHeaders  Headers { get; }
    IHttpQuery    Query   { get; }
    IHttpCookies  Cookies { get; }

    Stream Body           { get; }
    string ContentType    { get; }
    long?  ContentLength  { get; }

    /// <summary>
    /// Returns true if the form was requested
    /// </summary>
    bool IsFormContent{ get; }

    /// <summary>
    /// Returns requested form or null
    /// </summary>
    IHttpForm  Form{ get;}

    /// <summary>
    /// Gets request body as a form or null
    /// </summary>
    Task<IHttpForm>  GetFormAsync(CancellationToken cancellationToken = default(CancellationToken));
  }
}
