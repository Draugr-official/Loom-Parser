using Loom.Parser.ASTGenerator.AST;
using Loom.Parser.ASTGenerator.AST.Expressions;
using Loom.Parser.ASTGenerator.AST.Statements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.PrettyPrint
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

        void GenerateGroupedExpression(GroupedExpression groupedExpression, string indent = "", string newLine = " ")
        {
            scriptBuilder.Append("(");
            GenerateExpression(groupedExpression.Expression, indent, newLine);
            scriptBuilder.Append(")");
        }

        void GenerateArrayExpression(ArrayExpression arrayExpression, string indent = "", string newLine = " ")
        {
            scriptBuilder.Append($"{{{newLine}");
            foreach(Expression expression in arrayExpression.Array)
            {
                GenerateExpression(expression, indent, newLine);
                
                if(arrayExpression.Array.IndexOf(expression) != arrayExpression.Array.Count - 1)
                {
                    scriptBuilder.Append($",{newLine}");
                }
            }
            scriptBuilder.Append($"{indent}{newLine}}}");
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

        void GenerateLogicalExpression(LogicalExpression logicalExpression)
        {
            GenerateExpression(logicalExpression.Left);

            switch (logicalExpression.Operator)
            {
                case LogicalOperators.And: scriptBuilder.Append(" and "); break;
                case LogicalOperators.Or: scriptBuilder.Append(" or "); break;
                case LogicalOperators.Not: scriptBuilder.Append(" not "); break;
            }

            GenerateExpression(logicalExpression.Right);
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
                return;
            }

            scriptBuilder.Append(constantExpression.Value);
        }
        void GenerateCallExpression(CallExpression callExpression, string indent = "", string newLine = " ")
        {
            GenerateExpression(callExpression.Operand);
            scriptBuilder.Append("(");
            GenerateExpressions(callExpression.Arguments, indent, newLine);
            scriptBuilder.Append(")");
        }

        void GenerateFunctionDeclarationExpression(FunctionDeclarationExpression functionDeclarationExpression, string indent, string newLine = " ")
        {
            scriptBuilder.Append($"function{(functionDeclarationExpression.IsAnonymous ? "" : $" {functionDeclarationExpression.Name.Identifier}")}(");
            GenerateExpressions(functionDeclarationExpression.Parameters);
            scriptBuilder.Append($"){PrinterSettings.NewLine}");
            GenerateStatements(functionDeclarationExpression.Body, indent + PrinterSettings.Indentation, newLine);
            scriptBuilder.Append($"{indent}end");
        }

        void GenerateAssignmentExpression(AssignmentExpression assignmentExpression, string indent = "", string newLine = " ")
        {
            scriptBuilder.Append(indent);
            GenerateExpressions(assignmentExpression.Variables);
            scriptBuilder.Append(" = ");
            GenerateExpressions(assignmentExpression.Values);
        }

        void GenerateIfExpression(IfExpression ifExpression)
        {
            scriptBuilder.Append($"if ");
            GenerateExpression(ifExpression.Condition);
            scriptBuilder.Append($" then ");
            GenerateExpression(ifExpression.Body);

            foreach (IfExpression elseIfExpression in ifExpression.ElseIfExpressions)
            {
                scriptBuilder.Append($" elseif ");
                GenerateExpression(elseIfExpression.Condition);
                scriptBuilder.Append($" then ");
                GenerateExpression(elseIfExpression.Body);
            }

            if (ifExpression.ElseExpression != null)
            {
                scriptBuilder.Append($" else ");
                GenerateExpression(ifExpression.ElseExpression);
            }
        }

        void GenerateVarargExpression(VarargExpression varargExpression)
        {
            scriptBuilder.Append("...");
        }

        void GenerateLengthExpression(LengthExpression lengthExpression)
        {
            scriptBuilder.Append("#");
            GenerateExpression(lengthExpression.Identifier);
        }
        void GenerateNegativeExpression(NegativeExpression negativeExpression)
        {
            scriptBuilder.Append("-");
            GenerateExpression(negativeExpression.Identifier);
        }
        void GenerateNilExpression(NilExpression nilExpression)
        {
            scriptBuilder.Append("nil");
        }

        void GenerateExpression(Expression expression, string indent = "", string newLine = " ")
        {
            if(expression is ConstantExpression constantExpression)
            {
                GenerateConstantExpression(constantExpression);
            }
            if(expression is IdentifierExpression variableExpression)
            {
                GenerateIdentifierExpression(variableExpression);
            }
            if(expression is VarargExpression varargExpression)
            {
                GenerateVarargExpression(varargExpression);
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
                GenerateArrayExpression(arrayExpression, indent, newLine);
            }
            if(expression is GroupedExpression groupedExpression)
            {
                GenerateGroupedExpression(groupedExpression, indent, newLine);
            }
            if(expression is CallExpression functionCallExpression)
            {
                GenerateCallExpression(functionCallExpression, indent, newLine);
            }
            if(expression is IndexExpression indexExpression)
            {
                GenerateIndexExpression(indexExpression);
            }
            if(expression is FunctionDeclarationExpression functionDeclarationExpression)
            {
                GenerateFunctionDeclarationExpression(functionDeclarationExpression, indent, newLine);
            }
            if(expression is AssignmentExpression assignmentExpression)
            {
                GenerateAssignmentExpression(assignmentExpression, indent);
            }
            if(expression is IfExpression ifExpression)
            {
                GenerateIfExpression(ifExpression);
            }
            if(expression is LengthExpression lenExpression)
            {
                GenerateLengthExpression(lenExpression);
            }
            if(expression is NegativeExpression negativeExpression)
            {
                GenerateNegativeExpression(negativeExpression);
            }
            if(expression is LogicalExpression logicalExpression)
            {
                GenerateLogicalExpression(logicalExpression);
            }
            if(expression is NilExpression nilExpression)
            {
                GenerateNilExpression(nilExpression);
            }
        }

        void GenerateExpressions(ExpressionList expressions, string indent = "", string newLine = " ")
        {
            if(expressions.Count > 0)
            {
                for (int i = 0; i < expressions.Count - 1; i++)
                {
                    GenerateExpression(expressions[i], indent, newLine);
                    scriptBuilder.Append(", ");
                }

                GenerateExpression(expressions[expressions.Count - 1], indent, newLine);
            }
        }

        bool GenerateStatement(Statement statement, string indent)
        {
            if(statement is CallStatement callStatement)
            {
                scriptBuilder.Append(indent);
                GenerateExpression(callStatement.Function);
                scriptBuilder.Append("(");
                GenerateExpressions(callStatement.Arguments, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($")");
                return true;
            }
            if (statement is FunctionDeclarationStatement functionDeclarationStatement)
            {
                scriptBuilder.Append($"{indent + (functionDeclarationStatement.IsLocal ? "local " : "")}function {functionDeclarationStatement.Name.Identifier}(");
                GenerateExpressions(functionDeclarationStatement.Parameters);
                scriptBuilder.Append($"){PrinterSettings.NewLine}");
                GenerateStatements(functionDeclarationStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if(statement is AssignmentStatement assignmentStatement)
            {
                scriptBuilder.Append($"{indent}");
                GenerateExpressions(assignmentStatement.Variables);
                scriptBuilder.Append(" = ");
                GenerateExpressions(assignmentStatement.Values, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                return true;
            }
            if(statement is IfStatement ifStatement)
            {
                scriptBuilder.Append($"{indent}if ");
                GenerateExpression(ifStatement.Condition);
                scriptBuilder.Append($" then{PrinterSettings.NewLine}");
                GenerateStatements(ifStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);

                foreach(IfStatement elseIfStatement in ifStatement.ElseIfStatements)
                {
                    scriptBuilder.Append($"{indent}elseif ");
                    GenerateExpression(elseIfStatement.Condition);
                    scriptBuilder.Append($" then{PrinterSettings.NewLine}");
                    GenerateStatements(elseIfStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                }

                if(ifStatement.ElseStatements.Count > 0)
                {
                    scriptBuilder.Append($"{indent}else{PrinterSettings.NewLine}");
                    GenerateStatements(ifStatement.ElseStatements, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                }

                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if (statement is GenericForStatement genericForStatement)
            {
                scriptBuilder.Append($"{indent}for ");

                foreach (Expression variableExpression in genericForStatement.VariableArray.Array)
                {
                    GenerateExpression(variableExpression);

                    if (genericForStatement.VariableArray.Array.IndexOf(variableExpression) != genericForStatement.VariableArray.Array.Count - 1)
                    {
                        scriptBuilder.Append(", ");
                    }
                }


                scriptBuilder.Append($" in ");
                GenerateExpression(genericForStatement.Iterator);
                scriptBuilder.Append($" do{PrinterSettings.NewLine}");
                GenerateStatements(genericForStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if (statement is ForStatement forStatement)
            {
                scriptBuilder.Append($"{indent}for ");
                GenerateExpression(forStatement.ControlVariable);
                scriptBuilder.Append($", ");
                GenerateExpression(forStatement.EndValue);
                
                if(forStatement.Increment != null)
                {
                    scriptBuilder.Append($", ");
                    GenerateExpression(forStatement.Increment);
                }

                scriptBuilder.Append($" do{PrinterSettings.NewLine}");
                GenerateStatements(forStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if(statement is RepeatStatement repeatStatement)
            {
                scriptBuilder.Append($"{indent}repeat ");
                GenerateStatements(repeatStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}until ");
                GenerateExpression(repeatStatement.Condition, indent, PrinterSettings.NewLine);
                return true;
            }
            if(statement is DoStatement doStatement)
            {
                scriptBuilder.Append($"{indent}do{PrinterSettings.NewLine}");
                GenerateStatements(doStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if (statement is BreakStatement)
            {
                scriptBuilder.Append($"{indent}break");
                return true;
            }
            if (statement is ContinueStatement)
            {
                scriptBuilder.Append($"{indent}continue");
                return true;
            }
            if (statement is WhileStatement whileStatement)
            {
                scriptBuilder.Append($"{indent}while ");
                GenerateExpression(whileStatement.Condition);
                scriptBuilder.Append($" do{PrinterSettings.NewLine}");
                GenerateStatements(whileStatement.Body, indent + PrinterSettings.Indentation, PrinterSettings.NewLine);
                scriptBuilder.Append($"{indent}end");
                return true;
            }
            if(statement is ReturnStatement returnStatement)
            {
                scriptBuilder.Append($"{indent}return ");
                GenerateExpression(returnStatement.ReturnValue, indent, PrinterSettings.NewLine);
                return true;
            }
            if(statement is LocalDeclarationStatement localDeclarationStatement)
            {
                scriptBuilder.Append($"{indent}local ");
                GenerateStatement(localDeclarationStatement.Statement, "");
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
