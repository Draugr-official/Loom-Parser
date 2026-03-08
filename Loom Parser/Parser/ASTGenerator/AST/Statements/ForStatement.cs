using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    /// <summary>
    /// Lua for statement
    /// </summary>
    internal class ForStatement : Statement
    {
        /// <summary>
        /// The condition of the for statement
        /// </summary>
        public AssignmentExpression ControlVariable { get; set; }

        /// <summary>
        /// The end value that the for loop will iterate to
        /// </summary>
        public Expression EndValue { get; set; }

        /// <summary>
        /// The increment of the for statement
        /// <para>
        /// Not required
        /// </para>
        /// </summary>
        public Expression Increment { get; set; } = null;

        /// <summary>
        /// The body of the for statement
        /// </summary>
        public StatementList Body { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ForStatement()
        {
            this.Body = new StatementList();
        }
    }
}
