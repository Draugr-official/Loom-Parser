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
using System.Runtime.InteropServices.WindowsRuntime;

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

                if(tokenReader.Expect(LexKind.BraceClose))
                {
                    tokenReader.Skip(1);
                    return true;
                }

                arrayExpression.Array = ParseExpressions(true);

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

                if (tokenReader.ExpectFatal(LexKind.ParentheseClose))
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
            assignmentExpression.Variables.Add(left);


            switch (tokenReader.Peek().Kind)
            {
                case LexKind.Equals:
                    {
                        tokenReader.Skip(1);
                        if (ParseExpression(out Expression right))
                        {
                            assignmentExpression.Values.Add(right);
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
                    lengthExpression.Expression = expression;
                    return true;
                }

                throw new Exception("Length expression missing expression");
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
                    negativeExpression.Expression = expression;
                    return true;
                }

                throw new Exception("Negative expression missing expression");
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

        bool ParseGroupedExpression(out GroupedExpression groupedExpression)
        {
            groupedExpression = new GroupedExpression();

            if(tokenReader.Expect(LexKind.ParentheseOpen))
            {
                tokenReader.Skip(1);

                if(tokenReader.Expect(LexKind.ParentheseClose))
                {
                    groupedExpression = null;
                    return true;
                }

                if (ParseExpression(out Expression expression))
                {
                    groupedExpression.Expression = expression;
                    
                    if(tokenReader.ExpectFatal(LexKind.ParentheseClose))
                    {
                        tokenReader.Skip(1);
                        return true;
                    }
                }
            }
            return false;
        }

        bool ParseFunctionExpression(Expression leftExpression, out Expression expression)
        {
            expression = leftExpression;

            if (ParseGroupedExpression(out GroupedExpression groupedExpression))
            {
                expression = groupedExpression;
            }

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

        bool ParseFunctionExpressions(out ExpressionList expressionList)
        {
            expressionList = new ExpressionList();

            for (; ; )
            {
                if (ParseFunctionExpression(null, out Expression expression))
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

            if(expressionList.Count == 0)
            {
                return false;
            }

            return true;
        }

        // Unused as of now
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

        bool ParseVariableExpressions(out ExpressionList expressionList)
        {
            expressionList = new ExpressionList();

            for (; ; )
            {
                if (ParseVariableExpression(null, out Expression expression))
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

            if (expressionList.Count == 0)
            {
                return false;
            }

            return true;
        }

        bool ParseNotExpression(out NotExpression notExpression)
        {
            notExpression = new NotExpression();

            if (tokenReader.Expect(LexKind.Keyword, "not"))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression expression))
                {
                    notExpression.Expression = expression;
                    return true;
                }

                throw new Exception("Not expression missing expression");
            }

            return false;
        }

        bool ParseMemberExpression(Expression parentExpression, out MemberExpression memberExpression)
        {
            memberExpression = new MemberExpression();

            if(tokenReader.Expect(LexKind.Dot))
            {
                tokenReader.Skip(1);
                memberExpression.IsInvoke = false;
            }
            else
            {
                return false;
            }

            if(tokenReader.Expect(LexKind.Colon))
            {
                tokenReader.Skip(1);
                memberExpression.IsInvoke = true;
            }
            else
            {
                return false;
            }

            if(ParseExpression(out Expression expression))
            {
                memberExpression.Expression = expression;
                return true;
            }

            throw new Exception("Invalid member expression");
        }

        bool ParseRecordExpression(out RecordExpression recordExpression)
        {
            recordExpression = new RecordExpression();

            if(tokenReader.Expect(LexKind.BracketOpen))
            {
                tokenReader.Skip(1);
                if (ParseExpression(out Expression expression))
                {
                    recordExpression.Expression = expression;
                    if(tokenReader.ExpectFatal(LexKind.BracketClose))
                    {
                        tokenReader.Skip(1);
                        return true;
                    }
                }
            }

            return false;
        }

        bool ParseSingleExpression(out Expression expression)
        {
            expression = null;

            if (ParseGroupedExpression(out GroupedExpression groupedExpression))
            {
                expression = groupedExpression;
                return true;
            }
            if (ParseNegativeExpression(out NegativeExpression negativeExpression))
            {
                expression = negativeExpression;
                return true;
            }
            if (ParseLengthExpression(out LengthExpression lengthExpression))
            {
                expression = lengthExpression;
                return true;
            }
            if (ParseNotExpression(out NotExpression notExpression))
            {
                expression = notExpression;
                return true;
            }
            if (ParseNilExpression(out NilExpression nilExpression))
            {
                expression = nilExpression;
                return true;
            }
            if (ParseIdentifierExpression(out IdentifierExpression identifierExpression))
            {
                expression = identifierExpression;
                return true;
            }
            if (ParseVarargExpression(out VarargExpression varargExpression))
            {
                expression = varargExpression;
                return true;
            }
            if (ParseConstantExpression(out ConstantExpression constantExpression))
            {
                expression = constantExpression;
                return true;
            }
            if (ParseArrayExpression(out ArrayExpression tableExpression))
            {
                expression = tableExpression;
                return true;
            }
            if(ParseRecordExpression(out RecordExpression recordExpression))
            {
                expression = recordExpression;
                return true;
            }

            return false;
        }

        bool ParseExpression(out Expression expression, bool parsingArray = false)
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

            if(ParseSingleExpression(out Expression singleExpression))
            {
                expression = singleExpression;
            }
            else
            {
                return false;
            }

            // Expression with left side expressions
            for(; ; )
            {
                if(ParseMemberExpression(expression, out MemberExpression memberExpression))
                {
                    expression = memberExpression;
                    continue;
                }

                if (ParseCallExpression(expression, out CallExpression callExpression))
                {
                    expression = callExpression;
                    continue;
                }

                if (ParseIndexExpression(expression, out IndexExpression indexExpression))
                {
                    expression = indexExpression;
                    continue;
                }

                // Expressions with left and right side expressions
                if (ParseRelationalExpression(expression, out RelationalExpression relationalExpression))
                {
                    expression = relationalExpression;
                    continue;
                }

                if (ParseArithmeticExpression(expression, out ArithmeticExpression arithmeticExpression))
                {
                    expression = arithmeticExpression;
                    continue;
                }

                if (ParseLogicalExpression(expression, out LogicalExpression logicalExpression))
                {
                    expression = logicalExpression;
                    continue;
                }

                if (ParseConcatExpression(expression, out ConcatExpression concatExpression))
                {
                    expression = concatExpression;
                    continue;
                }

                if (parsingArray && ParseAssignmentExpression(expression, out AssignmentExpression assignmentExpression))
                {
                    expression = assignmentExpression;
                    continue;
                }

                break;
            }

            return expression != null;
        }

        ExpressionList ParseExpressions(bool parsingArray = false)
        {
            ExpressionList expressionList = new ExpressionList();

            for(; ; )
            {
                if (ParseExpression(out Expression expression, parsingArray))
                {
                    expressionList.Add(expression);
                }
                else
                {
                    break;
                }

                if (tokenReader.Expect(LexKind.Comma) 
                    || (parsingArray && tokenReader.Expect(LexKind.Semicolon)))
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
        bool ParseGroupedStatement(out GroupedStatement groupedStatement)
        {
            groupedStatement = new GroupedStatement();

            if (tokenReader.Expect(LexKind.ParentheseOpen))
            {
                tokenReader.Skip(1);

                if (tokenReader.Expect(LexKind.ParentheseClose))
                {
                    groupedStatement = null;
                    return true;
                }

                if (ParseExpression(out Expression expression))
                {
                    groupedStatement.Expression = expression;

                    if (tokenReader.ExpectFatal(LexKind.ParentheseClose))
                    {
                        tokenReader.Skip(1);
                        return true;
                    }
                }
            }
            return false;
        }
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

            if (tokenReader.Expect(LexKind.Keyword) && tokenReader.Peek().Value == "function")
            {
                tokenReader.Skip(1);
                Console.WriteLine("Attempted to parse function");

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
        bool ParseAssignmentStatement(ExpressionList varExpressions, out AssignmentStatement assignmentStatement)
        {
            assignmentStatement = new AssignmentStatement();

            assignmentStatement.Variables = varExpressions;
            if (tokenReader.Expect(LexKind.Equals))
            {
                tokenReader.Skip(1);
                assignmentStatement.Values = ParseExpressions();

                if(assignmentStatement.Values.Count == 0)
                {
                    throw new Exception($"Line {tokenReader.Peek().Line}: Assignment statement contains no values");
                }

                return true;
            }

            return false;
        }

        bool ParseLocalDeclarationStatement(out LocalDeclarationStatement localDeclarationStatement)
        {
            localDeclarationStatement = new LocalDeclarationStatement();

            if(tokenReader.Expect(LexKind.Keyword, "local"))
            {
                if(tokenReader.Expect(LexKind.Identifier, 1))
                {
                    tokenReader.Skip(1);

                    if(ParseVariableExpressions(out ExpressionList expressionList))
                    {
                        if (ParseAssignmentStatement(expressionList, out AssignmentStatement assignmentStatement))
                        {
                            localDeclarationStatement.Statement = assignmentStatement;
                            return true;
                        }

                        localDeclarationStatement.Expressions = expressionList;
                        return true;
                    }


                }

                if(tokenReader.Expect(LexKind.Keyword, "function", 1))
                {
                    tokenReader.Skip(1);
                    if(ParseFunctionDeclarationStatement(out FunctionDeclarationStatement functionDeclarationStatement))
                    {
                        localDeclarationStatement.Statement = functionDeclarationStatement;
                        return true;
                    }
                }

                throw new Exception("Identifier or function expected after keyword 'local'");
            }

            return false;
        }

        /// <summary>
        /// Generates a statement at the given position in the token reader
        /// </summary>
        bool ParseStatement(out Statement statement)
        {
            statement = new Statement();
            if (ParseFunctionDeclarationStatement(out FunctionDeclarationStatement functionDeclarationStatement))
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
            if(ParseLocalDeclarationStatement(out LocalDeclarationStatement localDeclarationStatement))
            {
                statement = localDeclarationStatement;
                return true;
            }


            if (ParseFunctionExpressions(out ExpressionList expressionList))
            {
                if(expressionList.Count == 1)
                {
                    if (ParseCallStatement(expressionList.First(), out CallStatement callStatement))
                    {
                        statement = callStatement;
                        return true;
                    }
                }

                if (ParseAssignmentStatement(expressionList, out AssignmentStatement assignmentStatement))
                {
                    statement = assignmentStatement;
                    return true;
                }

                throw new Exception("Invalid parsing");
            }


            if (ParseGroupedStatement(out GroupedStatement groupedStatement))
            {
                statement = groupedStatement;
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

                if (tokenReader.Expect(LexKind.Semicolon))
                {
                    tokenReader.Consume();
                }

                if (tokenReader.Expect(LexKind.EOF))
                {
                    break;
                }
            }

            return statements;
        }
    }
}
