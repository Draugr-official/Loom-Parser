using Loom_Parser.Parser.PrettyPrint;
using Loom_Parser.Parser.Lexer;
using Loom_Parser.Parser.ASTGen;
using Loom_Parser.Parser.ASTGen.AST.Statements;
using Loom_Parser.Parser.Lexer.Objects;
using Loom_Parser.Parser.ASTGen.AST.Expressions;
using Loom_Parser.Parser.ASTGen.AST;
using System;
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

            LexicalAnalyser codeLexer = new LexicalAnalyser(script);
            LexTokenList lexTokens = codeLexer.Analyze();

            ASTGenerator astGenerator = new ASTGenerator(lexTokens);
            StatementList statements = astGenerator.ParseStatements(new CallStatement());

            

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
            Console.WriteLine("Original;");
            Console.WriteLine(prettyPrinter.Print(statements));

            Console.ReadLine();
        }
    }
}
