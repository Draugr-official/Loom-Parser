using Loom_Parser.Parser.Lexer.ASTGen.AST.Statements;
using Loom_Parser.Parser.Lexer.ASTGen.AST.Expressions;
using Loom_Parser.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom_Parser.Parser.Lexer.ASTGen.AST;

namespace Loom_Parser.Parser.Lexer.ASTGen
{
    /// <summary>
    /// Abstract syntax tree generator
    /// </summary>
    class CodeGenerator
    {
        /// <summary>
        /// A list of lexical tokens used to generate an abstract syntax tree
        /// </summary>
        LexTokenList LexTokens { get; set; }

        /// <summary>
        /// The token reader of the AST generator
        /// </summary>
        TokenReader tokenReader { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="lexTokens"></param>
        public CodeGenerator(LexTokenList lexTokens)
        {
            this.LexTokens = lexTokens;
            this.tokenReader = new TokenReader(lexTokens);
        }

        bool ParseConstantExpression(out ConstantExpression constantExpression)
        {
            constantExpression = new ConstantExpression();

            if(tokenReader.Peek().Kind == LexKind.String
                || tokenReader.Peek().Kind == LexKind.Boolean
                || tokenReader.Peek().Kind == LexKind.Number)
            {
                switch (tokenReader.Peek().Kind)
                {
                    case LexKind.String:
                        {
                            constantExpression.Type = DataTypes.String;
                            break;
                        }
                    case LexKind.Boolean:
                        {
                            constantExpression.Type = DataTypes.Bool;
                            break;
                        }
                    case LexKind.Number:
                        {
                            constantExpression.Type = DataTypes.Number;
                            break;
                        }
                }

                constantExpression.Value = tokenReader.Consume().Value;

                return true;
            }

            return false;
        }

        bool GenerateExpression(out Expression expression)
        {
            expression = new Expression();

            if(ParseConstantExpression(out ConstantExpression constantExpression))
            {
                expression = constantExpression;
                return true;
            }

            return false;
        }

        ExpressionList GenerateExpressions()
        {
            ExpressionList expressionList = new ExpressionList();
            int Scope = 1;

            while (Scope > 0)
            {
                if (tokenReader.Expect(LexKind.ParentheseClose))
                {
                    Scope--;
                }
                else if (tokenReader.Expect(LexKind.ParentheseOpen))
                {
                    Scope++;
                }

                if (GenerateExpression(out Expression expression))
                {
                    expressionList.Add(expression);
                }
            }
            return expressionList;
        }

        bool ParseFunctionCallStatement(out FunctionCallStatement functionCallStatement)
        {
            functionCallStatement = new FunctionCallStatement();

            if (tokenReader.Peek().Kind == LexKind.Identifier)
            {
                functionCallStatement.Name = tokenReader.Consume().Value;

                if(tokenReader.Peek().Kind == LexKind.ParentheseOpen)
                {
                    tokenReader.Consume();
                    functionCallStatement.Arguments = GenerateExpressions();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates a statement at the given position in the token reader
        /// </summary>
        bool GenerateStatement(out Statement statement)
        {
            statement = new Statement();
            if (ParseFunctionCallStatement(out FunctionCallStatement functionCallStatement))
            {
                statement = functionCallStatement;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates the statements with the tokens from the token reader
        /// </summary>
        public StatementList GenerateStatements()
        {
            StatementList statements = new StatementList();

            for(int i = 0; i < tokenReader.LexTokens.Count; i++)
            {
                if (GenerateStatement(out Statement statement))
                {
                    statements.Add(statement);
                }

                if(tokenReader.Expect(LexKind.EOF))
                {
                    break;
                }
            }

            return statements;
        }
    }
}
