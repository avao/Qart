# Frequently Asked Questions

## How do I supply multiple parameters to CyberTester on a Unix shell?

When you run Qart from a Unix shell such as bash, you need to escape
the parameter separator, like so:

```
Qart.CyberTester.exe -o param_a=value_a\;param_b=value_b
```

If you do not escape the ```;```, the shell will interpret it as a
separate command and attempt to execute it.
