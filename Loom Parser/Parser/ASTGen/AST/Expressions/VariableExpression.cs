using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom_Parser.Parser.ASTGen.AST.Statements;

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
        public string Name
        {
            get
            {
                return VariableDef.Name;
            }

            set
            {
                VariableDef.Name = value;
            }
        }

        /// <summary>
        /// Reference to the variable defining this variable
        /// </summary>
        public VariableAssignStatement VariableDef { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        public VariableExpression(string name)
        {
            this.VariableDef = new VariableAssignStatement();
            this.Name = name;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public VariableExpression()
        {
            this.VariableDef = new VariableAssignStatement();
            this.Name = "";
        }
    }
}
