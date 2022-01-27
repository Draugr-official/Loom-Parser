using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Expressions
{
    /// <summary>
    /// Defines an index expression (e.g arr[2])
    /// </summary>
    class IndexExpression : Expression
    {
        /// <summary>
        /// The position in the array being accessed
        /// </summary>
        public Expression Index { get; set; }

        /// <summary>
        /// The expression that the index is accessing
        /// </summary>
        public Expression Array { get; set; }

        public IndexExpression(Expression array, Expression index)
        {
            this.Array = array;
            this.Index = index;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IndexExpression()
        {
            this.Array = new Expression();
            this.Index = new Expression();
        }
    }
}
