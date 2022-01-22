using Loom_Parser.Parser.ASTGen.AST;
using Loom_Parser.Parser.ASTGen.AST.Expressions;
using Loom_Parser.Parser.ASTGen.AST.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.Beautifier
{
    /// <summary>
    /// Convert code from AST to its code representation
    /// </summary>
    class CodeBeautifier
    {
        /// <summary>
        /// The stringbuilder utilized by the code beautifier
        /// </summary>
        StringBuilder scriptBuilder = new StringBuilder();

        /// <summary>
        /// The indentation applied to the current scope
        /// </summary>
        string Indentation = "";

        /// <summary>
        /// The statements being converted
        /// </summary>
        StatementList Statements { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="statements"></param>
        public CodeBeautifier(StatementList statements)
        {
            this.Statements = statements;
        }

        string GenerateConstantExpression(ConstantExpression constantExpression)
        {
            if (constantExpression.Type == DataTypes.String)
            {
                return $"'{constantExpression.Value}'";
            }
            else
            {
                return constantExpression.Value;
            }
        }

        string GenerateExpression(Expression expression)
        {
            if(expression is ConstantExpression constantExpression)
            {
                return GenerateConstantExpression(constantExpression);
            }

            return "nil";
        }

        void GenerateExpressions(ExpressionList expressions)
        {
            List<string> expressionsList = new List<string>();

            foreach(Expression expression in expressions)
            {
                expressionsList.Add(GenerateExpression(expression));
            }

            scriptBuilder.Append(string.Join(", ", expressionsList));
        }

        bool GenerateStatement(Statement statement)
        {
            if(statement is FunctionCallStatement functionCallStatement)
            {
                scriptBuilder.Append($"{this.Indentation + functionCallStatement.Name}(");
                GenerateExpressions(functionCallStatement.Arguments);
                scriptBuilder.Append(")");
                return true;
            }
            if (statement is FunctionDeclarationStatement functionDeclarationStatement)
            {
                scriptBuilder.Append($"{this.Indentation + functionDeclarationStatement.Name}(");
                GenerateExpressions(functionDeclarationStatement.Parameters);
                scriptBuilder.Append(")");
                GenerateStatements(functionDeclarationStatement.Body);
                scriptBuilder.Append("end");
                return true;
            }

            return false;
        }

        void GenerateStatements(StatementList statements)
        {
            foreach(Statement statement in statements)
            {
                if (GenerateStatement(statement))
                {
                    scriptBuilder.Append("\n");
                }
            }
        }

        public string Beautify()
        {
            scriptBuilder.Clear();
            GenerateStatements(this.Statements);
            return scriptBuilder.ToString();
        }
    }
}
