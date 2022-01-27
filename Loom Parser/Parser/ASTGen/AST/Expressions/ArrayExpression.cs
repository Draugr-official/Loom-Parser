using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Expressions
{
    /// <summary>
    /// Defines a array expression (e.g '{"Hello", 123, true}')
    /// </summary>
    class ArrayExpression : Expression
    {
        /// <summary>
        /// The contents of the table
        /// </summary>
        public ExpressionList Array { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="array"></param>
        public ArrayExpression(ExpressionList array)
        {
            this.Array = array;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ArrayExpression()
        {
            this.Array = new ExpressionList();
        }
    }
}
