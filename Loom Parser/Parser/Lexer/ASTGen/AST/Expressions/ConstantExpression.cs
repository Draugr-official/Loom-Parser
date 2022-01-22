using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.Lexer.ASTGen.AST.Expressions
{
    /// <summary>
    /// Defines a constant expression inside a scope
    /// </summary>
    class ConstantExpression : Expression
    {
        /// <summary>
        /// The value of the constant
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The datatype of the constant
        /// </summary>
        public DataTypes Type { get; set; }

    }
}
