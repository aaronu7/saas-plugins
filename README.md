# saas-plugins

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) [![Build Status](https://travis-ci.org/aaronu7/saas-plugins.svg?branch=master)](https://travis-ci.org/aaronu7/saas-plugins) [![NuGet](https://img.shields.io/nuget/v/MetaIS.SaaS.Plugins.svg)](https://www.nuget.org/packages/MetaIS.SaaS.Plugins/)

SaaS-Plugins is a C# project that deals with the runtime code compilation and management of plugin assemblies across multiple AppDomains while allowing for recompiles without restarting the controlling application.


## Features

 - Dynamically compiles C# code at runtime.
 - Dynamically loads/unloads C# assemblies at runtime.
 - Manages the complexity of interacting with remoted objects accross multiple AppDomains.
 - Manages the unloading/reloading of affected AppDomains when an assembly is recompiled. 


### Purpose
1.) This module can be extended to a Software as a Service (SaaS) model in which the plugins are licensed modules that extend a clients service which is running in a specific AppDomain. In such a model each client AppDomain is composed of various plugins which compose the clients specific service (ex. entities, workflows, triggers, security, etc). Although each plugin can be handcrafted, the true realization of this technique occurs by building the system model using code generation from meta-models (MDE/MDD/MDA).

2.) In its simplest form this module can offer plugins that allow significant application alterations without a project recompile (or an application restart) and with the full power of .NET available to each plugin.



## Installation

```sh
PM> Install-Package MetaIS.SaaS.Plugins
```

## Usage

### Basic Initialization: create a plugin and add it to a domain.
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
string pluginSubPath = @"/PluginsTest/";
string pluginDir = coreBinDir + pluginSubPath;

// Call the plugin creation helper to create a plugin
Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror.dll", coreBinDir, pluginDir, 
	new string[] {code}, "DynamicPlugins.CodeMirror", coreRefs, pluginRefs, 1);

List<Plugin> pluginSet = new List<Plugin>(){plugin};

// Load the plugins into the system
PluginSystem pluginSystem = new PluginSystem("AppDomain1", coreBinDir, pluginSubPath);            
pluginSystem.PluginSystemLoad(pluginSet);

// Load the plugin into a domain
pluginSystem.PluginDomainLoad("AppDomain1", new List<string>() { pluginSet[0].PluginID });
```

### Invoke method: building on the above we can invoke a method in the dynamically created assembly.
```cs
object objA = pluginSystem.InvokeMethod("AppDomain1", plugin.PluginID, plugin.ClassNamespacePath, 
	"MirrorInt", new object[] {7});
string sA = HelperPlugin.ObjectToString(objA);
System.Console.WriteLine(sA);

// NUnit Assert
//Assert.NotNull(objA);
//Assert.AreEqual("7", sA, "Expected: 7  but got: " + sA);
```

### Re-compile plugin: building on the above we can alter the code, update the system, and run the invoke
```cs
// Alter the code
plugin.Code = new string[] {
@"using System;
	namespace DynamicPlugins {
		public class CodeMirror {
			public int MirrorInt(int x) {return x*2;}
		}
}" };

// Trigger a re-compile (unloads then reloads all affected domains)
List<string> pluginIdSet = new List<string>() {plugin.PluginID};
pluginSystem.SystemUpdate(pluginIdSet);

// Invoke method and verify 
object objB = pluginSystem.InvokeMethod("AppDomain1", pluginSet[0].PluginID, pluginSet[0].ClassNamespacePath, 
	"MirrorInt", new object[] {7});
string sB = HelperPlugin.ObjectToString(objB);
System.Console.WriteLine(sB);

// NUnit Assert
//Assert.NotNull(objB);
//Assert.AreEqual("14", sB, "Expected: 14  but got: " + sB);
```

## Developer Environment & Notes
- Developed in VS2015 using the nunit package for unit testing.
- System and System.Drawing were added and set to copy local for better linking with plugins.
- The following nunit versions were found to work best with Travis CI integration.
```sh
PM> Install-Package NUnit -Version 3.6.1
PM> Install-Package NUnit3TestAdapter -Version 3.7.0
```

##### Class Diagram
![Alt text](readme-resources/PluginSystem.png?raw=true "Title")

##### Primary Use Cases
- Generator/Developer adds a new plugin to system. The plugin compiles and becomes available to load into client AppDomains.
- Generator/Developer updates a plugin in system.  All AppDomains using the plugin unload, the plugin recompiles, the affected AppDomains reload.
- Generator/Developer loads a plugin into AppDomain. The assembly is loaded and becomes accessible in the AppDomain.
- Generator/Developer removes a plugin from system. NYI.
- Generator/Developer unloads a plugin from an AppDomain. NYI.

##### Test Cases
This module was purposely designed to offer several layers of implementation:
- TestCase-Helper: The HelperPlugin class layer is a static class which can be used independently of the proposed implementation.
- TestCase-Domain: The PluginDomain layer offers a minimal implementation capable of managing plugins within a single domain.
- TestCase-System: The PluginSystem layer offers a solution that can manage loading/unloading plugins spanning multiple domains.
- Trivial interactive windows form: demonstrates loading and dynamic interaction with runtime compiled plugins.

##### Future Considerations
- Extend PluginSystem to load from a data model (link to CSV project).
- Extend unit tests to verify reflection methods.
- Extend use cases with more complex designs.
- Use the metadata from the xml generated during the compile process to annotate data in the plugin.

##### Module History
The saas-plugins module is a heavily refactored/refined version of one used in a earlier Model Driven Engineering (MDE) SaaS research project. This modules predecessor was the foundation for integrating model driven generated code and allowed entire inter-related systems to be dynamically compiled and linked in C# without unloading the controlling SaaS application.

## License

This module is released under the permissive [MIT License](http://revolunet.mit-license.org). Contributions are always welcome.


## Contributors

Created by [@Aaron Ulrich](https://github.com/aaronu7)












