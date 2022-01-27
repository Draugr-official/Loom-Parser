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
            this.Variables = new Dictionary<string, VariableAssignStatement>();
            this.Locals = new Dictionary<string, VariableAssignStatement>();
        }

        /// <summary>
        /// Global variables in the script being parsed
        /// </summary>
        Dictionary<string, VariableAssignStatement> Variables { get; set; }

        /// <summary>
        /// Local variables defined in the current scope, cleared when exiting scope
        /// </summary>
        Dictionary<string, VariableAssignStatement> Locals    { get; set; }

        VariableAssignStatement GetVariableFromName(string name)
        {
            if (this.Variables.ContainsKey(name))
            {
                return this.Variables[name];
            }
            else
            {
                return this.Locals[name];
            }
        }

        void AddGlobalVariable(VariableAssignStatement variable)
        {
            if (!this.Variables.ContainsValue(variable))
            {
                Console.WriteLine("Added " + variable.Name);
                this.Variables.Add(variable.Name, variable);
            }
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
                        return true;
                    }
                }
            }

            return false;
        }

        bool ParseArrayExpression(out ArrayExpression tableExpression)
        {
            tableExpression = new ArrayExpression();
            int Scope = 1;

            if(tokenReader.Expect(LexKind.BraceOpen))
            {
                tokenReader.Skip(1);
                while (Scope > 0)
                {
                    if (tokenReader.Expect(LexKind.BraceOpen))
                    {
                        Scope++;
                    }
                    else if (tokenReader.Expect(LexKind.BraceClose))
                    {
                        Scope--;
                    }

                    if (ParseExpression(out Expression expression))
                    {
                        tableExpression.Array.Add(expression);
                    }
                    else
                    {
                        tokenReader.Skip(1);
                    }
                }

                return true;
            }

            return false;
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
                variableExpression.VariableDef = GetVariableFromName(tokenReader.Consume().Value);
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
            if(!parsedLayer && ParseArrayExpression(out ArrayExpression tableExpression))
            {
                expression = tableExpression;
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
            if (parsedLayer && ParseRelationalExpression(expression, out RelationalExpression relationalExpression))
            {
                expression = relationalExpression;
                return true;
            }

            if (parsedLayer && ParseBinaryExpression(expression, out BinaryExpression binaryExpression))
            {
                expression = binaryExpression;
                return true;
            }

            if (parsedLayer && ParseConcatExpression(expression, out ConcatExpression concatExpression))
            {
                expression = concatExpression;
                return true;
            }

            if(parsedLayer && ParseIndexExpression(expression, out IndexExpression indexExpression))
            {
                expression = indexExpression;
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
                        Console.WriteLine("Variable assign parsed");
                        variableAssignStatement.Value = value;
                        AddGlobalVariable(variableAssignStatement);

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
        bool ParseStatement(Statement parent, out Statement statement)
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
