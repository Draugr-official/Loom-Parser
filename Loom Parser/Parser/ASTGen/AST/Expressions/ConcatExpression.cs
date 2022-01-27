using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Expressions
{
    /// <summary>
    /// Defines a concatenation expression (e.g '"Hello" .. " World"'
    /// </summary>
    class ConcatExpression : Expression
    {
        /// <summary>
        /// Left hand operand
        /// </summary>
        public Expression Left { get; set; }

        /// <summary>
        /// Right hand operand
        /// </summary>
        public Expression Right { get; set; }
    }
}
