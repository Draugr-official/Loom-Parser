using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    internal class DoStatement : Statement
    {
        public StatementList Body { get; set; }
    }
}
