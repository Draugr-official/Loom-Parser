using Loom_Parser.Parser.ASTGen.AST;
using Loom_Parser.Parser.ASTGen.AST.Expressions;
using Loom_Parser.Parser.ASTGen.AST.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.PrettyPrint
{
    /// <summary>
    /// Convert code from AST to its code representation
    /// </summary>
    class PrettyPrinter
    {
        /// <summary>
        /// The settings for the current prettyprinter
        /// </summary>
        PrettyPrinterSettings PrinterSettings { get; set; }

        /// <summary>
        /// The stringbuilder utilized by the code beautifier
        /// </summary>
        readonly StringBuilder scriptBuilder = new StringBuilder();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="statements"></param>
        public PrettyPrinter(PrettyPrinterSettings settings)
        {
            this.PrinterSettings = settings;
        }

        void GenerateIndexExpression(IndexExpression indexExpression)
        {
            GenerateExpression(indexExpression.Array);
            scriptBuilder.Append("[");
            GenerateExpression(indexExpression.Index);
            scriptBuilder.Append("]");
        }

        void GenerateArrayExpression(ArrayExpression arrayExpression)
        {
            scriptBuilder.Append("{ ");
            foreach(Expression expression in arrayExpression.Array)
            {
                GenerateExpression(expression);
                
                if(arrayExpression.Array.IndexOf(expression) != arrayExpression.Array.Count - 1)
                {
                    scriptBuilder.Append(", ");
                }
            }
            scriptBuilder.Append(" }");
        }

        void GenerateConcatExpression(ConcatExpression concatExpression)
        {
            GenerateExpression(concatExpression.Left);
            scriptBuilder.Append(" .. ");
            GenerateExpression(concatExpression.Right);
        }

        void GenerateArithmeticExpression(ArithmeticExpression arithmeticExpression)
        {
            GenerateExpression(arithmeticExpression.Left);

            switch (arithmeticExpression.Operator)
            {
                case ArithmeticOperators.Addition: scriptBuilder.Append(" + "); break;
                case ArithmeticOperators.Subtraction: scriptBuilder.Append(" - "); break;
                case ArithmeticOperators.Multiplication: scriptBuilder.Append(" * "); break;
                case ArithmeticOperators.Division: scriptBuilder.Append(" / "); break;
                case ArithmeticOperators.Modulus: scriptBuilder.Append(" % "); break;
                case ArithmeticOperators.Exponentiation: scriptBuilder.Append(" ^ "); break;
            }

            GenerateExpression(arithmeticExpression.Right);
        }

        void GenerateRelationalExpression(RelationalExpression relationalExpression)
        {
            GenerateExpression(relationalExpression.Left);

            switch (relationalExpression.Operator)
            {
                case RelationalOperators.EqualTo: scriptBuilder.Append(" == "); break;
                case RelationalOperators.NotEqualTo: scriptBuilder.Append(" ~= "); break;
                case RelationalOperators.BiggerThan: scriptBuilder.Append(" > "); break;
                case RelationalOperators.SmallerThan: scriptBuilder.Append(" < "); break;
                case RelationalOperators.BiggerOrEqual: scriptBuilder.Append(" >= "); break;
                case RelationalOperators.SmallerOrEqual: scriptBuilder.Append(" <= "); break;
            }

            GenerateExpression(relationalExpression.Right);
        }

        void GenerateIdentifierExpression(IdentifierExpression identifierExpression)
        {
            scriptBuilder.Append(identifierExpression.Identifier);
        }

        void GenerateConstantExpression(ConstantExpression constantExpression)
        {
            if (constantExpression.Type == DataTypes.String)
            {
                scriptBuilder.Append($"'{constantExpression.Value}'");
            }
            else
            {
                scriptBuilder.Append(constantExpression.Value);
            }
        }
        void GenerateCallExpression(CallExpression callExpression)
        {
            GenerateExpression(callExpression.Operand);
            scriptBuilder.Append("(");
            GenerateExpressions(callExpression.Arguments);
            scriptBuilder.Append(")");
        }

        void GenerateFunctionDeclarationExpression(FunctionDeclarationExpression functionDeclarationExpression)
        {
            scriptBuilder.Append($"function{(functionDeclarationExpression.IsAnonymous ? "" : $" {functionDeclarationExpression.Name.Identifier}")}(");
            GenerateExpressions(functionDeclarationExpression.Parameters);
            scriptBuilder.Append($") ");
            GenerateStatements(functionDeclarationExpression.Body, " ", "");
            scriptBuilder.Append($" end");
        }

        void GenerateAssignmentExpression(AssignmentExpression assignmentExpression)
        {
            GenerateExpression(assignmentExpression.Variable);
            scriptBuilder.Append(" = ");
            GenerateExpression(assignmentExpression.Value);
        }

        void GenerateExpression(Expression expression)
        {
            if(expression is ConstantExpression constantExpression)
            {
                GenerateConstantExpression(constantExpression);
            }
            if(expression is IdentifierExpression variableExpression)
            {
                GenerateIdentifierExpression(variableExpression);
            }
            if(expression is RelationalExpression relationalExpression)
            {
                GenerateRelationalExpression(relationalExpression);
            }
            if(expression is ArithmeticExpression arithmeticExpression)
            {
                GenerateArithmeticExpression(arithmeticExpression);
            }
            if(expression is ConcatExpression concatExpression)
            {
                GenerateConcatExpression(concatExpression);
            }
            if(expression is ArrayExpression arrayExpression)
            {
                GenerateArrayExpression(arrayExpression);
            }
            if(expression is CallExpression functionCallExpression)
            {
                GenerateCallExpression(functionCallExpression);
            }
            if(expression is IndexExpression indexExpression)
            {
                GenerateIndexExpression(indexExpression);
            }
            if(expression is FunctionDeclarationExpression functionDeclarationExpression)
            {
                GenerateFunctionDeclarationExpression(functionDeclarationExpression);
            }
            if(expression is AssignmentExpression assignmentExpression)
            {
                GenerateAssignmentExpression(assignmentExpression);
            }
        }

        void GenerateExpressions(ExpressionList expressions)
        {
            if(expressions.Count > 0)
            {
                for (int i = 0; i < expressions.Count - 1; i++)
                {
                    GenerateExpression(expressions[i]);
                    scriptBuilder.Append(", ");
                }

                GenerateExpression(expressions[expressions.Count - 1]);
            }
        }

        bool GenerateStatement(Statement statement, string indent)
        {
            if(statement is CallStatement callStatement)
            {
                scriptBuilder.Append($"{indent + callStatement.Name}(");
                GenerateExpressions(callStatement.Arguments);
                scriptBuilder.Append(")");
                return true;
            }
            if (statement is FunctionDeclarationStatement functionDeclarationStatement)
            {
                scriptBuilder.Append($"{indent + (functionDeclarationStatement.IsLocal ? "local " : "")}function {functionDeclarationStatement.Name.Identifier}(");
                GenerateExpressions(functionDeclarationStatement.Parameters);
                scriptBuilder.Append($"){PrinterSettings.NewLine}");;
                GenerateStatements(functionDeclarationStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if(statement is AssignmentStatement assignmentStatement)
            {
                scriptBuilder.Append($"{indent + (assignmentStatement.IsLocal ? "local " : "") + assignmentStatement.Name} = ");
                GenerateExpression(assignmentStatement.Value);
                return true;
            }
            if(statement is IfStatement ifStatement)
            {
                scriptBuilder.Append($"{indent}if ");
                GenerateExpression(ifStatement.Condition);
                scriptBuilder.Append($" then{PrinterSettings.NewLine}");
                GenerateStatements(ifStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append("end");
                return true;
            }
            if (statement is WhileStatement whileStatement)
            {
                scriptBuilder.Append($"{indent}while ");
                GenerateExpression(whileStatement.Condition);
                scriptBuilder.Append($" do{PrinterSettings.NewLine}");
                GenerateStatements(whileStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append("end");
                return true;
            }
            if(statement is ReturnStatement returnStatement)
            {
                scriptBuilder.Append("return ");
                GenerateExpression(returnStatement.ReturnValue);
                return true;
            }

            return false;
        }

        void GenerateStatements(StatementList statements, string indent, string newLine)
        {
            foreach(Statement statement in statements)
            {
                if (GenerateStatement(statement, indent))
                {
                    scriptBuilder.Append(newLine);
                }
            }
        }

        public string Print(StatementList statements)
        {
            scriptBuilder.Clear();
            GenerateStatements(statements, "", PrinterSettings.NewLine);
            return scriptBuilder.ToString();
        }
    }
}
