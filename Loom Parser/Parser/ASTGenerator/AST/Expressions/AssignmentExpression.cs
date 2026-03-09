using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    internal class AssignmentExpression : Expression
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
        public AssignmentExpression()
        {
            this.Variables = new ExpressionList();
            this.Values = new ExpressionList();
        }
    }
}
