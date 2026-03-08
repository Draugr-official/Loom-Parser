using Loom.Parser.ASTGenerator.AST.Statements;
using Loom.Parser.ASTGenerator.AST.Expressions;
using Loom.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom.Parser.ASTGenerator.AST;
using System.Runtime.InteropServices;
using Loom.Parser.Lexer;
using System.Threading;

namespace Loom.Parser.ASTGenerator
{
    /// <summary>
    /// Abstract syntax tree generator
    /// </summary>
    class ASTGenerator
    {
        /// <summary>
        /// The token reader of the AST generator
        /// </summary>
        LexTokenReader tokenReader { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="lexTokens"></param>
        public ASTGenerator(LexTokenList lexTokens)
        {
            this.tokenReader = new LexTokenReader(lexTokens);
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
                Console.WriteLine("1 " + tokenReader.Peek().Line + ": " + tokenReader.Peek().Kind + ", " + tokenReader.Peek().Value);
                tokenReader.Skip(1);

                Console.WriteLine("2---- " + tokenReader.Peek().Line + ": " + tokenReader.Peek().Kind + ", " + tokenReader.Peek().Value);
                callExpression.Arguments = ParseExpressions();
                Console.WriteLine("4----------------- " + tokenReader.Peek().Line + ": " + tokenReader.Peek().Kind + ", " + tokenReader.Peek().Value);

                if (tokenReader.ExpectFatal(LexKind.ParentheseClose))
                {
                    tokenReader.Skip(1);
                    Console.WriteLine("5----------------------------------- " + tokenReader.Peek().Line + ": " + tokenReader.Peek().Kind + ", " + tokenReader.Peek().Value);
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

        bool ParseLogicalExpression(Expression left, out LogicalExpression logicalExpression)
        {
            logicalExpression = new LogicalExpression();
            logicalExpression.Left = left;

            if (tokenReader.Expect(LexKind.Keyword))
            {
                switch(tokenReader.Peek().Value)
                {
                    case "and": logicalExpression.Operator = LogicalOperators.And; break;
                    case "or": logicalExpression.Operator = LogicalOperators.Or; break;
                    case "not": logicalExpression.Operator = LogicalOperators.Not; break;
                    default: return false;
                }

                tokenReader.Skip(1);
                if (ParseExpression(out Expression right))
                {
                    logicalExpression.Right = right;
                    return true;
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

            if (tokenReader.Expect(LexKind.Vararg))
            {
                tokenReader.Skip(1);
                return true;
            }

            return false;
        }

        bool ParseLengthExpression(out LengthExpression lengthExpression)
        {
            lengthExpression = new LengthExpression();

            if (tokenReader.Expect(LexKind.Hashtag))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression expression))
                {
                    lengthExpression.Identifier = expression;
                    return true;
                }

                throw new Exception("Length expression missing identifier");
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

        bool ParseNilExpression(out NilExpression nilExpression)
        {
            nilExpression = new NilExpression();

            if(tokenReader.Expect(LexKind.Keyword, "nil"))
            {
                tokenReader.Skip(1);
                return true;
            }

            return false;
        }

        bool ParseFunctionExpression(Expression leftExpression, out Expression expression)
        {
            expression = leftExpression;

            if (ParseFunctionDeclarationExpression(out FunctionDeclarationExpression functionDeclarationExpression))
            {
                expression = functionDeclarationExpression;
            }

            if (ParseIdentifierExpression(out IdentifierExpression identifierExpression))
            {
                expression = identifierExpression;
            }

            if (ParseIndexExpression(expression, out IndexExpression indexExpression))
            {
                expression = indexExpression;
            }


            if(expression == null)
            {
                return false;
            }

            return true;
        }

        bool ParseVariableExpression(Expression leftExpression, out Expression expression)
        {
            expression = leftExpression;

            if (ParseIdentifierExpression(out IdentifierExpression identifierExpression))
            {
                expression = identifierExpression;
            }

            if (ParseIndexExpression(expression, out IndexExpression indexExpression))
            {
                expression = indexExpression;
            }


            if (expression == null)
            {
                return false;
            }

            return true;
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

            if (ParseNegativeExpression(out NegativeExpression negativeExpression))
            {
                expression = negativeExpression;
            }
            if (ParseLengthExpression(out LengthExpression lengthExpression))
            {
                expression = lengthExpression;
            }
            if (ParseNilExpression(out NilExpression nilExpression))
            {
                expression = nilExpression;
            }
            if (ParseArrayExpression(out ArrayExpression tableExpression))
            {
                expression = tableExpression;
            }


            // Expression with left side expressions

            if (ParseFunctionDeclarationExpression(out FunctionDeclarationExpression functionDeclarationExpression))
            {
                expression = functionDeclarationExpression;
            }

            if (ParseIdentifierExpression(out IdentifierExpression identifierExpression))
            {
                expression = identifierExpression;
            }

            if (ParseIndexExpression(expression, out IndexExpression indexExpression))
            {
                expression = indexExpression;
            }

            if (ParseVarargExpression(out VarargExpression varargExpression))
            {
                expression = varargExpression;
            }
            if (ParseConstantExpression(out ConstantExpression constantExpression))
            {
                expression = constantExpression;
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

            if (ParseArithmeticExpression(expression, out ArithmeticExpression arithmeticExpression))
            {
                expression = arithmeticExpression;
            }

            if(ParseLogicalExpression(expression, out LogicalExpression logicalExpression))
            {
                expression = logicalExpression;
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
                if (ParseExpression(out Expression expression))
                {
                    expressionList.Add(expression);
                }

                if (tokenReader.Expect(LexKind.Comma))
                {
                    tokenReader.Skip(1);
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

        bool ParseForStatement(out ForStatement forStatement)
        {
            forStatement = new ForStatement();

            if(tokenReader.Expect(LexKind.Keyword, "for"))
            {
                tokenReader.Skip(1);
                if(ParseIdentifierExpression(out IdentifierExpression identifierExpression))
                {
                    // For loop
                    if (ParseAssignmentExpression(identifierExpression, out AssignmentExpression controlVariableExpression))
                    {
                        forStatement.ControlVariable = controlVariableExpression;
                        tokenReader.Skip(1); // Skip the comma

                        if (ParseExpression(out Expression endValueExpression))
                        {
                            forStatement.EndValue = endValueExpression;
                            
                            if(tokenReader.Expect(LexKind.Comma))
                            {
                                tokenReader.Skip(1); // Skip the coma
                                if (ParseExpression(out Expression incrementExpression))
                                {
                                    forStatement.Increment = incrementExpression;
                                }
                            }

                            if (tokenReader.ExpectFatal(LexKind.Keyword, "do"))
                            {
                                tokenReader.Skip(1);
                                forStatement.Body = ParseStatements();

                                if(tokenReader.ExpectFatal(LexKind.Keyword, "end"))
                                {
                                    tokenReader.Skip(1);
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Expected valid expression at endvalue in for statement");
                        }
                    }

                    // Generic for loop
                    GenericForStatement genericForStatement = new GenericForStatement();
                    genericForStatement.VariableArray.Array.Add(identifierExpression);

                    for(; ; )
                    {
                        if(!tokenReader.Expect(LexKind.Comma))
                        {
                            break;
                        }

                        tokenReader.Skip(1);
                        if(ParseIdentifierExpression(out IdentifierExpression variableExpression))
                        {
                            genericForStatement.VariableArray.Array.Add(variableExpression);
                        }
                    }

                    if(tokenReader.ExpectFatal(LexKind.Keyword, "in"))
                    {
                        tokenReader.Skip(1);
                        if(ParseExpression(out Expression iteratorExpression))
                        {
                            genericForStatement.Iterator = iteratorExpression;

                            if(tokenReader.ExpectFatal(LexKind.Keyword, "do"))
                            {
                                tokenReader.Skip(1);
                                genericForStatement.Body = ParseStatements();

                                if (tokenReader.ExpectFatal(LexKind.Keyword, "end"))
                                {
                                    tokenReader.Skip(1);
                                    forStatement = genericForStatement;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Expected valid expression at iterator in generic for statement");
                        }
                    }
                }

            }

            return false;
        }

        bool ParseDoStatement(out DoStatement doStatement)
        {
            doStatement = new DoStatement();

            if(tokenReader.Expect(LexKind.Keyword, "do"))
            {
                tokenReader.Skip(1);
                doStatement.Body = ParseStatements();

                if(tokenReader.ExpectFatal(LexKind.Keyword, "end"))
                {
                    tokenReader.Skip(1);
                    return true;
                }
            }

            return false;
        }

        bool ParseRepeatStatement(out RepeatStatement repeatStatement)
        {
            repeatStatement = new RepeatStatement();

            if(tokenReader.Expect(LexKind.Keyword, "repeat"))
            {
                tokenReader.Skip(1);
                repeatStatement.Body = ParseStatements();

                if(tokenReader.ExpectFatal(LexKind.Keyword, "until"))
                {
                    tokenReader.Skip(1);
                    if(ParseExpression(out Expression conditionalExpression))
                    {
                        repeatStatement.Condition = conditionalExpression;
                        return true;
                    }
                    else
                    {
                        throw new Exception("Expected valid expression at until in repeat statement");
                    }
                }
            }

            return false;
        }

        bool ParseBreakStatement(out BreakStatement breakStatement)
        {
            breakStatement = new BreakStatement();

            if(tokenReader.Expect(LexKind.Keyword, "break"))
            {
                tokenReader.Skip(1);
                return true;
            }

            return false;
        }

        bool ParseContinueStatement(out ContinueStatement continueStatement)
        {
            continueStatement = new ContinueStatement();

            if (tokenReader.Expect(LexKind.Keyword, "continue"))
            {
                tokenReader.Skip(1);
                return true;
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

        bool ParseFunctionDeclarationStatement(out FunctionDeclarationStatement functionDeclarationStatement)
        {
            functionDeclarationStatement = new FunctionDeclarationStatement();

            int localOffset = 0;

            if (tokenReader.Expect(LexKind.Keyword, "local"))
            {
                functionDeclarationStatement.IsLocal = true;
                localOffset = 1;
            }

            if (tokenReader.Expect(LexKind.Keyword, localOffset) && tokenReader.Peek(localOffset).Value == "function")
            {
                tokenReader.Skip(localOffset + 1);

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

        bool ParseCallStatement(Expression function, out CallStatement callStatement)
        {
            callStatement = new CallStatement();

            callStatement.Function = function;

            if(!tokenReader.Expect(LexKind.ParentheseOpen))
            {
                return false;
            }

            tokenReader.Skip(1);
            callStatement.Arguments = ParseExpressions();

            if (callStatement.Arguments.Count == 0)
            {
                tokenReader.Skip(1);
            }

            if (tokenReader.ExpectFatal(LexKind.ParentheseClose))
            {
                tokenReader.Skip(1);
            }

            return true;
        }

        // TODO: Seperate assignment statement from declaration statement (local statement)
        // TODO: Multiple declaration statement
        bool ParseAssignmentStatement(Expression varExpression, out AssignmentStatement assignmentStatement)
        {
            assignmentStatement = new AssignmentStatement();


            if(varExpression == null)
            {
                if (tokenReader.Expect(LexKind.Keyword, "local"))
                {
                    assignmentStatement.IsLocal = true;
                    tokenReader.Skip(1);
                }

                if (ParseVariableExpression(null, out Expression variableExpression))
                {
                    assignmentStatement.Variable = variableExpression;

                    if (tokenReader.Expect(LexKind.Equals))
                    {
                        tokenReader.Skip(1);
                        if (ParseExpression(out Expression value))
                        {
                            assignmentStatement.Value = value;

                            return true;
                        }
                    }
                }
            }
            else
            {
                assignmentStatement.Variable = varExpression;

                if (tokenReader.Expect(LexKind.Equals))
                {
                    tokenReader.Skip(1);
                    if (ParseExpression(out Expression value))
                    {
                        assignmentStatement.Value = value;

                        return true;
                    }
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
            if(ParseForStatement(out ForStatement forStatement))
            {
                statement = forStatement;
                return true;
            }
            if(ParseRepeatStatement(out RepeatStatement repeatStatement))
            {
                statement = repeatStatement;
                return true;
            }
            if(ParseDoStatement(out DoStatement doStatement))
            {
                statement = doStatement;
                return true;
            }
            if(ParseBreakStatement(out BreakStatement breakStatement))
            {
                statement = breakStatement;
                return true;
            }
            if(ParseContinueStatement(out ContinueStatement continueStatement))
            {
                statement = continueStatement;
                return true;
            }


            if(ParseFunctionExpression(null, out Expression expression))
            {
                if (ParseAssignmentStatement(expression, out AssignmentStatement assignmentStatement))
                {
                    statement = assignmentStatement;
                    return true;
                }
                if (ParseCallStatement(expression,out CallStatement callStatement))
                {
                    statement = callStatement;
                    return true;
                }
            }
            else
            {
                if (ParseAssignmentStatement(null, out AssignmentStatement assignmentStatement))
                {
                    statement = assignmentStatement;
                    return true;
                }
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
