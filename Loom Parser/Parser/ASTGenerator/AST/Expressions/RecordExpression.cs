using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    public class RecordExpression : Expression
    {
        public Expression Expression { get; set; }
    }
}
