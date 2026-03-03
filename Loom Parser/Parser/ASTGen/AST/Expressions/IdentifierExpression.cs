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
    internal class IdentifierExpression : Expression
    {
        public string Identifier { get; set; } = "";
    }
}
