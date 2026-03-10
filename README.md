# Loom Parser
Loom parser is a robust Lua parser designed to make transforming of AST easy.\
This parser is in development and is highly experimental.\
The end goal is a parser with AST you can easily interact with.

# Usage
Example
```cs
string script = File.ReadAllText("Tests\\Sample.lua");

LexicalAnalyser codeLexer = new LexicalAnalyser(script);
LexTokenList lexTokens = codeLexer.Analyze();

ASTGenerator astGenerator = new ASTGenerator(lexTokens);
StatementList statements = astGenerator.ParseStatements();

PrettyPrinter prettyPrinter = new PrettyPrinter(PrettyPrinterSettings.Minify);
Console.WriteLine(prettyPrinter.Print(statements));
```

# Supported syntax
* Statements
  * Locals
    * Functions
    * Variables
  * Assignments
    * Multiple assignments
  * Function declarations
  * If .. then
  * While .. do
  * Return
  * For .. do
  * Repeat .. until
  * Do
  * Break
  * Continue
  * Calls
 
* Expressions
  * Ternary (if true then "hello" else "goodbye")
  * Grouped 
  * Negative (E.g -10)
  * Length (E.g #table)
  * Nil
  * Function declaration (e.g function(...) print(...) end)
  * Identifier (E.g print)
  * Vararg (E.g ...)
  * Constant (E.g 123, "hello")
  * Array (E.g {["this"] = "hi"})
  * Call (E.g math.pow(5, 5))
  * Index (E.g table[1])
  * Relational (E.g 4 > 3)
  * Arithmetic (E.g 2 + 2)
  * Logical (E.g and, or, not)
  * Concat (E.g "Hello " .. "world")
