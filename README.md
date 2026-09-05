# Sigil

**Sigil is a statically typed programming language compiled to native code through LLVM.**

> 🚧 Sigil is an early-stage language under active development.

The goal of Sigil is to explore the design and implementation of a modern compiled programming language, from source code all the way to a native executable.

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
     ▼
   Clang/LLVM
     │
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

Arithmetic currently includes:

- Integer addition (`+`)
- Integer subtraction (`-`)

Variables are resolved, type checked, lowered to LLVM storage, and loaded when referenced.

---

# Roadmap

The roadmap is organized as a sequence of **vertical language milestones**.

Each language feature is intended to travel through the entire compiler pipeline:

```text
Syntax → AST → Resolution → Type Checking → LLVM → Native Execution
```

The goal is to avoid implementing large amounts of language infrastructure without proving that the feature can actually execute.

---

## Phase 0 — Compiler Foundation

- [x] Project structure
- [x] Lexer
- [x] Parser
- [x] AST
- [x] Name resolver
- [x] Bound AST
- [x] Type system foundation
- [x] Type checker
- [x] Typed AST
- [x] LLVMSharp integration
- [x] LLVM IR generation
- [x] Native compilation through Clang
- [x] Compiler API
- [x] Command-line interface
- [x] End-to-end source → executable pipeline

---

## Phase 1 — Expressions & Arithmetic

Build the expression system into a useful foundation.

- [x] Integer literals
- [x] Local variable declarations
- [x] Identifier resolution
- [x] Integer addition
- [x] Integer subtraction
- [ ] Integer multiplication
- [ ] Integer division
- [ ] Integer remainder
- [ ] Unary negation
- [ ] Parenthesized expressions
- [ ] Operator precedence
- [ ] Relational operators
  - [ ] `==`
  - [ ] `!=`
  - [ ] `<`
  - [ ] `<=`
  - [ ] `>`
  - [ ] `>=`

---

## Phase 2 — Booleans & Logic

Introduce boolean values and logical expressions.

- [ ] `Boolean` type
- [ ] Boolean literals
- [ ] Logical NOT
- [ ] Logical AND
- [ ] Logical OR
- [ ] Boolean equality
- [ ] Short-circuit evaluation
- [ ] Type-safe boolean expressions

Example:

```sigil
fn is_positive(x: Integer) -> Boolean {
    return x > 0;
}
```

---

## Phase 3 — Control Flow

Make programs capable of making decisions and repeating work.

- [ ] `if`
- [ ] `else`
- [ ] Conditional expressions
- [ ] `while`
- [ ] `break`
- [ ] `continue`
- [ ] Control-flow-aware type checking
- [ ] LLVM basic-block generation
- [ ] Conditional branches
- [ ] Loops

Example:

```sigil
fn main() -> Integer {
    let x: Integer = 10;

    if (x > 5) {
        return 42;
    }

    return 0;
}
```

---

## Phase 4 — Functions

Move beyond a single hard-coded entry point.

- [ ] Function parameters
- [ ] Function calls
- [ ] Multiple return paths
- [ ] Function-local scopes
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

## Phase 5 — Types

Expand the type system beyond integers and booleans.

- [ ] Floating-point types
- [ ] String type
- [ ] Character type
- [ ] Type conversions
- [ ] Type inference where appropriate
- [ ] Explicit type annotations
- [ ] Type compatibility rules

---

## Phase 6 — Composite Data

Introduce user-defined and structured data.

- [ ] Arrays
- [ ] Tuples
- [ ] Structs
- [ ] Field access
- [ ] Struct initialization
- [ ] Nested composite types
- [ ] Value semantics
- [ ] LLVM aggregate types

Example:

```sigil
struct Point {
    x: Integer;
    y: Integer;
}
```

---

## Phase 7 — Enums & Pattern Matching

Add algebraic-style data modeling.

- [ ] Enums
- [ ] Enum variants
- [ ] Variant payloads
- [ ] Pattern matching
- [ ] Exhaustiveness checking
- [ ] Destructuring
- [ ] Match guards

---

## Phase 8 — References & Memory

Introduce explicit memory concepts while keeping the language's safety model deliberate.

- [ ] References
- [ ] Dereferencing
- [ ] Mutable references
- [ ] Ownership model
- [ ] Borrow checking
- [ ] Stack allocation
- [ ] Heap allocation
- [ ] Memory lifetime rules

This phase will depend heavily on the final language semantics and is intentionally less prescriptive than the earlier phases.

---

## Phase 9 — Modules & Packages

Turn individual source files into real programs.

- [ ] Multiple source files
- [ ] Module declarations
- [ ] Imports
- [ ] Exports
- [ ] Module-level scopes
- [ ] Cross-module name resolution
- [ ] Visibility
- [ ] Package structure
- [ ] Dependency resolution

---

## Phase 10 — Generics

Introduce reusable abstractions.

- [ ] Generic functions
- [ ] Generic types
- [ ] Type parameters
- [ ] Generic type checking
- [ ] Monomorphization or alternative generic lowering strategy
- [ ] Generic constraints / bounds

---

## Phase 11 — Standard Library

Build the language's core runtime facilities.

- [ ] Strings
- [ ] Collections
- [ ] I/O
- [ ] File access
- [ ] Process/environment APIs
- [ ] Error handling
- [ ] Basic concurrency primitives
- [ ] Platform abstractions

---

## Phase 12 — Error Handling

Define how programs represent and recover from failure.

- [ ] Result type
- [ ] Error values
- [ ] Error propagation
- [ ] `try` / propagation syntax
- [ ] Pattern matching on errors
- [ ] Compiler diagnostics for invalid error handling

---

## Phase 13 — Tooling

Make Sigil pleasant to actually use.

- [ ] Improved compiler diagnostics
- [ ] Source locations throughout the compiler
- [ ] Pretty error messages
- [ ] Formatter
- [ ] Language server
- [ ] Editor integration
- [ ] REPL / interactive tooling where appropriate
- [ ] Documentation generation

---

## Phase 14 — Production Compiler

Move from "language experiment" toward a serious compiler.

- [ ] Cross-platform native compilation
- [ ] Debug information
- [ ] Optimization pipeline
- [ ] LLVM optimization passes
- [ ] Release/debug build modes
- [ ] Incremental compilation
- [ ] Compiler performance improvements
- [ ] Deterministic builds
- [ ] Comprehensive compiler test suite
- [ ] Compiler self-hosting investigation

---

# Development Philosophy

Sigil is being developed using **vertical language slices**.

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

The compiler is divided into distinct stages:

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

Responsible for understanding what the program means.

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

---

# Project Status

Sigil is **not yet ready for production use**.

The compiler architecture is being established first, with language features being added incrementally. Expect breaking syntax changes, incomplete features, and architectural changes while the language design evolves.

The current milestone is simple:

> **Build a small language that can reliably turn source code into native machine code, then grow the language without losing that end-to-end guarantee.**

---
