using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    /// <summary>
    /// Declares a function being called to within a scope
    /// </summary>
    class CallStatement : Statement
    {
        /// <summary>
        /// The name of the function being called to
        /// </summary>
        public Expression Function { get; set; }

        /// <summary>
        /// The arguments of the function being called to
        /// </summary>
        public ExpressionList Arguments { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public CallStatement()
        {
            this.Arguments = new ExpressionList();
        }
    }
}
