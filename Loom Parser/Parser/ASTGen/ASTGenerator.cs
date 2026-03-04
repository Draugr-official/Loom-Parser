using Loom.Parser.ASTGen.AST.Statements;
using Loom.Parser.ASTGen.AST.Expressions;
using Loom.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom.Parser.ASTGen.AST;
using System.Runtime.InteropServices;

namespace Loom.Parser.ASTGen
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
                tokenReader.Skip(1);
                arrayExpression.Array = ParseExpressions();

                if (tokenReader.ExpectFatal(LexKind.BraceClose))
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

        bool ParseArithmeticExpression(Expression left, out ArithmeticExpression arithmeticExpression)
        {
            arithmeticExpression = new ArithmeticExpression();
            arithmeticExpression.Left = left;

            ArithmeticOperators binaryOperator = ArithmeticOperators.Unknown;

            switch (tokenReader.Peek().Kind)
            {
                case LexKind.Add: binaryOperator = ArithmeticOperators.Addition; break;
                case LexKind.Sub: binaryOperator = ArithmeticOperators.Subtraction; break;
                case LexKind.Mul: binaryOperator = ArithmeticOperators.Multiplication; break;
                case LexKind.Div: binaryOperator = ArithmeticOperators.Division; break;
                case LexKind.Mod: binaryOperator = ArithmeticOperators.Modulus; break;
                case LexKind.Exp: binaryOperator = ArithmeticOperators.Exponentiation; break;
            }
            
            if(binaryOperator != ArithmeticOperators.Unknown)
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression right))
                {
                    arithmeticExpression.Right = right;
                    arithmeticExpression.Operator = binaryOperator;
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
                        switch (tokenReader.Peek().Kind)
                        {
                            case LexKind.Equals: relationalExpression.Operator = RelationalOperators.EqualTo; break;
                            case LexKind.NotEqualTo: relationalExpression.Operator = RelationalOperators.NotEqualTo; break;
                            case LexKind.BiggerOrEqual: relationalExpression.Operator = RelationalOperators.BiggerOrEqual; break;
                            case LexKind.SmallerOrEqual: relationalExpression.Operator = RelationalOperators.SmallerOrEqual; break;
                            case LexKind.ChevronOpen: relationalExpression.Operator = RelationalOperators.SmallerThan; break;
                            case LexKind.ChevronClose: relationalExpression.Operator = RelationalOperators.BiggerThan; break;
                        }

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

        bool ParseFunctionDeclarationExpression(out FunctionDeclarationExpression functionDeclarationExpression)
        {
            functionDeclarationExpression = new FunctionDeclarationExpression();

            if(tokenReader.Expect(LexKind.Keyword) && tokenReader.Peek().Value == "function")
            {
                tokenReader.Skip(1);
                if(ParseIdentifierExpression(out IdentifierExpression identifierExpression))
                {
                    functionDeclarationExpression.Name = identifierExpression;
                    functionDeclarationExpression.IsAnonymous = false;
                }

                if (tokenReader.ExpectFatal(LexKind.ParentheseOpen))
                {
                    tokenReader.Skip(1);
                    functionDeclarationExpression.Parameters = ParseExpressions();
                    tokenReader.Skip(1);
                    functionDeclarationExpression.Body = ParseStatements();

                    if (tokenReader.Expect(LexKind.Keyword, "end"))
                    {
                        tokenReader.Skip(1);
                        return true;
                    }
                }
            }

            return false;
        }

        // TODO: Add all the assignment operators
        bool ParseAssignmentExpression(Expression left, out AssignmentExpression assignmentExpression)
        {
            assignmentExpression = new AssignmentExpression();
            assignmentExpression.Variable = left;

            switch (tokenReader.Peek().Kind)
            {
                case LexKind.Equals:
                    {
                        tokenReader.Skip(1);
                        if (ParseExpression(out Expression right))
                        {
                            assignmentExpression.Value = right;
                            return true;
                        }
                        break;
                    }
            }

            return false;
        }

        bool ParseIfExpression(out IfExpression ifExpression)
        {
            ifExpression = new IfExpression();

            if (tokenReader.Expect(LexKind.Keyword, "if"))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression condition))
                {
                    ifExpression.Condition = condition;
                    if (tokenReader.Expect(LexKind.Keyword, "then"))
                    {
                        tokenReader.Skip(1);
                        if(ParseExpression(out Expression expression))
                        {
                            ifExpression.Body = expression;
                        }

                        for (; ; )
                        {
                            if (tokenReader.Expect(LexKind.Keyword, "elseif"))
                            {
                                tokenReader.Skip(1);
                                IfExpression elseIfExpression = new IfExpression();

                                if (ParseExpression(out Expression elseIfConditionExpression))
                                {
                                    elseIfExpression.Condition = elseIfConditionExpression;
                                    if (tokenReader.Expect(LexKind.Keyword, "then"))
                                    {
                                        tokenReader.Skip(1);
                                        if (ParseExpression(out Expression elseIfBodyExpression))
                                        {
                                            elseIfExpression.Body = elseIfBodyExpression;
                                            ifExpression.ElseIfExpressions.Add(elseIfExpression);
                                        }
                                    }
                                }
                                continue;
                            }

                            if (tokenReader.Expect(LexKind.Keyword, "else"))
                            {
                                tokenReader.Skip(1);
                                if(ParseExpression(out Expression elseExpression))
                                {
                                    ifExpression.ElseExpression = elseExpression;
                                }
                                continue;
                            }

                            break;
                        }


                        return true;
                    }
                }
            }

            return false;
        }

        bool ParseVarargExpression(out VarargExpression varargExpression)
        {
            varargExpression = new VarargExpression();

            if(tokenReader.Expect(LexKind.Dot)
                && tokenReader.Expect(LexKind.Dot, 1)
                && tokenReader.Expect(LexKind.Dot, 2))
            {
                return true;
            }

            return false;
        }

        bool ParseLenExpression(out LenExpression lenExpression)
        {
            lenExpression = new LenExpression();

            if (tokenReader.Expect(LexKind.Hashtag))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression expression))
                {
                    lenExpression.Identifier = expression;
                    return true;
                }

                throw new Exception("Len expression missing identifier");
            }

            return false;
        }
        bool ParseNegativeExpression(out NegativeExpression negativeExpression)
        {
            negativeExpression = new NegativeExpression();

            if (tokenReader.Expect(LexKind.Sub))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression expression))
                {
                    negativeExpression.Identifier = expression;
                    return true;
                }

                throw new Exception("Negative expression missing identifier");
            }

            return false;
        }

        bool ParseExpression(out Expression expression)
        {
            expression = null;

            // Expressions that can not be directly included in another expression without modifying it
            if (ParseIfExpression(out IfExpression ifExpression))
            {
                expression = ifExpression;
                return true;
            }

            if (ParseFunctionDeclarationExpression(out FunctionDeclarationExpression functionDeclarationExpression))
            {
                expression = functionDeclarationExpression;
                return true;
            }

            // The rest

            if (ParseArrayExpression(out ArrayExpression tableExpression))
            {
                expression = tableExpression;
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

            if (ParseVarargExpression(out VarargExpression varargExpression))
            {
                expression = varargExpression;
            }
            if (ParseLenExpression(out LenExpression lenExpression))
            {
                expression = lenExpression;
            }
            if (ParseConstantExpression(out ConstantExpression constantExpression))
            {
                expression = constantExpression;
            }
            if(ParseNegativeExpression(out NegativeExpression negativeExpression))
            {
                expression = negativeExpression;
            }

            // Expressions with left and right side expressions
            if (ParseRelationalExpression(expression, out RelationalExpression relationalExpression))
            {
                expression = relationalExpression;
            }

            if (ParseArithmeticExpression(expression, out ArithmeticExpression arithmeticExpression))
            {
                expression = arithmeticExpression;
            }

            if (ParseConcatExpression(expression, out ConcatExpression concatExpression))
            {
                expression = concatExpression;
            }

            if(ParseAssignmentExpression(expression, out AssignmentExpression assignmentExpression))
            {

                expression = assignmentExpression;
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
                        whileStatement.Body = ParseStatements();
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
                        ifStatement.Body = ParseStatements();

                        for (; ; )
                        {
                            if (tokenReader.Expect(LexKind.Keyword, "elseif"))
                            {
                                tokenReader.Skip(1);
                                IfStatement elseIfStatement = new IfStatement();

                                if (ParseExpression(out Expression elseIfExpression))
                                {
                                    elseIfStatement.Condition = elseIfExpression;
                                    if (tokenReader.Expect(LexKind.Keyword, "then"))
                                    {
                                        tokenReader.Skip(1);
                                        elseIfStatement.Body = ParseStatements();
                                        ifStatement.ElseIfStatements.Add(elseIfStatement);
                                    }
                                }
                                continue;
                            }

                            if (tokenReader.Expect(LexKind.Keyword, "else"))
                            {
                                tokenReader.Skip(1);
                                ifStatement.ElseStatements = ParseStatements();
                                continue;
                            }

                            break;
                        }


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

        // TODO: Seperate assignment statement from declaration statement (local statement)
        bool ParseAssignmentStatement(out AssignmentStatement assignmentStatement)
        {
            assignmentStatement = new AssignmentStatement();

            int localOffset = 0;

            if(tokenReader.Expect(LexKind.Keyword, "local"))
            {
                assignmentStatement.IsLocal = true;
                localOffset = 1;
            }

            if (tokenReader.Expect(LexKind.Identifier, localOffset))
            {
                assignmentStatement.Name = tokenReader.Peek(localOffset).Value;

                if(tokenReader.Expect(LexKind.Equals, 1 + localOffset))
                {
                    tokenReader.Skip(2 + localOffset);
                    if (ParseExpression(out Expression value))
                    {
                        assignmentStatement.Value = value;

                        return true;
                    }
                }

                if(assignmentStatement.IsLocal)
                {
                    tokenReader.Skip(2);
                    return true;
                }
            }

            return false;
        }

        bool ParseFunctionDeclarationStatement(out FunctionDeclarationStatement functionDeclarationStatement)
        {
            functionDeclarationStatement = new FunctionDeclarationStatement();

            if(tokenReader.Expect(LexKind.Keyword, "local"))
            {
                functionDeclarationStatement.IsLocal = true;
            }

            if (tokenReader.Expect(LexKind.Keyword) && tokenReader.Peek().Value == "function")
            {
                tokenReader.Skip(functionDeclarationStatement.IsLocal ? 2 : 1);

                if (ParseIdentifierExpression(out IdentifierExpression identifierExpression))
                {
                    functionDeclarationStatement.Name = identifierExpression;
                    if (tokenReader.ExpectFatal(LexKind.ParentheseOpen))
                    {
                        tokenReader.Skip(1);
                        functionDeclarationStatement.Parameters = ParseExpressions();
                        tokenReader.Skip(1);
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
                    tokenReader.Skip(1);
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
            if(ParseAssignmentStatement(out AssignmentStatement assignmentStatement))
            {
                statement = assignmentStatement;
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
