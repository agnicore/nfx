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
using System.Threading;

using NFX.Environment;

namespace NFX.ApplicationModel
{
  /// <summary>
  /// Provides base implementation of IApplication for applications that have no forms like services and console apps. This class IS thread safe
  /// </summary>
  public class ServiceBaseApplication : CommonApplicationLogic
  {
    /// <summary>
    /// Takes optional application args[] and root configuration.
    /// The args are parsed into CommandArgsConfiguration. If configuration is null then
    /// application is configured from a file co-located with entry-point assembly and
    ///  called the same name as assembly with '.config' extension, unless args are specified and "/config file"
    ///   switch is used in which case 'file' has to be locatable and readable.
    /// </summary>
    public ServiceBaseApplication(string[] args, ConfigSectionNode rootConfig)
      : this(false, args, rootConfig)
    {}

    /// <summary>
    /// Takes optional application args[] and root configuration.
    /// The args are parsed into CommandArgsConfiguration. If configuration is null then
    /// application is configured from a file co-located with entry-point assembly and
    ///  called the same name as assembly with '.config' extension, unless args are specified and "/config file"
    ///   switch is used in which case 'file' has to be locatable and readable.
    /// Pass allowNesting=true to nest other app container instances
    /// </summary>
    public ServiceBaseApplication(bool allowNesting, string[] args, ConfigSectionNode rootConfig)
      : this(allowNesting, args == null ? null : new CommandArgsConfiguration(args), rootConfig)
    {}

    /// <summary>
    /// Takes optional command-line configuration args and root configuration. If configuration is null then
    ///  application is configured from a file co-located with entry-point assembly and
    ///   called the same name as assembly with '.config' extension, unless args are specified and "/config file"
    ///   switch is used in which case 'file' has to be locatable and readable.
    /// Pass allowNesting=true to nest other app container instances
    /// </summary>
    public ServiceBaseApplication(bool allowNesting, Configuration cmdLineArgs, ConfigSectionNode rootConfig) : base(allowNesting, cmdLineArgs, rootConfig)
    {
      try
      {
        InitApplication();
      }
      catch
      {
        Destructor();
        throw;
      }
    }

    protected override void Destructor()
    {
       CleanupApplication();
       base.Destructor();
    }
  }

}
