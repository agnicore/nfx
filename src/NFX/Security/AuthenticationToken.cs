/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2018 Agnicore Inc. portions ITAdapter Corp. Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
</FILE_LICENSE>*/
using System;

namespace NFX.Security
{
    /// <summary>
    /// Represents security provider-internal ID that SecurityManager assigns into User object on authentication.
    /// These tokens can be used in place of Credentials to re-authenticate users or to re-query user rights (e.g. upon re/authorization).
    /// External parties should never be supplied with this struct as it is system backend internal token used inside the system
    /// </summary>
    [Serializable]
    public struct AuthenticationToken
    {
      public AuthenticationToken(string realm, object data)
      {
          m_Realm = realm;
          m_Data = data;
      }

      private string m_Realm;
      private object m_Data;

      /// <summary>
      /// Provides information about back-end security source (realm) that performed authentication, i.e. LDAP instance, Database name etc...
      /// </summary>
      public string Realm
      {
        get {return m_Realm;}
      }

      /// <summary>
      /// Provides provider-specific key/id that uniquely identifies the user in the realm
      /// </summary>
      public object Data
      {
        get { return m_Data; }
      }

      /// <summary>
      /// Returns true when the structure contains data
      /// </summary>
      public bool Assigned
      {
        get { return m_Realm.IsNotNullOrWhiteSpace() || m_Data!=null;}
      }

      public override string ToString()
      {
        return "AuthToken({0}::{1})".Args(m_Realm, m_Data);
      }
    }
}
