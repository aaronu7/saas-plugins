# saas-plugins

DRAFT

The saas-plugins module is a heavily refactored version of one used in a earlier Model Driven Engineering SaaS research project. This module is the foundation for model driven code generation and allows entire inter-related systems to be dynamically compiled and linked in C# without unloading the controlling appliction.

This module was purposly designed to offer several layers of implementation:
- TestCase-Helper: The HelperPlugin class layer is a static class which can be used independantly of the proposed implementation.
- TestCase-Domain: The PluginDomain layer offers a minimal implementation capable of managing plugins within a single domain.
- TestCase-System: The PluginSystem layer offers a solution that can manage loading/unloading plugins spanning multiple domains.


![Alt text](readme-resources/PluginSystem.png?raw=true =250x250)


Dependancies:
- Developed in VS2015 using the nunit package for unit testing.
- Download the "NUnit 3 Test Adapter" extension to view example tests in the Test Explorer
- Although a trivial application implements the module, examples of usage are best exemplified through the unit tests.


Usage Notes:
- Although a trivial windows form example is included, this module is meant to run in a Web Application/API.
