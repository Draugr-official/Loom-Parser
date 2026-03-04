using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGen.AST.Expressions
{
    internal class LenExpression : Expression
    {
        public Expression Identifier { get; set; } = new Expression();
    }
}
