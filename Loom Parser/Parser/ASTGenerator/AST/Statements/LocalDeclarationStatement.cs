using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    internal class LocalDeclarationStatement : Statement
    {
        public Statement Statement { get; set; }

        /// <summary>
        /// If an unassigned declaration, uses an expression list
        /// </summary>
        public ExpressionList Expressions { get; set; }
    }
}
