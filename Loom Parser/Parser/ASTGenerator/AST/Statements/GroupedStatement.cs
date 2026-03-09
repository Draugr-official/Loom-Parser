using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    internal class GroupedStatement : Statement
    {
        public Expression Expression { get; set; }
    }
}
