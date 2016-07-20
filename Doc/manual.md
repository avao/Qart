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

Qart uses the filesystem to define tests; any directory can be used to
house a test case, provided a ```.test``` file is present. This means
that users are free to organise the directory structure as they like,
provided the leaf directories have the expected ```.test``` file. For
example, a possible structure could be:

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

Its important to bear in mind that this structure is presented here
just as an example; all of these directory names are defined by the
user, and have no meaning at all to Qart. The full path to the test
directory is treated as a test ID -
i.e. ```my_system_tests/test_suite_1/test_1```.

#### ```.test``` Files

Once Qart has located a ```.test``` file, it expects it to have the
following format (described in EBNF):

```
PROCESSOR_ID[ PAYLOAD]
```

```PROCESSOR_ID``` can be a string of any length, but it must be made
up of letters (lower or upper case), digits or underscores. When the
optional ```PAYLOAD``` is present, the two fields must be separated by
a space. Qart reads the ```PROCESSOR_ID``` and uses it to locate a
Test Case Processor for it. If found, it is then used to execute the
test. If there are no registrations against that processor ID, the
test will fail.

The second part of the ```.test``` file is the ```PAYLOAD```. At
present it is expected to be in JSON and it may span any number of
lines. Although Qart performs some basic pre-processing on the JSON,
the contents of the ```PAYLOAD``` are really only meaningful to the
Test Case Processor. The ```PAYLOAD``` can be thought of as the
arguments for the Test Case Processor.
