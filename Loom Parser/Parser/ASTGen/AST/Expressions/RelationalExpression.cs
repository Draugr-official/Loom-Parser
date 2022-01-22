using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Expressions
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
        EqualTo,
        NotEqualTo,
        BiggerThan,
        BiggerOrEqual,
        SmallerThan,
        SmallerOrEqual
    }
}
