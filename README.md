[![Licence Apache 2.0](https://img.shields.io/badge/license-apache_3-green.svg)](http://www.apache.org/licenses/LICENSE-2.0/) [![NuGet Version](http://img.shields.io/nuget/v/Qart.CyberTester.svg?style=flat)](https://www.nuget.org/packages/Qart.CyberTester/) [![Build status](http://img.shields.io/appveyor/ci/avao/Qart.svg?label=windows)](https://ci.appveyor.com/project/avao/Qart) [![Build status](http://img.shields.io/travis/avao/Qart.svg?label=linux)](https://travis-ci.org/avao/Qart)

# Qart

Qart is a generic framework for System and Acceptance Testing [1]. In
order to write tests for your system, you need to augment Qart with
the domain specific logic; Qart then manages testing execution and
reporting for you, called from within a CI system and/or locally from
a developer's machine. Qart also provides basic infrastructure to help
writing the domain specific extensions.

You can install Qart from NuGet.

```
Install-Package Qart.CyberTester
```

For more details on Qart see the
[manual](https://github.com/avao/Qart/blob/master/Doc/manual.md). If
you'd like to submit a pull request or raise an issue, please read the
document on
[contributing](https://github.com/avao/Qart/blob/master/Doc/CONTRIBUTING.md).

[1] https://en.wikipedia.org/wiki/Acceptance_testing
