using Loom_Parser.Parser.Lexer.ASTGen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.Lexer.ASTGen.AST.Statements
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
        /// The parameters of the function being declared
        /// </summary>
        public ExpressionList Parameters { get; set; }

        /// <summary>
        /// The body of the function being declared
        /// </summary>
        public Statement Body { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="body"></param>
        public FunctionDeclarationStatement(string name, ExpressionList parameters, Statement body)
        {
            this.Name = name;
            this.Parameters = parameters;
            this.Body = body;
        }
    }
}
