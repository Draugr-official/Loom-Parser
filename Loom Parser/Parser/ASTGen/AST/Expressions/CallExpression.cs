using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGen.AST.Expressions
{
    /// <summary>
    /// Declares a function call as an expression
    /// </summary>
    class CallExpression : Expression
    {
        /// <summary>
        /// The operand of the function being called to
        /// </summary>
        public Expression Operand { get; set; }

        /// <summary>
        /// The arguments of the function being called to
        /// </summary>
        public ExpressionList Arguments { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        public CallExpression(Expression name, ExpressionList arguments)
        {
            this.Operand = name;
            this.Arguments = arguments;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public CallExpression()
        {
            Operand = new Expression();
            Arguments = new ExpressionList();
        }
    }
}
