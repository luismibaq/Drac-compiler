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
using System.Text;

namespace Drac {

    class SyntaxError: Exception {

        public SyntaxError(TokenCategory expectedCategory,
                           Token token):
            base($"Syntax Error: Expecting {expectedCategory} \n"
                 + $"but found {token.Category} (\"{token.Lexeme}\") at "
                 + $"row {token.Row}, column {token.Column}.") {
        }

        public SyntaxError(ISet<TokenCategory> expectedCategories,
                           Token token):
            base($"Syntax Error: Expecting one of {Elements(expectedCategories)}\n"
                 + $"but found {token.Category} (\"{token.Lexeme}\") at "
                 + $"row {token.Row}, column {token.Column}.") {
        }

        static string Elements(ISet<TokenCategory> expectedCategories) {
            var sb = new StringBuilder("{");
            var first = true;
            foreach (var elem in expectedCategories) {
                if (first) {
                    first = false;
                } else {
                    sb.Append(", ");
                }
                sb.Append(elem);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
