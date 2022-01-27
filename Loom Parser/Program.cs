using Loom_Parser.Parser.PrettyPrint;
using Loom_Parser.Parser.Lexer;
using Loom_Parser.Parser.ASTGen;
using Loom_Parser.Parser.ASTGen.AST.Statements;
using Loom_Parser.Parser.Lexer.Objects;
using System;
using Loom_Parser.Obfuscator;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            string script = File.ReadAllText("Tests\\Sample.lua");

            CodeLexer codeLexer = new CodeLexer(script);
            LexTokenList lexTokens = codeLexer.Analyze();

            CodeGenerator codeGenerator = new CodeGenerator(lexTokens);
            StatementList statements = codeGenerator.ParseStatements(new FunctionCallStatement());

            PrettyPrinter codeBeautifier = new PrettyPrinter(new PrettyPrinterSettings() 
            {
                Indentation = "    ",
                Minify = false,
            });

            Console.WriteLine("Amount of statements; " + statements.Count.ToString());
            Console.WriteLine("Original;");
            Console.WriteLine(codeBeautifier.Beautify(statements));

            ObfCore obfCore = new ObfCore();
            StatementList obfStatements = obfCore.Obfuscate(statements);

            Console.WriteLine("\nObfuscated;");
            Console.WriteLine(codeBeautifier.Beautify(obfStatements));
            Console.WriteLine("-- END --");

            Console.ReadLine();
        }
    }
}
