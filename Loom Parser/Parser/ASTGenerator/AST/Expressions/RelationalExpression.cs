using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    /// <summary>
    /// Defines a relational operator as an expression
    /// </summary>
    class RelationalExpression : Expression
    {
        /// <summary>
        /// The operator of the relational expression
        /// </summary>
        public RelationalOperators Operator { get; set; }

        /// <summary>
        /// The left hand expression of the relational expression
        /// </summary>
        public Expression Left { get; set; }

        /// <summary>
        /// The right hand expression of the relational expression
        /// </summary>
        public Expression Right { get; set; }
    }

    /// <summary>
    /// Enumerator displaying all available relational operators
    /// </summary>
    public enum RelationalOperators
    {
        /// <summary>
        /// ==
        /// </summary>
        EqualTo,

        /// <summary>
        /// ~=
        /// </summary>
        NotEqualTo,

        /// <summary>
        /// >
        /// </summary>
        BiggerThan,

        /// <summary>
        /// >=
        /// </summary>
        BiggerOrEqual,

        /// <summary>
        /// <
        /// </summary>
        SmallerThan,

        /// <summary>
        /// <=
        /// </summary>
        SmallerOrEqual
    }
}
