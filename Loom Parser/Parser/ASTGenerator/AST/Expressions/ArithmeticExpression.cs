using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    /// <summary>
    /// Defines a binary expression (e.g '3 + 4', '6 * 8'
    /// </summary>
    class ArithmeticExpression : Expression
    {
        /// <summary>
        /// Left hand operand
        /// </summary>
        public Expression Left { get; set; }

        /// <summary>
        /// Right hand operand
        /// </summary>
        public Expression Right { get; set; }

        /// <summary>
        /// The operator between each operand
        /// </summary>
        public ArithmeticOperators Operator { get; set; }
    }

    /// <summary>
    /// An enumerator containing all arithmetic operators
    /// </summary>
    public enum ArithmeticOperators
    {
        Unknown,

        /// <summary>
        /// E.g '+'
        /// </summary>
        Addition,

        /// <summary>
        /// E.g '-'
        /// </summary>
        Subtraction,

        /// <summary>
        /// E.g '*'
        /// </summary>
        Multiplication,

        /// <summary>
        /// E.g '/'
        /// </summary>
        Division,

        /// <summary>
        /// E.g '%'
        /// </summary>
        Modulus,

        /// <summary>
        /// E.g '^'
        /// </summary>
        Exponentiation
    }
}
