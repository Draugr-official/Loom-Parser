using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{

    /// <summary>
    /// Declares a function within a scope
    /// </summary>
    class FunctionDeclarationStatement : Statement
    {
        /// <summary>
        /// The name of the function being declared
        /// </summary>
        public IdentifierExpression Name { get; set; }

        /// <summary>
        /// Determines if the function declaration is named
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// The parameters of the function being declared
        /// </summary>
        public ExpressionList Parameters { get; set; }

        /// <summary>
        /// The body of the function being declared
        /// </summary>
        public StatementList Body { get; set; }

        /// <summary>
        /// Determines if the function declaration is localized
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="body"></param>
        public FunctionDeclarationStatement(string name, ExpressionList parameters, StatementList body)
        {
            this.Name = new IdentifierExpression() { Identifier = name };
            this.IsAnonymous = name == "";
            this.Parameters = parameters;
            this.Body = body;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FunctionDeclarationStatement()
        {
            this.Name = new IdentifierExpression() { Identifier = "" };
            this.IsAnonymous = true;
            this.Parameters = new ExpressionList();
            this.Body = new StatementList();
        }
    }
}
