using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    /// <summary>
    /// Defines a return statement (e.g 'return 3')
    /// </summary>
    class ReturnStatement : Statement
    {
        /// <summary>
        /// The return expression of the return statement
        /// </summary>
        public ExpressionList ReturnValues { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="returnValues"></param>
        public ReturnStatement(ExpressionList returnValues)
        {
            this.ReturnValues = returnValues;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReturnStatement()
        {
            this.ReturnValues = null;
        }
    }
}
