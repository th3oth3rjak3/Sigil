# Sigil Language Grammar (Extended BNF) - Complete

program = [ module_declaration ] { import_declaration } { declaration } ;

# Module and Import Declarations
module_declaration = "module" module_path ";" ;
module_path = identifier { "::" identifier } ;

import_declaration = "use" module_path [ "as" identifier ] ";" ;

declaration = class_declaration
            | interface_declaration
            | enum_declaration
            | function_declaration
            | statement ;

# Interface Declarations
interface_declaration = "interface" identifier [ generic_params ] "{" { method_signature } "}" ;

method_signature = identifier "(" parameter_list ")" [ "->" type_annotation ] ";" ;

# Class Declarations
class_declaration = "class" identifier [ generic_params ] [ inheritance_clause ] "{" { class_member } "}" ;

inheritance_clause = ":" inheritance_list ;

inheritance_list = identifier [ generic_type_args ] { "," identifier [ generic_type_args ] } ;

generic_type_args = "[" type_annotation { "," type_annotation } "]" ;

class_member = field_declaration
             | constructor_declaration
             | method_declaration
             | static_method_declaration ;

field_declaration = identifier ":" type_annotation ";" ;

constructor_declaration = "new" "(" parameter_list ")" [ ":" super_call ] block ;

super_call = "super" "(" argument_list ")" ;

method_declaration = identifier "(" parameter_list ")" [ "->" type_annotation ] block ;

static_method_declaration = "static" identifier "(" parameter_list ")" [ "->" type_annotation ] block ;

# Enum Declarations
enum_declaration = "enum" identifier [ generic_params ] "{" enum_variant_list [ "," ]
                   { method_declaration } "}" ;

enum_variant_list = enum_variant { "," enum_variant } ;

enum_variant = identifier "(" enum_variant_params ")" ;

enum_variant_params = type_annotation
                    | labeled_param_list ;

labeled_param_list = labeled_param { "," labeled_param } ;
labeled_param = identifier ":" type_annotation ;

# Function Declarations
function_declaration = "fun" identifier [ generic_params ] "(" parameter_list ")" [ "->" type_annotation ] block ;

parameter_list = [ parameter { "," parameter } ] ;
parameter = identifier ":" type_annotation ;

# Generic Parameters
generic_params = "[" identifier { "," identifier } "]" ;

# Type Annotations
type_annotation = primitive_type
                | identifier
                | generic_type
                | function_type
                | array_type
                | tuple_type ;

primitive_type = "String" | "Int" | "Float" | "Bool" | "()" ;

generic_type = identifier "[" type_annotation { "," type_annotation } "]" ;

function_type = "|" parameter_list "|" [ "->" type_annotation ] ;

array_type = "Array" "[" type_annotation "]" ;

tuple_type = "(" type_annotation "," type_annotation { "," type_annotation } ")" ;

# Statements
statement = expression_statement
          | let_statement
          | assignment_statement
          | match_statement
          | if_statement
          | while_statement
          | for_statement
          | return_statement
          | block ;

expression_statement = expression ";" ;

let_statement = "let" identifier [ ":" type_annotation ] "=" expression ";" ;

assignment_statement = lvalue "=" expression ";" ;

match_statement = "match" expression "{" match_arm { "," match_arm } [ "," ] "}" ;

match_arm = pattern "=>" expression ;

# Control Flow Statements
if_statement = "if" expression block [ "else" ( if_statement | block ) ] ;

while_statement = "while" expression block ;

for_statement = "for" identifier "in" expression block ;

return_statement = "return" [ expression ] ";" ;

block = "{" { statement } "}" ;

# Patterns (for match expressions)
pattern = enum_pattern
        | tuple_pattern
        | array_pattern
        | identifier
        | literal ;

enum_pattern = identifier "(" pattern_list ")" ;

tuple_pattern = "(" pattern { "," pattern } ")" ;

array_pattern = "[" pattern { "," pattern } "]" ;

pattern_list = [ pattern { "," pattern } ] ;

# Expressions (with precedence from lowest to highest)
expression = lambda_expression ;

lambda_expression = ( "|" [ parameter_list ] "|" assignment_expression )
                   | assignment_expression ;

assignment_expression = logical_or ;

logical_or = logical_and { "||" logical_and } ;

logical_and = equality { "&&" equality } ;

equality = comparison { ( "==" | "!=" ) comparison } ;

comparison = addition { ( "<" | ">" | "<=" | ">=" ) addition } ;

addition = multiplication { ( "+" | "-" ) multiplication } ;

multiplication = unary { ( "*" | "/" | "%" ) unary } ;

unary = ( "!" | "-" ) unary | postfix ;

postfix = primary { postfix_op } ;

postfix_op = "(" argument_list ")"              # Function call
           | "." identifier                     # Field access
           | "." identifier "(" argument_list ")" # Method call
           | "[" expression "]"                 # Array/tuple indexing
           ;

primary = identifier
        | literal
        | array_literal
        | tuple_literal
        | string_interpolation
        | enum_constructor
        | static_method_call
        | "(" expression ")"
        | "this"
        | "super" ;

argument_list = [ expression { "," expression } ] ;

# Literals
literal = integer_literal
        | float_literal
        | string_literal
        | boolean_literal
        | unit_literal ;

array_literal = "[" [ expression { "," expression } ] "]" ;

tuple_literal = "(" expression "," expression { "," expression } ")" ;

integer_literal = digit { digit } ;
float_literal = digit { digit } "." digit { digit } ;
string_literal = '"' { string_char } '"' ;
boolean_literal = "true" | "false" ;
unit_literal = "()" ;

# String Interpolation
string_interpolation = '$"' { interpolation_part } '"' ;
interpolation_part = string_char | "{" expression "}" ;

# Enum Constructor and Static Method Calls
enum_constructor = identifier "." identifier "(" argument_list ")" ;

static_method_call = identifier "::" identifier "(" argument_list ")" ;

# Left-hand values (for assignments)
lvalue = identifier
       | "this" "." identifier
       | "super" "." identifier ;

# Identifiers and Characters
identifier = letter { letter | digit | "_" } ;

letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
         "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" |
         "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
         "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;

digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

string_char = ? any character except '"' and newline ? ;

# Comments
comment = "//" { ? any character except newline ? } newline ;

# Whitespace (ignored by parser)
whitespace = " " | "\t" | "\r" | "\n" ;
