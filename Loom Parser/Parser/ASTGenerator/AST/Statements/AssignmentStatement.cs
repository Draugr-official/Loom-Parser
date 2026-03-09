using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    /// <summary>
    /// Variable assigning statement | local a = "b" 
    /// </summary>
    class AssignmentStatement : Statement
    {
        /// <summary>
        /// The name of the variable being declared
        /// </summary>
        public ExpressionList Variables { get; set; }

        /// <summary>
        /// The value being assigned to the variable
        /// </summary>
        public ExpressionList Values { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public AssignmentStatement()
        {
            this.Variables = new ExpressionList();
            this.Values = new ExpressionList();
        }
    }
}
