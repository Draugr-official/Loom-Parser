using Loom.Parser.PrettyPrint;
using Loom.Parser.Lexer;
using Loom.Parser.ASTGenerator;
using Loom.Parser.ASTGenerator.AST.Statements;
using Loom.Parser.Lexer.Objects;
using Loom.Parser.ASTGenerator.AST.Expressions;
using Loom.Parser.ASTGenerator.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom.Parser;
using Loom.Parser.Preprocessor;

namespace Loom
{
    class Program
    {
        static void Main(string[] args)
        {
            string script = File.ReadAllText("Tests\\Sample.lua");

            Console.WriteLine("Lexing...");
            LexicalAnalyser codeLexer = new LexicalAnalyser(script);
            LexTokenList lexTokens = codeLexer.Analyze();

            Console.WriteLine("Preprocessing...");
            Preprocessor preProcessor = new Preprocessor(lexTokens);
            lexTokens = preProcessor.Process();

            Console.WriteLine("Generating AST...");
            ASTGenerator astGenerator = new ASTGenerator(lexTokens);
            StatementList statements = astGenerator.ParseStatements();

            

            //statements.Add(new VariableAssignStatement()
            //{
            //    IsLocal = true,
            //    Name = "Stack",
            //    Value = new ArrayExpression()
            //    {
            //        Array = new ExpressionList() { new IndexExpression() {  } }
            //    }
            //});

            //statements.Add(new VariableAssignStatement()
            //{
            //    IsLocal = true,
            //    Name = "getter",
            //    Value = new IndexExpression()
            //    {
            //        Array = new VariableExpression()
            //        {
            //            Name = "Stack"
            //        },
            //        Index = new ConstantExpression()
            //        {
            //            Type = DataTypes.Number,
            //            Value = "1"
            //        }
            //    }
            //});

            //statements.Add(new FunctionCallStatement()
            //{
            //    Name = "print",
            //    Arguments = { new VariableExpression() { Name = "getter" } }
            //});

            PrettyPrinter prettyPrinter = new PrettyPrinter(PrettyPrinterSettings.Beautify);

            Console.WriteLine("Amount of statements; " + statements.Count.ToString());
            statements.ForEach(s => Console.WriteLine(s.GetType().FullName));
            Console.WriteLine("Original;");
            Console.WriteLine(prettyPrinter.Print(statements));

            Console.ReadLine();
        }
    }
}
