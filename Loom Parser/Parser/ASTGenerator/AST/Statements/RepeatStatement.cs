using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    internal class RepeatStatement : Statement
    {
        public Expression Condition { get; set; }

        public StatementList Body { get; set; }
    }
}
