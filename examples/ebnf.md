(* Sigil Language Grammar (Extended BNF) - Complete *)
program = [ module_declaration ] { import_declaration } { declaration } ;

(* Module and Import Declarations *)
module_declaration = "module" module_path ";" ;
module_path = identifier { "::" identifier } ;
import_declaration = "use" module_path [ "as" identifier ] ";" ;

declaration = class_declaration
            | interface_declaration
            | enum_declaration
            | function_declaration
            | statement ;

(* Interface Declarations *)
interface_declaration = "interface" identifier [ generic_params ] "{" { method_signature } "}" ;
method_signature = identifier "(" parameter_list ")" [ "->" type_annotation ] ";" ;

(* Class Declarations *)
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

(* Enum Declarations *)
enum_declaration = "enum" identifier [ generic_params ] "{" enum_variant_list [ "," ](* Sigil Language Grammar (Extended BNF) - Complete *)
program = [ module_declaration ] { import_declaration } { declaration } ;

(* Module and Import Declarations *)
module_declaration = "module" module_path ";" ;
module_path = identifier { "::" identifier } ;
import_declaration = "use" module_path [ "as" identifier ] ";" ;

declaration = class_declaration
            | interface_declaration
            | enum_declaration
            | function_declaration
            | statement ;

(* Interface Declarations *)
interface_declaration = "interface" identifier [ generic_params ] "{" { method_signature } "}" ;
method_signature = identifier "(" parameter_list ")" [ "->" type_annotation ] ";" ;

(* Class Declarations *)
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

(* Enum Declarations *)
enum_declaration = "enum" identifier [ generic_params ] "{" enum_variant_list [ "," ]
                   { method_declaration } "}" ;
enum_variant_list = enum_variant { "," enum_variant } ;
enum_variant = identifier "(" enum_variant_params ")" ;
enum_variant_params = type_annotation
                    | labeled_param_list ;
labeled_param_list = labeled_param { "," labeled_param } ;
labeled_param = identifier ":" type_annotation ;

(* Function Declarations *)
function_declaration = "fun" identifier [ generic_params ] "(" parameter_list ")" [ "->" type_annotation ] block ;
parameter_list = [ parameter { "," parameter } ] ;
parameter = identifier ":" type_annotation ;

(* Generic Parameters *)
generic_params = "[" identifier { "," identifier } "]" ;

(* Type Annotations *)
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

(* Statements *)
statement = expression_statement
          | let_statement
          | assignment_statement
          | match_statement
          | if_statement
          | while_statement
          | for_statement
          | return_statement
          | spawn_statement
          | block ;

expression_statement = expression ";" ;
let_statement = "let" assignment_target [ ":" type_annotation ] "=" expression ";" ;
assignment_statement = assignment_target "=" expression ";" ;
match_statement = "match" expression "{" match_arm { "," match_arm } [ "," ] "}" ;
match_arm = pattern "=>" expression ;
spawn_statement = "spawn" block ;

(* Assignment Targets and Destructuring *)
assignment_target = lvalue
                  | tuple_destructure ;

tuple_destructure = "(" identifier { "," identifier } ")" ;

(* Control Flow Statements *)
if_statement = "if" expression block [ "else" ( if_statement | block ) ] ;
while_statement = "while" expression block ;
for_statement = "for" identifier "in" expression block ;
return_statement = "return" [ expression ] ";" ;
block = "{" { statement } "}" ;

(* Patterns for match expressions *)
pattern = enum_pattern
        | tuple_pattern
        | array_pattern
        | identifier
        | literal ;

enum_pattern = identifier "::" identifier "(" pattern_list ")" ;
tuple_pattern = "(" pattern { "," pattern } ")" ;
array_pattern = "[" pattern { "," pattern } "]" ;
pattern_list = [ pattern { "," pattern } ] ;

(* Expressions with precedence from lowest to highest *)
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

postfix_op = "(" argument_list ")"                    (* Function call *)
           | "." identifier                           (* Field access *)
           | "." identifier "(" argument_list ")"     (* Instance method call *)
           | "[" expression "]"                       (* Array/tuple indexing *)
           ;

primary = identifier
        | literal
        | array_literal
        | tuple_literal
        | string_interpolation
        | enum_constructor
        | static_method_call
        | module_function_call
        | "(" expression ")"
        | "this"
        | "super" ;

argument_list = [ expression { "," expression } ] ;

(* Literals *)
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

(* String Interpolation *)
string_interpolation = '$"' { interpolation_part } '"' ;
interpolation_part = interpolation_char | "{" expression "}" ;

(* Static calls, enum constructors, and module function calls *)
enum_constructor = identifier "::" identifier "(" argument_list ")" ;
static_method_call = identifier "::" identifier "(" argument_list ")" ;
module_function_call = module_path "::" identifier "(" argument_list ")" ;

(* Left-hand values for assignments *)
lvalue = identifier
       | "this" "." identifier
       | "super" "." identifier
       | lvalue "[" expression "]"
       | lvalue "." identifier ;

(* Identifiers and Characters - Updated for visibility *)
identifier = exported_identifier | private_identifier ;
exported_identifier = uppercase_letter { letter | digit | "_" } ;
private_identifier = lowercase_letter { letter | digit | "_" } ;

uppercase_letter = "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
                   "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;

lowercase_letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
                   "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" ;

letter = uppercase_letter | lowercase_letter ;

digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

(* String Character Definitions *)
string_char = escape_sequence | normal_char ;
escape_sequence = "\" ( "n" | "t" | "r" | "\" | '"' | "0" | "u" unicode_hex ) ;
unicode_hex = hex_digit hex_digit hex_digit hex_digit ;
hex_digit = digit | "a" | "b" | "c" | "d" | "e" | "f" | "A" | "B" | "C" | "D" | "E" | "F" ;
normal_char = letter | digit | space | punctuation ;
space = " " | "\t" ;
punctuation = "!" | "@" | "#" | "$" | "%" | "^" | "&" | "*" | "(" | ")" | "-" | "_" |
              "=" | "+" | "[" | "]" | "{" | "}" | "|" | ";" | ":" | "'" | "<" | ">" |
              "," | "." | "?" | "/" | "~" | "`" ;

(* String Interpolation Character Definitions *)
interpolation_char = escape_sequence | interpolation_normal_char ;
interpolation_normal_char = letter | digit | space | interpolation_punctuation ;
interpolation_punctuation = "!" | "@" | "#" | "$" | "%" | "^" | "&" | "*" | "(" | ")" |
                           "-" | "_" | "=" | "+" | "[" | "]" | "|" | ";" | ":" | "'" |
                           "<" | ">" | "," | "." | "?" | "/" | "~" | "`" ;
(* Note: excludes '{', '}', and '"' which have special meaning in interpolation *)

(* Comments *)
comment = "//" { comment_char } newline ;
comment_char = letter | digit | space | punctuation | "{" | "}" ;
newline = "\n" | "\r\n" | "\r" ;

(* Whitespace - ignored by parser *)
whitespace = " " | "\t" | "\r" | "\n" ;
                   { method_declaration } "}" ;
enum_variant_list = enum_variant { "," enum_variant } ;
enum_variant = identifier "(" enum_variant_params ")" ;
enum_variant_params = type_annotation
                    | labeled_param_list ;
labeled_param_list = labeled_param { "," labeled_param } ;
labeled_param = identifier ":" type_annotation ;

(* Function Declarations *)
function_declaration = "fun" identifier [ generic_params ] "(" parameter_list ")" [ "->" type_annotation ] block ;
parameter_list = [ parameter { "," parameter } ] ;
parameter = identifier ":" type_annotation ;

(* Generic Parameters *)
generic_params = "[" identifier { "," identifier } "]" ;

(* Type Annotations *)
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

(* Statements *)
statement = expression_statement
          | let_statement
          | assignment_statement
          | match_statement
          | if_statement
          | while_statement
          | for_statement
          | return_statement
          | spawn_statement
          | block ;

expression_statement = expression ";" ;
let_statement = "let" assignment_target [ ":" type_annotation ] "=" expression ";" ;
assignment_statement = assignment_target "=" expression ";" ;
match_statement = "match" expression "{" match_arm { "," match_arm } [ "," ] "}" ;
match_arm = pattern "=>" expression ;
spawn_statement = "spawn" block ;

(* Assignment Targets and Destructuring *)
assignment_target = lvalue
                  | tuple_destructure ;

tuple_destructure = "(" identifier { "," identifier } ")" ;

(* Control Flow Statements *)
if_statement = "if" expression block [ "else" ( if_statement | block ) ] ;
while_statement = "while" expression block ;
for_statement = "for" identifier "in" expression block ;
return_statement = "return" [ expression ] ";" ;
block = "{" { statement } "}" ;

(* Patterns for match expressions *)
pattern = enum_pattern
        | tuple_pattern
        | array_pattern
        | identifier
        | literal ;

enum_pattern = identifier "::" identifier "(" pattern_list ")" ;
tuple_pattern = "(" pattern { "," pattern } ")" ;
array_pattern = "[" pattern { "," pattern } "]" ;
pattern_list = [ pattern { "," pattern } ] ;

(* Expressions with precedence from lowest to highest *)
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

postfix_op = "(" argument_list ")"                    (* Function call *)
           | "." identifier                           (* Field access *)
           | "." identifier "(" argument_list ")"     (* Instance method call *)
           | "[" expression "]"                       (* Array/tuple indexing *)
           ;

primary = identifier
        | literal
        | array_literal
        | tuple_literal
        | string_interpolation
        | enum_constructor
        | static_method_call
        | module_function_call
        | "(" expression ")"
        | "this"
        | "super" ;

argument_list = [ expression { "," expression } ] ;

(* Literals *)
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

(* String Interpolation *)
string_interpolation = '$"' { interpolation_part } '"' ;
interpolation_part = interpolation_char | "{" expression "}" ;

(* Static calls, enum constructors, and module function calls *)
enum_constructor = identifier "::" identifier "(" argument_list ")" ;
static_method_call = identifier "::" identifier "(" argument_list ")" ;
module_function_call = module_path "::" identifier "(" argument_list ")" ;

(* Left-hand values for assignments *)
lvalue = identifier
       | "this" "." identifier
       | "super" "." identifier
       | lvalue "[" expression "]"
       | lvalue "." identifier ;

(* Identifiers and Characters *)
identifier = letter { letter | digit | "_" } ;
letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
         "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" |
         "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
         "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;

digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

(* String Character Definitions *)
string_char = escape_sequence | normal_char ;
escape_sequence = "\" ( "n" | "t" | "r" | "\" | '"' | "0" | "u" unicode_hex ) ;
unicode_hex = hex_digit hex_digit hex_digit hex_digit ;
hex_digit = digit | "a" | "b" | "c" | "d" | "e" | "f" | "A" | "B" | "C" | "D" | "E" | "F" ;
normal_char = letter | digit | space | punctuation ;
space = " " | "\t" ;
punctuation = "!" | "@" | "#" | "$" | "%" | "^" | "&" | "*" | "(" | ")" | "-" | "_" |
              "=" | "+" | "[" | "]" | "{" | "}" | "|" | ";" | ":" | "'" | "<" | ">" |
              "," | "." | "?" | "/" | "~" | "`" ;

(* String Interpolation Character Definitions *)
interpolation_char = escape_sequence | interpolation_normal_char ;
interpolation_normal_char = letter | digit | space | interpolation_punctuation ;
interpolation_punctuation = "!" | "@" | "#" | "$" | "%" | "^" | "&" | "*" | "(" | ")" |
                           "-" | "_" | "=" | "+" | "[" | "]" | "|" | ";" | ":" | "'" |
                           "<" | ">" | "," | "." | "?" | "/" | "~" | "`" ;
(* Note: excludes '{', '}', and '"' which have special meaning in interpolation *)

(* Comments *)
comment = "//" { comment_char } newline ;
comment_char = letter | digit | space | punctuation | "{" | "}" ;
newline = "\n" | "\r\n" | "\r" ;

(* Whitespace - ignored by parser *)
whitespace = " " | "\t" | "\r" | "\n" ;
