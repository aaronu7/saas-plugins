# saas-plugins

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) [![Build Status](https://travis-ci.org/aaronu7/saas-plugins.svg?branch=master)](https://travis-ci.org/aaronu7/saas-plugins) [![NuGet](https://img.shields.io/nuget/v/MetaIS.SaaS.Plugins.svg)](https://www.nuget.org/packages/MetaIS.SaaS.Plugins/)

SaaS-Plugins is a C# project that deals with the runtime code compilation and management of plugin assemblies across multiple AppDomains while allowing for recompiles without restarting the controlling application.


## Features

 - Dynamically compiles C# code at runtime.
 - Dynamically loads/unloads C# assemblies at runtime.
 - Manages the complexity of interacting with remoted objects accross multiple AppDomains.
 - Manages the unloading/reloading of affected AppDomains when an assembly is recompiled. 

 
## Installation

```sh
PM> Install-Package MetaIS.SaaS.Plugins
```

## Usage

```cs
// The code for this plugin
string code = @"
	using System;
	namespace DynamicPlugins {
		public class CodeMirror {
			public int MirrorInt(int x) {return x;}
		}
	}";

// Core references are located in the bin folder and are not plugins
string[] coreRefs = new string[] {"System.dll", "MetaIS.SaaS.Plugins.dll"};

// Plugin references are DLL's that have been compiled and linked at runtime
string[] pluginRefs = null;

// Get the path to the core and plugin DLL's
string coreBinDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
string pluginDir = coreBinDir + @"/PluginsTest/";

// Call the plugin creation helper to create a plugin
Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", coreBinDir,
	pluginDir, "CodeMirror.dll", new string[] {code}, "DynamicPlugins.CodeMirror", coreRefs, pluginRefs, 1);
```


## License

This module is released under the permissive [MIT License](http://revolunet.mit-license.org). Contributions are always welcome.


## Contributors

Created by [@Aaron Ulrich](https://github.com/aaronu7)





### History
The saas-plugins module is a heavily refactored/refined version of one used in a earlier Model Driven Engineering (MDE) SaaS research project. This modules predecessor was the foundation for integrating model driven generated code and allowed entire inter-related systems to be dynamically compiled and linked in C# without unloading the controlling SaaS application.

### Examples of purpose
1.) This module can be extended to a Software as a Service (SaaS) model in which the plugins are licensed modules that extend a clients service which is running in a specific AppDomain. In such a model each client AppDomain is composed of various plugins which compose the clients specific service (ex. entities, workflows, triggers, security, etc). Although each plugin can be handcrafted, the true realization of this technique occurs by building the system model using code generation from meta-models (MDE/MDD/MDA).

2.) In its simplest form this module can offer plugins that allow significant application alterations without a project recompile (or an application restart) and with the full power of .NET available to each plugin.



### Primary Use Cases
- Generator/Developer adds a new plugin to system. The plugin compiles and becomes available to load into client AppDomains.
- Generator/Developer updates a plugin in system.  All AppDomains using the plugin unload, the plugin recompiles, the affected AppDomains reload.
- Generator/Developer loads a plugin into AppDomain. The assembly is loaded and becomes accessible in the AppDomain.
- Generator/Developer removes a plugin from system. NYI.
- Generator/Developer unloads a plugin from an AppDomain. NYI.


### Test Cases
This module was purposely designed to offer several layers of implementation:
- TestCase-Helper: The HelperPlugin class layer is a static class which can be used independently of the proposed implementation.
- TestCase-Domain: The PluginDomain layer offers a minimal implementation capable of managing plugins within a single domain.
- TestCase-System: The PluginSystem layer offers a solution that can manage loading/unloading plugins spanning multiple domains.
- Trivial interactive windows form: demonstrates loading and dynamic interaction with runtime compiled plugins.

### Class Diagram
![Alt text](readme-resources/PluginSystem.png?raw=true "Title")

### Project Dependencies
- Developed in VS2015 using the nunit package for unit testing.
- Download the "NUnit 3 Test Adapter" extension to view example tests in the Test Explorer
---- Install-Package NUnit -Version 3.6.1
---- Install-Package NUnit3TestAdapter -Version 3.7.0
---- System and System.Drawing were added and set to copy local (for better linking with plugins)

### Future Considerations
- Extend reflection mechanism to better interact with static class methods.
- Extend unit tests to verify reflection methods.
- Explore other trivial interfaces with minimal dependencies.
- Add a basic web service API example.
- Extend usage cases with more complex designs.
- Use the metadata from the xml generated during the compile process.
- Add a "plugin publisher" solution


### Screenshots of the trivial interactive interface:

![Alt text](readme-resources/ScreenShot.png?raw=true "Title")

![Alt text](readme-resources/ScreenShot2.png?raw=true "Title")
