[![Licence Apache 2.0](https://img.shields.io/badge/license-apache_3-green.svg)](http://www.apache.org/licenses/LICENSE-2.0/) [![NuGet Version](http://img.shields.io/nuget/v/Qart.CyberTester.svg?style=flat)](https://www.nuget.org/packages/Qart.CyberTester/) [![Build status](http://img.shields.io/appveyor/ci/avao/Qart.svg?label=windows)](https://ci.appveyor.com/project/avao/Qart) [![Build status](http://img.shields.io/travis/avao/Qart.svg?label=linux)](https://travis-ci.org/avao/Qart)

# Qart

Qart is a generic framework designed for writing system and acceptance
tests in C#. A typical usage of Qart starts by extending it with a
domain specific _Processor_ for your system. You then generate one or
more _Test Cases_ according to the Qart format. These may also contain
data files required by your _Procesor_. Finally, you execute the tests
via the ```Qart.CyberTester``` command, which generates a _Test
Report_ detailing successes and failures.
