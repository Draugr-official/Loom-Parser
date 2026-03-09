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

        bool RemoveComment()
        {
            if (tokenReader.Expect(LexKind.Sub)
                    && tokenReader.Expect(LexKind.Sub, 1))
            {
                bool isLongComment = false;

                if(tokenReader.Expect(LexKind.BracketOpen, 2)
                    && tokenReader.Expect(LexKind.BracketOpen, 3))
                {
                    isLongComment = true;
                }

                for (; ; )
                {
                    tokenReader.Remove();

                    if (!isLongComment && tokenReader.Expect(LexKind.NewLine))
                    {
                        break;
                    }

                    if(isLongComment
                        && tokenReader.Expect(LexKind.BracketClose)
                        && tokenReader.Expect(LexKind.BracketClose, 1))
                    {
                        tokenReader.Remove();
                        tokenReader.Remove();
                        break;
                    }

                    if (tokenReader.Expect(LexKind.EOF))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void RemoveComments()
        {
            for (; ; )
            {
                if(RemoveComment())
                {
                    break;
                }

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
