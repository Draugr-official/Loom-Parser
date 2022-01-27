using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Statements
{
    /// <summary>
    /// Statement superclass
    /// </summary>
    class Statement
    {
        /// <summary>
        /// The locals defined within the currrent scope
        /// </summary>
        public Dictionary<string, VariableAssignStatement> Locals = new Dictionary<string, VariableAssignStatement>();
    }
}
