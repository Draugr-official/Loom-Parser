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
    class ASTGenerator
    {
        /// <summary>
        /// The token reader of the AST generator
        /// </summary>
        TokenReader tokenReader { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="lexTokens"></param>
        public ASTGenerator(LexTokenList lexTokens)
        {
            this.tokenReader = new TokenReader(lexTokens);
        }

        /* Expressions */

        bool ParseIndexExpression(Expression array, out IndexExpression indexExpression)
        {
            indexExpression = new IndexExpression();
            indexExpression.Array = array;

            if(tokenReader.Expect(LexKind.BracketOpen))
            {
                tokenReader.Skip(1);
                if(ParseExpression(out Expression index))
                {
                    indexExpression.Index = index;
                    if(tokenReader.Expect(LexKind.BracketClose))
                    {
                        tokenReader.Skip(1);
                        return true;
                    }
                }
            }

            return false;
        }

        bool ParseArrayExpression(out ArrayExpression arrayExpression)
        {
            arrayExpression = new ArrayExpression();

            if(tokenReader.Expect(LexKind.BraceOpen))
            {
                arrayExpression.Array = ParseExpressions();
                if(tokenReader.ExpectFatal(LexKind.BraceClose))
                {
                    tokenReader.Skip(1);
                    return true;
                }
            }

            return false;
        }

        bool ParseCallExpression(Expression left, out CallExpression callExpression)
        {
            callExpression = new CallExpression();
            callExpression.Operand = left;

            if (tokenReader.Expect(LexKind.ParentheseOpen))
            {
                tokenReader.Skip(1);
                callExpression.Arguments = ParseExpressions();
                if(tokenReader.ExpectFatal(LexKind.ParentheseClose))
                {
                    tokenReader.Skip(1);
                    return true;
                }
            }

            return false; 
        }

        bool ParseConcatExpression(Expression left, out ConcatExpression concatExpression)
        {
            concatExpression = new ConcatExpression();
            concatExpression.Left = left;

            switch (tokenReader.Peek().Kind)
            {
                case LexKind.Concat:
                    {
                        tokenReader.Skip(1);
                        if (ParseExpression(out Expression right))
                        {
                            concatExpression.Right = right;
                            return true;
                        }
                        break;
                    }
            }

            return false;
        }

        bool ParseBinaryExpression(Expression left, out BinaryExpression binaryExpression)
        {
            binaryExpression = new BinaryExpression();
            binaryExpression.Left = left;

            BinaryOperators binaryOperator = BinaryOperators.Unknown;

            switch (tokenReader.Peek().Kind)
            {
                case LexKind.Add: binaryOperator = BinaryOperators.Addition; break;
                case LexKind.Sub: binaryOperator = BinaryOperators.Subtraction; break;
                case LexKind.Mul: binaryOperator = BinaryOperators.Multiplication; break;
                case LexKind.Div: binaryOperator = BinaryOperators.Division; break;
                case LexKind.Mod: binaryOperator = BinaryOperators.Modulus; break;
                case LexKind.Exp: binaryOperator = BinaryOperators.Exponentiation; break;
            }
            
            if(binaryOperator != BinaryOperators.Unknown)
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression right))
                {
                    binaryExpression.Right = right;
                    binaryExpression.Operator = binaryOperator;
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

        bool ParseIdentifierExpression(out IdentifierExpression identifierExpression)
        {
            identifierExpression = new IdentifierExpression();

            if(tokenReader.Expect(LexKind.Identifier))
            {
                identifierExpression.Identifier = tokenReader.Consume().Value;
                return true;
            }

            return false;
        }

        bool ParseExpression(out Expression expression)
        {
            expression = null;

            if(ParseArrayExpression(out ArrayExpression tableExpression))
            {
                expression = tableExpression;
            }
            if (ParseConstantExpression(out ConstantExpression constantExpression))
            {
                expression = constantExpression;
            }
            if (ParseIdentifierExpression(out IdentifierExpression identifierExpression))
            {
                expression = identifierExpression;
            }

            // Expression with left side expressions
            if (ParseIndexExpression(expression, out IndexExpression indexExpression))
            {
                expression = indexExpression;
            }

            if (ParseCallExpression(expression, out CallExpression callExpression))
            {
                expression = callExpression;
            }

            // Expressions with left and right side expressions
            if (ParseRelationalExpression(expression, out RelationalExpression relationalExpression))
            {
                expression = relationalExpression;
            }

            if (ParseBinaryExpression(expression, out BinaryExpression binaryExpression))
            {
                expression = binaryExpression;
            }

            if (ParseConcatExpression(expression, out ConcatExpression concatExpression))
            {
                expression = concatExpression;
            }

            return expression != null;
        }

        ExpressionList ParseExpressions()
        {
            ExpressionList expressionList = new ExpressionList();

            for(; ; )
            {
                if (tokenReader.Expect(LexKind.Comma))
                {
                    tokenReader.Skip(1);
                }

                if (ParseExpression(out Expression expression))
                {
                    expressionList.Add(expression);
                }
                else
                {
                    tokenReader.Skip(1);
                    break;
                }
            }

            return expressionList;
        }

        /* Statements */

        bool ParseReturnStatement(out ReturnStatement returnStatement)
        {
            returnStatement = new ReturnStatement();

            if(tokenReader.Expect(LexKind.Keyword, "return"))
            {
                tokenReader.Skip(1);
                if(ParseExpression(out Expression returnExpression))
                {
                    returnStatement.ReturnValue = returnExpression;
                    return true;
                }
            }

            return false;
        }

        bool ParseWhileStatement(out WhileStatement whileStatement)
        {
            whileStatement = new WhileStatement();

            if (tokenReader.Expect(LexKind.Keyword, "while"))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression condition))
                {
                    whileStatement.Condition = condition;
                    if (tokenReader.Expect(LexKind.Keyword, "do"))
                    {
                        tokenReader.Skip(1);
                        whileStatement.Body = ParseStatements(whileStatement);
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
                        ifStatement.Body = ParseStatements(ifStatement);
                        if(tokenReader.Expect(LexKind.Keyword, "end"))
                        {
                            tokenReader.Skip(1);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        bool ParseVariableAssignStatement(Statement parent, out VariableAssignStatement variableAssignStatement)
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
                        tokenReader.Skip(3 + localOffset);
                        functionDeclarationStatement.Parameters = ParseExpressions();
                        functionDeclarationStatement.Body = ParseStatements(functionDeclarationStatement);

                        if (tokenReader.Expect(LexKind.Keyword, "end"))
                        {
                            tokenReader.Skip(1);
                            return true;
                        }
                    }
                }
                else if (tokenReader.Expect(LexKind.ParentheseOpen, 1 + localOffset))
                {
                    tokenReader.Skip(2 + localOffset);
                    functionDeclarationStatement.Parameters = ParseExpressions();
                    functionDeclarationStatement.Body = ParseStatements(functionDeclarationStatement);

                    if (tokenReader.Expect(LexKind.Keyword, "end"))
                    {
                        tokenReader.Skip(1);
                        return true;
                    }
                }
            }

            return false;
        }

        bool ParseCallStatement(out CallStatement callStatement)
        {
            callStatement = new CallStatement();

            if (tokenReader.Expect(LexKind.Identifier))
            {
                if (tokenReader.Expect(LexKind.ParentheseOpen, 1))
                {
                    callStatement.Name = tokenReader.Peek().Value;
                    tokenReader.Skip(2);
                    callStatement.Arguments = ParseExpressions();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a statement at the given position in the token reader
        /// </summary>
        bool ParseStatement(Statement parent, out Statement statement)
        {
            statement = new Statement();
            if (ParseCallStatement(out CallStatement callStatement))
            {
                statement = callStatement;
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
            if(ParseWhileStatement(out WhileStatement whileStatement))
            {
                statement = whileStatement;
                return true;
            }
            if(ParseReturnStatement(out ReturnStatement returnStatement))
            {
                statement = returnStatement;
                return true;
            }
            if(ParseVariableAssignStatement(parent, out VariableAssignStatement variableAssignStatement))
            {
                statement = variableAssignStatement;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates the statements with the tokens from the token reader
        /// </summary>
        public StatementList ParseStatements(Statement parent)
        {
            StatementList statements = new StatementList();

            for(int i = 0; i < tokenReader.LexTokens.Count; i++)
            {
                if (ParseStatement(parent, out Statement statement))
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
