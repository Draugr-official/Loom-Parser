using Loom_Parser.Parser.ASTGen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.ASTGen.AST.Statements
{
    class WhileStatement : Statement
    {
        /// <summary>
        /// The condition of the if statement
        /// </summary>
        public Expression Condition { get; set; }

        /// <summary>
        /// The body of the if statement
        /// </summary>
        public StatementList Body { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="body"></param>
        public WhileStatement(Expression condition, StatementList body)
        {
            this.Condition = condition;
            this.Body = body;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public WhileStatement()
        {
            this.Condition = new Expression();
            this.Body = new StatementList();
        }
    }
}
