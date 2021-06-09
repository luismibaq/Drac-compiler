/*
  Drac compiler - This class performs the lexical analysis,
  (a.k.a. scanning).


  Copyright (C) 2013-2021 Ariel Ortiz, ITESM CEM

  Copyright (C) 2021
  Luis Enrique Neri Pérez - A01745995
  Luis Miguel Baqueiro Vallejo - A01745997
  Francisco Javier Zavala Torres - A01746851

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Drac {

    class Scanner {

        readonly string input;

        static readonly Regex regex = new Regex(
            @"
                (?<Comment>    \-\-.*  )
              | (?<MultilineComment>    (\(\*(.|\n)*?\*\))   )
              | (?<Newline>    \n        )
              | (?<WhiteSpace> \s        )  
              | (?<CarriageReturn>    \r )
              | (?<Coma>       [,]       )
              | (?<Compare>    \=\=      )
              | (?<Different>  <>        )
              | (?<Div>        /         )
              | (?<LessE>      <=        )
              | (?<MoreE>      >=        )
              | (?<LessT>      <         ) 
              | (?<MoreT>      >         )
              | (?<OpenSquareBracket>      [[]       )
              | (?<CloseSquareBracket>     []]       )
              | (?<OpenCurlyBracket>       [{]       )
              | (?<CloseCurlyBracket>      [}]       )
              | (?<Semicolon>              [;]       )
              | (?<Assign>     =         )
              | (?<IntLit>     [-]?\d+       )
              | (?<CharLit>    '(\\n|\\r|\\t|\\'|\\\\|\\""|\\u[A-Fa-f\d]{6}|.)'  )
              | (?<StringLit>  "".*""    )
              | (?<Unicode>    \\u[A-Fa-f\d]{6}    )
              | (?<Multiply>   \*        )
              | (?<Backslash>  \\        )
              | (?<ParLeft>    \(        )
              | (?<ParRight>   \)        )
              | (?<Plus>       \+        )
              | (?<Simple>       \'        )
              | (?<Double>       \""        )
              | (?<Module>      %         )
              | (?<Minus>       -         )
              | (?<Identifier> [a-zA-Z][a-zA-Z0-9_]* )
              | (?<Other>       .         )
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> tokenMap =
            new Dictionary<string, TokenCategory>() {

                {"Comment", TokenCategory.COMMENT},
                {"MultilineComment", TokenCategory.MULTILINE_COMMENT},
                {"Newline", TokenCategory.MINUS},
                {"Tab", TokenCategory.TAB},
                {"CarriageReturn", TokenCategory.CARRIAGE_RETURN},
                {"WhiteSpace", TokenCategory.WHITE_SPACE},
                {"Simple", TokenCategory.SIMPLE},
                {"Double", TokenCategory.DOUBLE},
                {"Coma", TokenCategory.COMA},
                {"Compare", TokenCategory.COMPARE},
                {"Different", TokenCategory.DIFERENT},
                {"Div", TokenCategory.DIV},
                {"LessE", TokenCategory.LESS_E},
                {"MoreE", TokenCategory.MORE_E},
                {"MoreT", TokenCategory.MORE_T},
                {"LessT", TokenCategory.LESS_T},
                {"OpenSquareBracket", TokenCategory.OPEN_SQUARE_BRACKET},
                {"CloseSquareBracket", TokenCategory.CLOSE_SQUARE_BRACKET},
                {"OpenCurlyBracket", TokenCategory.OPEN_CURLY_BRACKET},
                {"CloseCurlyBracket", TokenCategory.CLOSE_CURLY_BRACKET},
                {"Semicolon", TokenCategory.SEMICOLON},
                {"Assign", TokenCategory.ASSIGN},
                {"IntLit", TokenCategory.INT_LIT},
                {"Multiply", TokenCategory.MULTIPLY},
                {"Backslash", TokenCategory.BACKSLASH},
                {"Unicode", TokenCategory.UNICODE},
                {"CharLit", TokenCategory.CHAR_LIT},
                {"StringLit", TokenCategory.STRING_LIT},
                {"ParLeft", TokenCategory.PAR_LEFT},
                {"ParRight", TokenCategory.PAR_RIGHT},
                {"Plus", TokenCategory.PLUS},
                {"Minus", TokenCategory.MINUS},
                {"Module", TokenCategory.MODULE},
                {"Identifier", TokenCategory.IDENTIFIER},
                {"Other", TokenCategory.IDENTIFIER}
            };

        static readonly IDictionary<string, TokenCategory> reservedMap =
            new Dictionary<string, TokenCategory>() {

                {"var", TokenCategory.VAR},
                {"break", TokenCategory.BREAK},
                {"return", TokenCategory.RETURN},
                {"dec", TokenCategory.DEC},
                {"do", TokenCategory.DO},
                {"if", TokenCategory.IF}, 
                {"elif", TokenCategory.ELIF}, 
                {"else", TokenCategory.ELSE},
                {"while", TokenCategory.WHILE},
                {"true", TokenCategory.TRUE},
                {"false", TokenCategory.FALSE},
                {"inc", TokenCategory.INC},
                {"not", TokenCategory.NOT}, 
                {"or", TokenCategory.OR}, 
                {"and", TokenCategory.AND},  

            };


        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Scan() {

            var result = new LinkedList<Token>();
            var row = 1;
            var columnStart = 0;

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    row++;
                    columnStart = m.Index + m.Length;

                }else if (m.Groups["MultilineComment"].Success) {

                    row += m.Value.Split('\n').Length - 1;

                }else if (m.Groups["WhiteSpace"].Success || m.Groups["Comment"].Success) {

                    // Skip white space and comments.

                } else if (m.Groups["Other"].Success) {

                    // Found an illegal character.
                    result.AddLast(
                        new Token(m.Value,
                            TokenCategory.ILLEGAL_CHAR,
                            row,
                            m.Index - columnStart + 1));

                }  else if (m.Groups["Identifier"].Success && reservedMap.ContainsKey(m.Value)) {
                    
                    // Found an illegal character.
                    result.AddLast(
                        new Token(m.Value,
                            reservedMap[m.Value],
                            row,
                            m.Index - columnStart + 1));

                } else {
                    // Must be any of the other tokens.
                    result.AddLast(FindToken(m, row, columnStart));
                }
            //Console.WriteLine($"hola \"{m.Value}\")");
            }
            result.AddLast(
                new Token(null,
                    TokenCategory.EOF,
                    row,
                    input.Length - columnStart + 1));

            return result;
        }

        Token FindToken(Match m, int row, int columnStart) {
            foreach (var name in tokenMap.Keys) {
                if (m.Groups[name].Success) {
                    return new Token(m.Value,
                        tokenMap[name],
                        row,
                        m.Index - columnStart + 1);
                }
            }
            throw new InvalidOperationException(
                "\nregex and tokenMap are inconsistent: " + m.Value + "\n");
        }
    }
}
