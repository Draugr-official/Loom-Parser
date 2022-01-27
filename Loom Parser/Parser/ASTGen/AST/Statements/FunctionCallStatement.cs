using Loom_Parser.Parser.ASTGen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Statements
{
    /// <summary>
    /// Declares a function being called to within a scope
    /// </summary>
    class FunctionCallStatement : Statement
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
        public FunctionCallStatement(string name, ExpressionList arguments = null)
        {
            this.Name = name;
            if(arguments == null)
            {
                this.Arguments = new ExpressionList();
            }
            else
            {
                this.Arguments = arguments;
            }
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FunctionCallStatement()
        {
            this.Name = "";
            this.Arguments = new ExpressionList();
        }
    }
}
