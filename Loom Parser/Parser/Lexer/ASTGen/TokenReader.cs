using Loom_Parser.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom_Parser.Parser.Lexer.ASTGen
{
    /// <summary>
    /// Generic token reader for the abstract syntax tree generator
    /// </summary>
    class TokenReader
    {
        /// <summary>
        /// The lexical tokens in the token reader
        /// </summary>
        public LexTokenList LexTokens { get; set; }

        /// <summary>
        /// The base of the token reader
        /// </summary>
        public int Base { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="lexTokens"></param>
        public TokenReader(LexTokenList lexTokens)
        {
            this.LexTokens = lexTokens;
        }

        /// <summary>
        /// Peeks into the lextoken list at the base + offset in the token reader
        /// </summary>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public LexToken Peek(int Offset = 0) => LexTokens[Base + Offset];

        /// <summary>
        /// Consumes current lex token and skips forward
        /// </summary>
        /// <returns></returns>
        public LexToken Consume() => LexTokens[Base++];

        /// <summary>
        /// Skils a token
        /// </summary>
        /// <param name="i"></param>
        public void Skip(int i) => Base += i;

        /// <summary>
        /// Expects a lex kind at the current + offset position
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool Expect(LexKind kind, int offset = 0) => (this.Base + offset <= this.LexTokens.Count - 1 && this.LexTokens[this.Base + offset].Kind == kind);
    }
}
