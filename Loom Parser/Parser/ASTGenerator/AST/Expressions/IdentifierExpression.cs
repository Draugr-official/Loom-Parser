using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loom.Parser.ASTGenerator.AST.Statements;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    /// <summary>
    /// Lua variable expression
    /// </summary>
    internal class IdentifierExpression : Expression
    {
        public string Identifier { get; set; } = "";
    }
}
