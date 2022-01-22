using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Expressions
{
    /// <summary>
    /// Lua variable expression
    /// </summary>
    internal class VariableExpression : Expression
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        public VariableExpression(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public VariableExpression()
        {
            this.Name = "";
        }
    }
}
