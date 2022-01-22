using Loom_Parser.Parser.ASTGen.AST.Statements;
using Loom_Parser.Parser.ASTGen.AST.Expressions;
using Loom_Parser.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom_Parser.Parser.ASTGen.AST;

namespace Loom_Parser.Parser.ASTGen
{
    /// <summary>
    /// Abstract syntax tree generator
    /// </summary>
    class CodeGenerator
    {
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
            this.tokenReader = new TokenReader(lexTokens);
        }

        bool ParseFunctionCallExpression(out FunctionCallExpression functionCallExpression)
        {
            functionCallExpression = new FunctionCallExpression();

            if(tokenReader.Expect(LexKind.Identifier))
            {
                functionCallExpression.Name = tokenReader.Peek().Value;
                if(tokenReader.Expect(LexKind.ParentheseOpen, 1))
                {
                    tokenReader.Skip(2);
                    functionCallExpression.Arguments = ParseExpressions(); 
                    return true;
                }
            }

            return false;
        }

        bool ParseRelationalExpression(Expression left, out RelationalExpression relationalExpression)
        {
            relationalExpression = new RelationalExpression();
            relationalExpression.Left = left;

            switch(tokenReader.Peek().Kind)
            {
                case LexKind.EqualTo:
                case LexKind.NotEqualTo:
                case LexKind.BiggerOrEqual:
                case LexKind.SmallerOrEqual:
                case LexKind.ChevronOpen:
                case LexKind.ChevronClose:
                    {
                        tokenReader.Skip(1);
                        if (ParseExpression(out Expression right))
                        {
                            relationalExpression.Right = right;
                            return true;
                        }
                        break;
                    }
            }

            return false;
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

        bool ParseVariableExpression(out VariableExpression variableExpression)
        {
            variableExpression = new VariableExpression();

            if(tokenReader.Expect(LexKind.Identifier))
            {
                variableExpression.Name = tokenReader.Consume().Value;
                return true;
            }

            return false;
        }

        bool ParseExpression(out Expression expression)
        {
            expression = new Expression();
            bool parsedLayer = false;

            if (!parsedLayer && ParseFunctionCallExpression(out FunctionCallExpression functionCallExpression))
            {
                expression = functionCallExpression;
                parsedLayer = true;
            }
            if (!parsedLayer && ParseVariableExpression(out VariableExpression variableExpression))
            {
                expression = variableExpression;
                parsedLayer = true;
            }
            if (!parsedLayer && ParseConstantExpression(out ConstantExpression constantExpression))
            {
                expression = constantExpression;
                parsedLayer = true;
            }

            // Expressions with left expressions
            if(parsedLayer && ParseRelationalExpression(expression, out RelationalExpression relationalExpression))
            {
                expression = relationalExpression;
                return true;
            }

            return parsedLayer;
        }

        ExpressionList ParseExpressions()
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

                if (ParseExpression(out Expression expression))
                {
                    expressionList.Add(expression);
                }
                else
                {
                    tokenReader.Skip(1);
                }
            }
            return expressionList;
        }

        bool ParseIfStatement(out IfStatement ifStatement)
        {
            ifStatement = new IfStatement();

            if (tokenReader.Expect(LexKind.Keyword, "if"))
            {
                tokenReader.Skip(1);
                if(ParseExpression(out Expression condition))
                {
                    ifStatement.Condition = condition;
                    if(tokenReader.Expect(LexKind.Keyword, "then"))
                    {
                        tokenReader.Skip(1);
                        ifStatement.Body = ParseStatements();
                        if(tokenReader.Expect(LexKind.Keyword, "end"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        bool ParseVariableAssignStatement(out VariableAssignStatement variableAssignStatement)
        {
            variableAssignStatement = new VariableAssignStatement();

            int localOffset = 0;

            if(tokenReader.Expect(LexKind.Keyword, "local"))
            {
                variableAssignStatement.IsLocal = true;
                localOffset = 1;
            }

            if (tokenReader.Expect(LexKind.Identifier, localOffset))
            {
                variableAssignStatement.Name = tokenReader.Peek(localOffset).Value;
                if(tokenReader.Expect(LexKind.Equals, 1 + localOffset))
                {
                    tokenReader.Skip(2 + localOffset);
                    if(ParseExpression(out Expression value))
                    {
                        variableAssignStatement.Value = value;
                        return true;
                    }
                }
            }

            return false;
        }

        bool ParseFunctionDeclarationStatement(out FunctionDeclarationStatement functionDeclarationStatement)
        {
            functionDeclarationStatement = new FunctionDeclarationStatement();

            int localOffset = 0;

            if(tokenReader.Expect(LexKind.Keyword, "local"))
            {
                functionDeclarationStatement.IsLocal = true;
                localOffset = 1;
            }

            if(tokenReader.Expect(LexKind.Keyword, "function", localOffset))
            {
                if (tokenReader.Expect(LexKind.Identifier, 1 + localOffset))
                {
                    functionDeclarationStatement.Name = tokenReader.Peek(1 + localOffset).Value;
                    if(tokenReader.Expect(LexKind.ParentheseOpen, 2 + localOffset))
                    {
                        functionDeclarationStatement.IsLocal = tokenReader.Expect(LexKind.Keyword, "local", -1 + localOffset);
                        tokenReader.Skip(3 + localOffset);
                        functionDeclarationStatement.Parameters = ParseExpressions();
                        functionDeclarationStatement.Body = ParseStatements();

                        if (tokenReader.Expect(LexKind.Keyword, "end"))
                        {
                            tokenReader.Skip(1);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        bool ParseFunctionCallStatement(out FunctionCallStatement functionCallStatement)
        {
            functionCallStatement = new FunctionCallStatement();

            if (tokenReader.Expect(LexKind.Identifier))
            {
                functionCallStatement.Name = tokenReader.Peek().Value;

                if(tokenReader.Expect(LexKind.ParentheseOpen, 1))
                {
                    tokenReader.Skip(2);
                    functionCallStatement.Arguments = ParseExpressions();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a statement at the given position in the token reader
        /// </summary>
        bool ParseStatement(out Statement statement)
        {
            statement = new Statement();
            if (ParseFunctionCallStatement(out FunctionCallStatement functionCallStatement))
            {
                statement = functionCallStatement;
                return true;
            }
            if(ParseFunctionDeclarationStatement(out FunctionDeclarationStatement functionDeclarationStatement))
            {
                statement = functionDeclarationStatement;
                return true;
            }
            if(ParseIfStatement(out IfStatement ifStatement))
            {
                statement = ifStatement;
                return true;
            }
            if(ParseVariableAssignStatement(out VariableAssignStatement variableAssignStatement))
            {
                statement = variableAssignStatement;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates the statements with the tokens from the token reader
        /// </summary>
        public StatementList ParseStatements()
        {
            StatementList statements = new StatementList();

            for(int i = 0; i < tokenReader.LexTokens.Count; i++)
            {
                if (ParseStatement(out Statement statement))
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
