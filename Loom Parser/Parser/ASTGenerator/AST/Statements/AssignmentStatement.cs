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
        /// Determines if the variable is localized to a scope
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// The name of the variable being declared
        /// </summary>
        public Expression Variable { get; set; }

        /// <summary>
        /// The value being assigned to the variable
        /// </summary>
        public Expression Value { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public AssignmentStatement()
        {
            this.IsLocal = false;
            this.Value = null;
        }
    }
}
