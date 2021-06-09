/*
  Drac compiler - Token categories for the scanner.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM

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

namespace Drac {

    enum TokenCategory {
        SIMPLE,
        DOUBLE,
        COMMENT,
        MULTILINE_COMMENT,
        NEWLINE,
        WHITE_SPACE,
        CARRIAGE_RETURN,
        TAB,
        VAR,
        BREAK,
        RETURN,
        DEC,
        DO,
        IF,
        ELIF,
        ELSE,
        WHILE,
        FALSE,
        TRUE,
        INC,
        NOT,
        OR,
        AND,
        COMA,
        COMPARE,
        DIFERENT,
        DIV,
        LESS_E,
        MORE_E,
        MORE_T,
        LESS_T,
        OPEN_SQUARE_BRACKET,
        CLOSE_SQUARE_BRACKET,
        OPEN_CURLY_BRACKET,
        CLOSE_CURLY_BRACKET,
        EOF,
        SEMICOLON,
        ASSIGN,
        INT_LIT,
        MULTIPLY,
        BACKSLASH,
        UNICODE,
        CHAR_LIT,
        STRING_LIT,
        PAR_LEFT,
        PAR_RIGHT,
        PLUS,
        MINUS,
        MODULE,
        IDENTIFIER,
        ILLEGAL_CHAR,
        OTHER
    }
}
