# ContractGenerator

## Introduction

This is the .NET library used by the protoc-plugin executable for the parsing & generation of C# aelf-contract code from
source proto files.


## Test

For contract generation unit testing, we need to compile proto files and generate a bin file before you can run any Xunit tests.
This file will be used as a dependency for running unit tests.

Follow the steps below. Firstly, we compile the protos into a bin file using the following python command.

```shell
python3 test/ContractGenerator.Tests/scripts/generate_descriptor.py
```

Then you will see the descriptor.bin file under the folder of proto.

Next, run the unit tests directly, and we can see the testing results.
