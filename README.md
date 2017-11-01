# saas-plugins

The saas-plugins module is a heavily refactored version of one used in a earlier Model Driven Engineering SaaS research project. This module is the foundation for executing model driven generated code and allows entire inter-related systems to be dynamically compiled and linked in C# without unloading the controlling application. To achieve this, the design implements a method for loading and managing plugins in seperate AppDomains which can be easily unloaded to free memory allocations and file locks on target plugin DLL's.

This module was purposely designed to offer several layers of implementation:
- TestCase-Helper: The HelperPlugin class layer is a static class which can be used independently of the proposed implementation.
- TestCase-Domain: The PluginDomain layer offers a minimal implementation capable of managing plugins within a single domain.
- TestCase-System: The PluginSystem layer offers a solution that can manage loading/unloading plugins spanning multiple domains.
- Although a trivial interactive windows form example is included, this module is meant to run in a Web Application/API.

![Alt text](readme-resources/PluginSystem.png?raw=true "Title")


Dependencies:
- Developed in VS2015 using the nunit package for unit testing.
- Download the "NUnit 3 Test Adapter" extension to view example tests in the Test Explorer

Future Considerations:
- Extend reflection mechanism to better interact with static class methods.
- Extend unit tests to verify reflection methods.
- Explore other trivial interfaces with minimal dependancies.
- Add a basic web service API example.
- Extend usage cases with more complex designs.
- Use the metadata from the xml generated during the compile process.
- Add a "plugin publisher" solution


Screenshots of the trivial interactive interface:

![Alt text](readme-resources/ScreenShot.png?raw=true "Title")

![Alt text](readme-resources/ScreenShot2.png?raw=true "Title")