using Loom.Parser.ASTGenerator.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.ASTGenerator.AST.Statements
{
    internal class GenericForStatement : ForStatement
    {
        /// <summary>
        /// The variable array in the generic for statement
        /// <para>
        /// Should only consist identifier expressions
        /// </para>
        /// </summary>
        public ArrayExpression VariableArray { get; set; }

        /// <summary>
        /// The iterator of the generic for statement
        /// </summary>
        public Expression Iterator { get; set; }

        public GenericForStatement()
        {
            VariableArray = new ArrayExpression();
            Iterator = new Expression();
        }
    }
}
