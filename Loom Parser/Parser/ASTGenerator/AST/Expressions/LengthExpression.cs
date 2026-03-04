using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    internal class LengthExpression : Expression
    {
        public Expression Identifier { get; set; } = new Expression();
    }
}
