using Loom.Parser.ASTGenerator.AST.Statements;
using Loom.Parser.Lexer;
using Loom.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loom.Parser.Preprocessor
{
    internal class Preprocessor
    {
        LexTokenReader tokenReader { get; set; }

        public Preprocessor(LexTokenList lexTokens)
        {
            tokenReader = new LexTokenReader(lexTokens);
        }

        void RemoveComment()
        {
            if (tokenReader.Expect(LexKind.Sub)
                    && tokenReader.Expect(LexKind.Sub, 1))
            {
                bool isShortComment = false;

                if(tokenReader.Expect(LexKind.BracketOpen, 2)
                    && tokenReader.Expect(LexKind.BracketOpen, 3))
                {
                    isShortComment = true;
                }

                for (; ; )
                {
                    tokenReader.Remove();

                    if (tokenReader.Expect(LexKind.NewLine))
                    {
                        break;
                    }

                    if(isShortComment
                        && tokenReader.Expect(LexKind.BracketClose)
                        && tokenReader.Expect(LexKind.BracketClose, 1))
                    {
                        tokenReader.Remove();
                        tokenReader.Remove();
                        break;
                    }

                    if (tokenReader.Expect(LexKind.EOF))
                    {
                        break;
                    }
                }
            }
        }

        void RemoveComments()
        {
            for (; ; )
            {
                RemoveComment();

                tokenReader.Skip(1);
                if(tokenReader.Expect(LexKind.EOF))
                {
                    break;
                }
            }
        }

        void RemoveNewLine()
        {
            tokenReader.LexTokens.RemoveAll(token => token.Kind == LexKind.NewLine);
        }

        public LexTokenList Process()
        {
            RemoveComments();
            RemoveNewLine();

            return tokenReader.LexTokens;
        }
    }
}
