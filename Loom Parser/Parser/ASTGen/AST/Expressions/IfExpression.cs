using Loom.Parser.ASTGen.AST.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGen.AST.Expressions
{
    /// <summary>
    /// Lua if statement
    /// </summary>
    internal class IfExpression : Expression
    {
        /// <summary>
        /// The condition of the if statement
        /// </summary>
        public Expression Condition { get; set; }

        /// <summary>
        /// The body of the if statement
        /// </summary>
        public Expression Body { get; set; }

        /// <summary>
        /// Nested else if statements in the conditional
        /// </summary>
        public List<IfExpression> ElseIfExpressions { get; set; }

        /// <summary>
        /// Else statements in the conditional
        /// </summary>
        public Expression ElseExpression { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="body"></param>
        public IfExpression(Expression condition, Expression body)
        {
            this.Condition = condition;
            this.Body = body;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IfExpression()
        {
            this.Condition = new Expression();
            this.Body = new Expression();
            this.ElseIfExpressions = new List<IfExpression>();
            this.ElseExpression = null;
        }
    }
}
