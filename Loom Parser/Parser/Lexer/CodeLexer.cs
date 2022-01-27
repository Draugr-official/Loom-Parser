using Loom_Parser.Parser.Lexer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* This lexer is specified for Lua */

namespace Loom_Parser.Parser.Lexer
{
    class CodeLexer
    {
        readonly List<string> Keywords = new List<string>
        {
            "and", 
            "break", 
            "do", 
            "else", 
            "elseif", 
            "end", 
            "false", 
            "for", 
            "function", 
            "if", 
            "in", 
            "local", 
            "nil", 
            "not", 
            "or", 
            "repeat", 
            "return", 
            "then", 
            "true", 
            "until", 
            "while"
        };

        string Input { get; set; }

        public CodeLexer(string Input)
        {
            this.Input = Input;
        }

        LexKind Identify(string Value)
        {
            if (ulong.TryParse(Value, out _))
                return LexKind.Number;

            if (Value == "false"
                || Value == "true")
                return LexKind.Boolean;

            if (Keywords.Contains(Value))
                return LexKind.Keyword;

            return LexKind.Identifier;
        }

        public LexTokenList Analyze()
        {
            LexTokenList LexTokens = new LexTokenList();
            StringBuilder sb = new StringBuilder();
            int Line = 1;

            for (int i = 0; i < Input.Length; i++)
            {
                LexKind kind = LexKind.Terminal;
                string value = "";
                switch (Input[i])
                {
                    case '(': kind = LexKind.ParentheseOpen; break;
                    case ')': kind = LexKind.ParentheseClose; break;

                    case '[': kind = LexKind.BracketOpen; break;
                    case ']': kind = LexKind.BracketClose; break;

                    case '{': kind = LexKind.BraceOpen; break;
                    case '}': kind = LexKind.BraceClose; break;

                    case '<':
                        {
                            if(Input[i + 1] == '=')
                            {
                                kind = LexKind.SmallerOrEqual;
                                i++;
                            }
                            else
                            {
                                kind = LexKind.ChevronOpen;
                            }
                            break;
                        }
                    case '>':
                        {
                            if (Input[i + 1] == '=')
                            {
                                kind = LexKind.BiggerOrEqual;
                                i++;
                            }
                            else
                            {
                                kind = LexKind.ChevronClose;
                            }
                            break;
                        }


                    case ';': kind = LexKind.Semicolon; break;

                    case ',': kind = LexKind.Comma; break;

                    case '.':
                        {
                            if(Input[i + 1] == '.')
                            {
                                kind = LexKind.Concat;
                            }
                            break;
                        }

                    case '"':
                        {
                            i++;
                            while (Input[i] != '"')
                            {
                                if (Input[i] == '\\' && Input[i + 1] == '"')
                                {
                                    i++;
                                }
                                sb.Append(Input[i]);
                                i++;
                            }
                            value = sb.ToString();
                            kind = LexKind.String;
                            sb.Clear();

                            break;
                        }

                    case '\'':
                        {
                            i++;
                            while (Input[i] != '\'')
                            {
                                if (Input[i] == '\\' && Input[i + 1] == '\'')
                                {
                                    i++;
                                }
                                sb.Append(Input[i]);
                                i++;
                            }
                            value = sb.ToString();
                            kind = LexKind.String;
                            sb.Clear();

                            break;
                        }

                    case '=':
                        {
                            if(Input[i + 1] == '=')
                            {
                                kind = LexKind.EqualTo;
                                i++;
                            }
                            else
                            {
                                kind = LexKind.Equals;
                            }
                            break;
                        }

                    case '~':
                        {
                            if (Input[i + 1] == '=')
                            {
                                kind = LexKind.NotEqualTo;
                                i++;
                            }
                            break;
                        }


                    case '\n':
                        Line++;
                        break;

                    case '?':
                        {
                            kind = LexKind.Question;
                            break;
                        }

                    case '!':
                        {
                            kind = LexKind.Exclamation;
                            break;
                        }

                    case '+':
                        {
                            kind = LexKind.Add;
                            break;
                        }
                    case '-':
                        {
                            if (Input.Length > i + 1 && Input[i + 1] == '-')
                            {
                                i += 2;
                                for (; Input[i] != '\n' && i < Input.Length; i++)
                                {
                                    sb.Append(Input[i]);
                                }
                                value = sb.ToString();
                                kind = LexKind.Comment;
                                sb.Clear();
                                Line++;
                            }
                            else
                            {
                                kind = LexKind.Sub;
                            }
                            break;
                        }
                    case '*':
                        {
                            kind = LexKind.Mul;
                            break;
                        }
                    case '/':
                        {
                            
                            kind = LexKind.Div;
                            break;
                        }
                    case '%':
                        {
                            kind = LexKind.Mod;
                            break;
                        }
                    case '^':
                        {

                            kind = LexKind.Exp;
                            break;
                        }

                    // Discard
                    case ' ':
                    case '\r':
                    case '\t':
                        break;

                    default:
                        {
                            if (Char.IsLetterOrDigit(Input[i])
                                || Input[i] == '.'
                                || Input[i] == '_'
                                || Input[i] == ':')
                            {
                                while (Input.Length > i && (Char.IsLetterOrDigit(Input[i]) || Input[i] == '.' || Input[i] == ':' || Input[i] == '_'))
                                {
                                    sb.Append(Input[i++]);
                                }
                                i--;
                            }
                            value = sb.ToString();
                            kind = Identify(value);

                            sb.Clear();
                            break;
                        }
                }


                if (kind != LexKind.Terminal)
                {
                    LexTokens.Add(new LexToken()
                    {
                        Kind = kind,
                        Value = value,
                        Line = Line
                    });
                }
            }

            LexTokens.Add(new LexToken()
            {
                Kind = LexKind.EOF
            });

            return LexTokens;
        }
    }
}
