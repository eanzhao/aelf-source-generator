# ContractGenerator

## Introduction

This is the .NET library used by the protoc-plugin executable for the parsing & generation of C# aelf-contract code from
source proto files.


## Test

You'll need to recompile the protos into a bin file before you can run any Xunit tests. Hence you'll need to run this python-script:

```shell
python3 test/ContractGenerator.Tests/scripts/generate_descriptor.py
```
