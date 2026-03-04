using Loom.Parser.PrettyPrint;
using Loom.Parser.Lexer;
using Loom.Parser.ASTGen;
using Loom.Parser.ASTGen.AST.Statements;
using Loom.Parser.Lexer.Objects;
using Loom.Parser.ASTGen.AST.Expressions;
using Loom.Parser.ASTGen.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom
{
    class Program
    {
        static void Main(string[] args)
        {
            string script = File.ReadAllText("Tests\\Sample.lua");

            LexicalAnalyser codeLexer = new LexicalAnalyser(script);
            LexTokenList lexTokens = codeLexer.Analyze();

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

            PrettyPrinter prettyPrinter = new PrettyPrinter(PrettyPrinterSettings.Minify);

            Console.WriteLine("Amount of statements; " + statements.Count.ToString());
            Console.WriteLine("Original;");
            Console.WriteLine(prettyPrinter.Print(statements));

            Console.ReadLine();
        }
    }
}
