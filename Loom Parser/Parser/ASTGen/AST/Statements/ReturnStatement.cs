using Loom_Parser.Parser.ASTGen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Statements
{
    /// <summary>
    /// Defines a return statement (e.g 'return 3')
    /// </summary>
    class ReturnStatement : Statement
    {
        /// <summary>
        /// The return expression of the return statement
        /// </summary>
        public Expression ReturnValue { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="returnValue"></param>
        public ReturnStatement(Expression returnValue)
        {
            this.ReturnValue = returnValue;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReturnStatement()
        {
            this.ReturnValue = null;
        }
    }
}
