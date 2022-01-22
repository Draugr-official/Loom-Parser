using Loom_Parser.Parser.Lexer.ASTGen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.Lexer.ASTGen.AST.Statements
{
    /// <summary>
    /// Variable assigning statement | local a = "b" 
    /// </summary>
    class VariableAssignStatement : Statement
    {
        /// <summary>
        /// Determines if the variable is localized to a scope
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// The name of the variable being declared
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value being assigned to the variable
        /// </summary>
        public Expression Value { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isLocal"></param>
        public VariableAssignStatement(string name, Expression value, bool isLocal)
        {
            this.Name = name;
            this.Value = value;
            this.IsLocal = isLocal;
        }
    }
}
