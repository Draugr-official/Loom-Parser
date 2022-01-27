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

        void GenerateBinaryExpression(BinaryExpression binaryExpression)
        {
            GenerateExpression(binaryExpression.Left);

            switch (binaryExpression.Operator)
            {
                case BinaryOperators.Addition: scriptBuilder.Append(" + "); break;
                case BinaryOperators.Subtraction: scriptBuilder.Append(" - "); break;
                case BinaryOperators.Multiplication: scriptBuilder.Append(" * "); break;
                case BinaryOperators.Division: scriptBuilder.Append(" / "); break;
                case BinaryOperators.Modulus: scriptBuilder.Append(" % "); break;
                case BinaryOperators.Exponentiation: scriptBuilder.Append(" ^ "); break;
            }

            GenerateExpression(binaryExpression.Right);
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

        void GenerateVariableExpression(VariableExpression variableExpression)
        {
            scriptBuilder.Append(variableExpression.Name);
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
        void GenerateFunctionCallExpression(FunctionCallExpression functionCallExpression)
        {
            scriptBuilder.Append(functionCallExpression.Name + "(");
            GenerateExpressions(functionCallExpression.Arguments);
            scriptBuilder.Append(")");
        }

        void GenerateExpression(Expression expression)
        {
            if(expression is ConstantExpression constantExpression)
            {
                GenerateConstantExpression(constantExpression);
            }
            if(expression is VariableExpression variableExpression)
            {
                GenerateVariableExpression(variableExpression);
            }
            if(expression is RelationalExpression relationalExpression)
            {
                GenerateRelationalExpression(relationalExpression);
            }
            if(expression is BinaryExpression binaryExpression)
            {
                GenerateBinaryExpression(binaryExpression);
            }
            if(expression is ConcatExpression concatExpression)
            {
                GenerateConcatExpression(concatExpression);
            }
            if(expression is ArrayExpression arrayExpression)
            {
                GenerateArrayExpression(arrayExpression);
            }
            if(expression is FunctionCallExpression functionCallExpression)
            {
                GenerateFunctionCallExpression(functionCallExpression);
            }
            if(expression is IndexExpression indexExpression)
            {
                GenerateIndexExpression(indexExpression);
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
            if(statement is FunctionCallStatement functionCallStatement)
            {
                scriptBuilder.Append($"{indent + functionCallStatement.Name}(");
                GenerateExpressions(functionCallStatement.Arguments);
                scriptBuilder.Append(")");
                return true;
            }
            if (statement is FunctionDeclarationStatement functionDeclarationStatement)
            {
                scriptBuilder.Append($"{indent + (functionDeclarationStatement.IsLocal ? "local " : "")}function {functionDeclarationStatement.Name}(");
                GenerateExpressions(functionDeclarationStatement.Parameters);
                scriptBuilder.Append($")\n");;
                GenerateStatements(functionDeclarationStatement.Body, indent + PrinterSettings.Indentation);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if(statement is VariableAssignStatement variableAssignStatement)
            {
                scriptBuilder.Append($"{indent + (variableAssignStatement.IsLocal ? "local " : "") + variableAssignStatement.Name} = ");
                GenerateExpression(variableAssignStatement.Value);
                return true;
            }
            if(statement is IfStatement ifStatement)
            {
                scriptBuilder.Append($"{indent}if ");
                GenerateExpression(ifStatement.Condition);
                scriptBuilder.Append(" then\n");
                GenerateStatements(ifStatement.Body, indent + PrinterSettings.Indentation);
                scriptBuilder.Append("end");
                return true;
            }
            if (statement is WhileStatement whileStatement)
            {
                scriptBuilder.Append($"{indent}while ");
                GenerateExpression(whileStatement.Condition);
                scriptBuilder.Append(" do\n");
                GenerateStatements(whileStatement.Body, indent + PrinterSettings.Indentation);
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

        void GenerateStatements(StatementList statements, string indent)
        {
            foreach(Statement statement in statements)
            {
                if (GenerateStatement(statement, indent))
                {
                    scriptBuilder.Append("\n");
                }
            }
        }

        public string Beautify(StatementList statements)
        {
            scriptBuilder.Clear();
            GenerateStatements(statements, "");
            return scriptBuilder.ToString();
        }
    }
}
