using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    internal class LogicalExpression : Expression
    {
        public LogicalOperators Operator { get; set; }

        /// <summary>
        /// The left hand expression
        /// </summary>
        public Expression Left { get; set; }

        /// <summary>
        /// The right hand expression
        /// </summary>
        public Expression Right { get; set; }
    }

    public enum LogicalOperators
    {
        And,
        Or,
        Not,
    }
}
