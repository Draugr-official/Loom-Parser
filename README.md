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
