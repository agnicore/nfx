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


/* NFX by Agnicore
 * Originated: 2017.11
 */
using System;
using System.Runtime.Serialization;

namespace NFX.PAL
{
  /// <summary>
  /// Base exception thrown for the PAL-related issues
  /// </summary>
  [Serializable]
  public class PALException : NFXException
  {
    public PALException() {}
    public PALException(string message) : base(message) {}
    public PALException(string message, Exception inner) : base(message, inner) {}
    protected PALException(SerializationInfo info, StreamingContext context) : base(info, context) {}
  }

}