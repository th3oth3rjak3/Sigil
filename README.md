# Sigil

**Sigil is a statically typed programming language inspired by Lox and compiled to native code through LLVM.**

> 🚧 Sigil is an early-stage language under active development.

The goal of Sigil is to explore the design and implementation of a small, practical programming language with a straightforward type system and a native compilation pipeline.

```text
Sigil Source
     │
     ▼
   Lexer
     │
     ▼
   Parser
     │
     ▼
    AST
     │
     ▼
Name Resolution
     │
     ▼
 Bound AST
     │
     ▼
 Type Checking
     │
     ▼
 Typed AST
     │
     ▼
 LLVM Code Generation
     │
     ▼
   LLVM IR
     │
     ├──────────────────┐
     ▼                  ▼
 Native Compiler   Sigil Runtime
     │                  │
     └────────┬─────────┘
              ▼
      Native Executable
```

## Current Status

Sigil can currently compile and execute simple programs all the way from source code to a native Linux executable.

The compiler currently has:

- Lexer
- Parser
- Abstract syntax tree
- Name resolution
- Bound AST
- Static type checking
- Typed AST
- LLVM IR generation
- Native compilation through Clang
- Compiler API
- Command-line compiler
- End-to-end compiler tests

### Currently Supported

```sigil
fn main() -> Integer {
    let x: Integer = 20;
    let y: Integer = 22;
    return x + y;
}
```

This program is compiled to LLVM IR and then to a native executable.

---

# Roadmap

Sigil is being developed as a sequence of **vertical language slices**.

A feature is considered complete when it can travel through the entire compiler:

```text
Source
  ↓
Lexer
  ↓
Parser
  ↓
AST
  ↓
Name Resolution
  ↓
Bound AST
  ↓
Type Checking
  ↓
Typed AST
  ↓
LLVM Code Generation
  ↓
Native Compilation
  ↓
End-to-End Test
```

The goal is to avoid building large amounts of compiler infrastructure without proving that the language feature can actually execute.

---

## Phase 0 — Compiler Foundation

- [x] Project structure
- [x] Lexer
- [x] Parser
- [x] AST
- [x] Name resolution
- [x] Bound AST
- [x] Type system
- [x] Type checker
- [x] Typed AST
- [x] LLVMSharp integration
- [x] LLVM IR generation
- [x] Native compilation through Clang
- [x] Compiler API
- [x] Command-line compiler
- [x] End-to-end source → executable pipeline

---

## Phase 1 — Runtime Foundation

Establish the native runtime boundary between compiled Sigil programs and the runtime implementation.

The compiler is implemented in C#, while the Sigil runtime is implemented separately in Zig.

```text
Sigil Source
     │
     ▼
C# Compiler
     │
     ▼
LLVM IR
     │
     ▼
Native Compiler
     │
     ├───────────────┐
     ▼               ▼
Sigil Program   Sigil Runtime
                     │
                     ▼
                   Zig
```

The compiler should only depend on the runtime's public ABI. Runtime implementation details should remain outside the compiler.

- [ ] Runtime project
- [ ] Runtime build system
- [ ] C ABI between compiler-generated code and runtime
- [ ] Runtime library linking
- [ ] Runtime initialization
- [ ] Runtime error handling
- [ ] Basic runtime allocator
- [ ] `print` runtime function
- [ ] `println` runtime function
- [ ] Integer output
- [ ] Float output
- [ ] End-to-end stdout testing
- [ ] Runtime/stdlib ABI conventions
- [ ] Runtime library integration with native compilation

---

## Phase 2 — Expressions

Build the expression system into a useful foundation.

- [x] Integer literals
- [x] Float literals
- [x] Local variable declarations
- [x] Identifier expressions
- [x] Binary expressions
- [x] Integer addition
- [x] Integer subtraction
- [x] Integer multiplication
- [x] Integer division
- [ ] Integer remainder
- [ ] Float addition _(blocked: needs stdout or another result-verification mechanism)_
- [ ] Float subtraction _(blocked: needs stdout or another result-verification mechanism)_
- [ ] Float multiplication _(blocked: needs stdout or another result-verification mechanism)_
- [ ] Float division _(blocked: needs stdout or another result-verification mechanism)_
- [ ] Unary expressions
- [ ] Unary negation
- [ ] Parenthesized expressions
- [ ] Operator precedence
    - [x] addition/multiplication
    - [ ] unary
    - [ ] parentheses
    - [ ] relational/equality
- [ ] Relational operators
    - [ ] `<`
    - [ ] `<=`
    - [ ] `>`
    - [ ] `>=`
- [ ] Equality operators
    - [ ] `==`
    - [ ] `!=`

---

## Phase 3 — Functions

Move beyond a single hard-coded entry point.

- [ ] Function parameters
- [ ] Function calls
- [ ] Argument evaluation
- [ ] Function-local scopes
- [ ] Multiple functions
- [ ] Recursive functions
- [ ] Argument type checking
- [ ] Return type checking
- [ ] LLVM function calls

Example:

```sigil
fn add(a: Integer, b: Integer) -> Integer {
    return a + b;
}

fn main() -> Integer {
    return add(20, 22);
}
```

---

## Phase 4 — Booleans & Control Flow

Introduce boolean values and the ability to make decisions.

- [ ] Boolean literals
- [ ] Boolean expressions
- [ ] Logical NOT
- [ ] Logical AND
- [ ] Logical OR
- [ ] `if`
- [ ] `else`
- [ ] `while`
- [ ] `break`
- [ ] `continue`
- [ ] Short-circuit evaluation
- [ ] LLVM conditional branches
- [ ] LLVM basic-block generation
- [ ] LLVM loop generation

Example:

```sigil
fn is_positive(x: Integer) -> Boolean {
    return x > 0;
}
```

---

## Phase 5 — Core Types

Expand the type system beyond integers and booleans.

- [ ] Floating-point types
- [ ] String type
- [ ] Character type
- [ ] Type conversions
- [ ] Type inference where appropriate
- [ ] Explicit type annotations
- [ ] Type compatibility rules
- [ ] `null` / nullable references

---

## Phase 6 — User-Defined Types

Introduce structured value types.

- [ ] Structs
- [ ] Struct initialization
- [ ] Field access
- [ ] Value semantics
- [ ] Arrays
- [ ] Array indexing
- [ ] Array length
- [ ] Nested composite types
- [ ] LLVM aggregate types

Example:

```sigil
struct Point {
    x: Integer;
    y: Integer;
}
```

---

## Phase 7 — Reference Types & Objects

Introduce reference semantics and object-oriented features.

Sigil uses conventional value/reference semantics:

- Value types are copied when passed or assigned.
- Reference types are represented by references to objects.
- There is no ownership or borrowing system.

- [ ] Classes
- [ ] Object construction
- [ ] Instance fields
- [ ] Instance methods
- [ ] Method calls
- [ ] Reference semantics
- [ ] Reference equality
- [ ] Value equality
- [ ] Heap-allocated objects
- [ ] Object representation
- [ ] Garbage collector integration

---

## Phase 8 — Enums & Pattern Matching

Add additional ways to model data and control program flow.

- [ ] Enums
- [ ] Enum variants
- [ ] Variant payloads
- [ ] Pattern matching
- [ ] Exhaustiveness checking
- [ ] Destructuring

---

## Phase 9 — Modules

Turn individual source files into larger programs.

- [ ] Multiple source files
- [ ] Modules
- [ ] Imports
- [ ] Exports
- [ ] Visibility
- [ ] Module-level declarations
- [ ] Cross-module name resolution

---

## Phase 10 — Error Handling

Define how programs represent and recover from failures.

- [ ] Error type
- [ ] Result type
- [ ] Error propagation
- [ ] Error handling syntax
- [ ] Pattern matching on errors

---

## Phase 11 — Generic Programming

Introduce reusable abstractions where they provide real value.

- [ ] Generic functions
- [ ] Generic types
- [ ] Type parameters
- [ ] Generic type checking
- [ ] Generic code generation
- [ ] Generic constraints, if needed

---

## Phase 12 — Standard Library

Build the core facilities needed for useful programs.

- [ ] Strings
- [ ] Collections
- [ ] I/O
- [ ] Filesystem APIs
- [ ] Process/environment APIs
- [ ] Date/time
- [ ] Networking
- [ ] Concurrency primitives
- [ ] Platform abstractions

---

## Phase 13 — Tooling

Make Sigil pleasant to develop with.

- [ ] Source-aware diagnostics
- [ ] Pretty compiler errors
- [ ] Formatter
- [ ] Language server
- [ ] Editor integration
- [ ] Documentation generation
- [ ] Package manager

---

## Phase 14 — Production Compiler

Move from language experiment toward a serious compiler.

- [ ] Debug information
- [ ] Optimization
- [ ] LLVM optimization pipeline
- [ ] Debug/release build modes
- [ ] Cross-platform native compilation
- [ ] Incremental compilation
- [ ] Compiler performance improvements
- [ ] Deterministic builds
- [ ] Comprehensive compiler test suite
- [ ] Compiler self-hosting investigation

---

# Development Philosophy

Sigil is developed using **vertical language slices**.

When a feature is added, it should be represented throughout the compiler:

```text
Lexer
  ↓
Parser
  ↓
AST
  ↓
Name Resolution
  ↓
Bound AST
  ↓
Type Checking
  ↓
Typed AST
  ↓
LLVM Code Generation
  ↓
Native Compilation
  ↓
End-to-End Test
```

For example, integer addition is not considered complete merely because the parser understands `+`.

It is complete because this:

```sigil
fn main() -> Integer {
    let x: Integer = 20;
    let y: Integer = 22;
    return x + y;
}
```

can travel through the entire compiler and produce a native executable that actually returns `42`.

This approach keeps the compiler grounded in the language it is supposed to implement.

---

# Compiler Architecture

The compiler is divided into several major stages.

### Syntax

Responsible for turning source text into an AST.

```text
Source
  ↓
Lexer
  ↓
Parser
  ↓
AST
```

### Semantics

Responsible for determining what the program means.

```text
AST
 ↓
Name Resolver
 ↓
Bound AST
 ↓
Type Checker
 ↓
Typed AST
```

### Code Generation

Responsible for turning the typed program into LLVM IR.

```text
Typed AST
    ↓
LLVMSharp
    ↓
LLVM IR
```

### Native Compilation

LLVM IR is passed to the native LLVM toolchain to produce the final executable.

```text
LLVM IR
  ↓
Clang
  ↓
Native executable
```

### Runtime

The Sigil runtime provides the low-level services required by compiled Sigil programs.

The runtime is implemented independently of the C# compiler. The compiler communicates with it through a small, stable ABI.

```text
LLVM-generated code
        │
        ▼
    Runtime ABI
        │
        ▼
   Zig Runtime
```

The runtime will eventually provide facilities such as:

- Memory allocation
- Garbage collection
- Object management
- Runtime initialization
- Runtime error handling
- String operations
- Standard I/O
- Other low-level services required by the language

The compiler should generate calls to runtime functions rather than embedding runtime implementation details directly into generated code.

### Standard Library

The standard library provides higher-level functionality available to Sigil programs.

Examples include:

- Strings
- Collections
- Filesystem APIs
- Process/environment APIs
- Date/time
- Networking
- Concurrency
- Platform abstractions

The standard library may be implemented using the Sigil runtime, but the two are separate architectural layers.

### Performance Philosophy

Sigil is intended to be a compiled, native language with performance suitable for general-purpose applications. The target is broadly comparable to languages such as Go, although exact performance will depend on the program and runtime implementation.

Performance should primarily be an implementation concern.

Sigil intentionally avoids exposing optimization strategies as language features unless they are necessary for defining program semantics. The compiler and runtime should be responsible for producing efficient native code without requiring programmers to annotate ordinary code with optimization directives.

In practice, this means preferring:

- Simple language semantics
- Straightforward value and reference semantics
- Automatic compiler optimizations
- An efficient runtime
- LLVM's optimization infrastructure
- Profiling and measurement over speculation

The guiding principle is:

> **Make the language simple. Make the compiler clever.**

---

# Project Status

Sigil is **not yet ready for production use**.

The compiler architecture is being established first, with language features added incrementally. Expect breaking syntax changes, incomplete features, and architectural changes while the language design evolves.

The current goal is straightforward:

> **Build a small, practical language that can reliably turn source code into native machine code, then grow the language without losing that end-to-end guarantee.**
