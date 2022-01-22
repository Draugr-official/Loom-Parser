using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.Lexer.ASTGen.AST.Expressions
{
    /// <summary>
    /// Declares a function call as an expression
    /// </summary>
    class FunctionCallExpression : Expression
    {
        /// <summary>
        /// The name of the function being called to
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The arguments of the function being called to
        /// </summary>
        public ExpressionList Arguments { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        public FunctionCallExpression(string name, ExpressionList arguments)
        {
            this.Name = name;
            this.Arguments = arguments;
        }
    }
}
