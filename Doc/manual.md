# Manual

# Overview

A _Test System_ represents the system which one wishes to test. One or
more _Test Cases_ are defined for this system and they are executed
within a _Test Session_. Test Cases produce _Test Case Results_, which
are gathered by the Test Session. A _Test Case Processor_ is
responsible for running the Test Cases.

A typical usage of Qart starts by extending it with one or more _Test
Case Processors_ for a given Test System. These are domain
specific. You can then generate one or more _Test Cases_ according to
the Qart specification and the requirements of the Test Case Processor
you defined. Finally, tests are executed via the command-line tool
```Qart.CyberTester```, which generates a _Test Report_ with the
results of test execution.

# Design

As inferred from the introduction, System Tests are split into three
different aspects:

- the generic infrastructure required for testing, which Qart
  provides;
- the domains specific infrastructure which the user must write;
- the test cases themselves.

## Generic Infrastructure

Qart provides different kinds of services for its consumers:
management of Test Sessions, access to data and assertion support. But
before we can make sense of this we need to understand how tests are
defined.

### Test Structure

Qart attempts to be as flexible as possible in what it allows as a
structure for tests. In fact, there is only one core requirement to
define a Test Case: it must be housed in a directory containing a file
with the name ```.test```. This file is called a _Test Definition
File_. As a corollary, each Test Case must be housed in its own
directory.

Test Cases are executed in the context of a _Test Session_.  The
process of determining which tests to run on a Test Session is called
_Test Case Discovery_ and it works as follows: starting from a
top-level directory, we check it and all of its child-directories for
Test Definition Files; each directory with a Test Definition File is
added to the current session as a Test Case for execution.

Users are thus free to choose any directory structure they'd like for
their tests, and often this structure evolves over time as more tests
are added. Ideally one should choose to place tests that are often
executed together in the same directory. Also, it is possible - though
not wise - to have a Test Definition File in a directory which itself
has sub-directories. However, for clarity, it's better to have only
leaf directories as Test Cases. The choice of test structure becomes
crucial once your tests grow from a few to several hundreds, and
usability will be drastically affected by it.

As an example, a possible directory structure could be:

```
/ my_system_tests
    / test_suite_1
        / test_1
            .test
...
        / test_n
            .test
    / test_suite_2
...
```

However, bear in mind that this structure is presented here just for
illustrative purposes; all of these directory names are defined by the
user, and have no meaning at all to Qart.

#### Test Cases

Each Test Case has a unique identifier called the _Test Case ID_. It
is made up of the full path to directory housing the Test Definition
File. Using the example above, the Test Case ID for ```test_1``` would
be ```my_system_tests/test_suite_1/test_1```. By virtue of the
properties of the filesystem, Test Case IDs are unique.

The Test Definition File has a well-defined format, described here in
EBNF:

```
test_case_processor_id[ payload]
```

```test_case_processor_id``` can be a string of any length, but it
must be made up of letters (lower or upper case), digits or
underscores. When the optional ```payload``` is present, the two
fields must be separated by a space. Qart reads the
```test_case_processor_id``` and uses it to locate a _Test Case
Processor_ - which we will cover in detail later on. If found, the
processor is then used to execute the test. If there are no
registrations against that ID, the test will fail to execute.

The second part of the Test Definition File is the ```payload```. The
```payload``` is encoded in JSON and it may span any number of
lines. The contents of the ```payload``` are arguments for the Test
Case Processor and are really only meaningful to it. However, in order
to simplify the Test Case Processor, Qart pre-processes the JSON into
a more C#-idiomatic representation.

#### Test Case Processor

Out of the box, Qart will not do very much. This is because there are
no Test Case Processors built-in. They require knowledge of the system
you are trying to test.







Test Case Processors are injected use the Qart testing framework

#### Test Execution

Qart provides the command line utility ```Qart.CyberTester.exe``` to
execute tests. It has the following command line arguments:

| Short | Long       | Description                                                     |
|-------|------------|-----------------------------------------------------------------|
| -d    | dir        | Directory with test cases. Accepts multiple directories         |
| -r    | rebaseline | Rebaselines the test data. This is a processor specific command |
| -o    | options    | Processor specific variables as kvp, separated by ```;````.     |
| -h    | help       | Provides the usage documentation                                |
