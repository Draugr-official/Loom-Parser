using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Expressions
{
    public class MemberExpression : Expression
    {
        public Expression Parent { get; set; }

        public Expression Expression { get; set; }

        public bool IsInvoke { get; set; }
    }
}
