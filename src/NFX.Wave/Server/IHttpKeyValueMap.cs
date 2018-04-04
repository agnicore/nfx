using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Represents a key:value map
  /// </summary>
  public interface IHttpKeyValueMap : IEnumerable<KeyValuePair<string, string>>
  {
    /// <summary>
    /// True if key case is respected, for example in query parameters, whereas headers are NOT case sensitive by the spec
    /// </summary>
    bool IsCaseSensitive { get; }

    /// <summary>
    /// Returns the count of items in the map
    /// </summary>
    int Count { get; }

    /// <summary>
    /// True if the map contains the key
    /// </summary>
    bool Contains(string key);

    /// <summary>
    /// If true then map can be changed, for example in the response context
    /// </summary>
    bool IsMutable{ get; }

    /// <summary>
    /// Returns an item by key or null if not found. Throws on set if immutable
    /// </summary>
    string this[string key]{ get; set;}

    /// <summary>
    /// Returns an item at the specified index or null if index is out of bounds. Throws on set if immutable
    /// </summary>
    string this[int idx]{ get; set;}

    /// <summary>
    /// Sets the keys value returning true if it was added (vs replaced existing). Throws if immutable
    /// </summary>
    bool Set(string key, string value);

    /// <summary>
    /// Tries to delete an item by key and returns true if removed. Throws if immutable
    /// </summary>
    bool Remove(string key);
  }

  /// <summary>
  /// Represents headers
  /// </summary>
  public interface IHttpHeaders : IHttpKeyValueMap
  {

  }

  /// <summary>
  /// Represents query string
  /// </summary>
  public interface IHttpQuery : IHttpKeyValueMap
  {
    /// <summary>
    /// Returns full query string
    /// </summary>
    string FullString { get;}
  }

  /// <summary>
  /// Represents cookies
  /// </summary>
  public interface IHttpCookies : IHttpKeyValueMap
  {

  }

}
