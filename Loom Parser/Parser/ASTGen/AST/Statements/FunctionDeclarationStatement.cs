using Loom_Parser.Parser.ASTGen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Statements
{

    /// <summary>
    /// Declares a function within a scope
    /// </summary>
    class FunctionDeclarationStatement : Statement
    {
        /// <summary>
        /// The name of the function being declared
        /// </summary>
        public string Name { get; set; }

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
            this.Name = name;
            this.IsAnonymous = name == "";
            this.Parameters = parameters;
            this.Body = body;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FunctionDeclarationStatement()
        {
            this.Name = "";
            this.IsAnonymous = true;
            this.Parameters = new ExpressionList();
            this.Body = new StatementList();
        }
    }
}
