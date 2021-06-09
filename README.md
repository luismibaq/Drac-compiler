# Drac compiler, version 0.5

This program is free software. You may redistribute it under the terms of the GNU General Public License version 3 or later. See `license.txt` for details.

Included in this release:

* Lexical analysis
* Syntactic analysis
* AST construction
* Semantic analysis
* Wat code generation

## Requirements

Additionally to the Mono C# software, make sure your system has the following installed:

* Python 3
* The Wasmer Python package

### Python 3

You’ll need Python version 3.6 or newer. To check that you have the correct version of Python, at the terminal type:

    python -V

**NOTE:** You might need to use the command `python3` instead.

The output should be something like this:

    Python 3.9.2

Go to the [Python website](https://www.python.org/downloads/) if you don’t have Python installed or if it’s older than 3.6.

### Wasmer

The [Wasmer Python package](https://github.com/wasmerio/wasmer-python) brings the required API to execute WebAssembly modules from within a Python runtime system.

To install Wasmer, type at the terminal the following two commands:

    pip install wasmer==1.0.0
    pip install wasmer_compiler_cranelift==1.0.0

**NOTE:** If in the previous section you had to use the `python3` command, then you should use `pip3` here as well. Also, you might need to prepend `sudo` to run the command with admin privileges.

## How to Build

At the terminal type:

    make

## How to Run

At the terminal type:

    mono drac.exe <drac_source_file>

Where `<drac_source_file>` is the name of a Buttercup source file. You can try with these files:

* `001_hello.drac`
* `002_binary.drac`
* `003_palindrome.drac`
* `004_factorial.drac`
* `005_arrays.drac`
* `006_next_day.drac`
* `007_literals.drac`
* `008_vars.drac`
* `009_operators.drac`
* `010_breaks.drac`

To execute the resulting Wat files, type:

    ./execute.py <wat_file>

Where `<wat_file>` is a WebAssembly text file, for example:

* `001_hello.wat`
* `002_binary.wat`
* `003_palindrome.wat`
* `004_factorial.wat`
* `005_arrays.wat`
* `006_next_day.wat`
* `007_literals.wat`
* `008_vars.wat`
* `009_operators.wat`
* `010_breaks.wat`

## How to Clean

To delete all the files that get created automatically, at the terminal type:

    make clean
