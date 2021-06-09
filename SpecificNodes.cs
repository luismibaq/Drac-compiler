/*
  Drac compiler - Specific node subclasses for the AST (Abstract
  Syntax Tree).
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM

  Copyright (C) 2021
  Luis Enrique Neri Pérez - A01745995
  Luis Miguel Baqueiro Vallejo - A01745997

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

    class Blanc: Node {} // Node that need to be deleted in the next phace

    class ProgramN: Node {} //First node, 2 child nodes VarList and FunList

    class VarList: Node {} // N child nodes VarDef

    class VarDef: Node {} // Any variable (TokenCategory.VAR)

    class FunList: Node {} //N child nodes FunDef

    class FunDef: Node {} // 2 child nodes VarList and StatementList (TokenCategory.IDENTIFIER)

    class StatementList: Node{} // N child nodes (FunCall, Assignment, Id, Inc, Dec, If, While, Do, Break, Return)

    class FunCall: Node {} // 1 node ExprList

    class Assignment: Node {} // 1 node Expr()

    class Var: Node {} // One child node ExprList (TokenCategory.IDENTIFIER)

    class VarParam: Node {} // One child node ExprList (TokenCategory.IDENTIFIER)

    class ExprList: Node {} // N child nodes Expr

    class DeclarationList: Node {} // N child nodes local VarDef's

    class Inc: Node {} // One child node Id (TokenCategory.INC)

    class Dec: Node {} // One child node Id (TokenCategory.DEC)

    class If: Node {} // 3 child nodes ExprList, StatementList, Else and N child nodes Elseif (TokenCategory.IF)

    class Elseif: Node {} //  2 child nodes ExprList and StatementList (TokenCategory.ELIF)

    class Else: Node {} // One child node StatementList (TokenCategory.ELSE)

    class While: Node {} //  2 child nodes ExprList and StatementList (TokenCategory.WHILE)

    class Do: Node {} //  2 child nodes StatementList and Expr (TokenCategory.DO)

    class Break: Node {} //  TokenCategory.BREAK

    class Return: Node {} //  One child node ExprList (TokenCategory.RETURN)

    class Or: Node {} //  2 child nodes And|OR (TokenCategory.OR)

    class And: Node {} //  2 child nodes Comp|And (TokenCategory.AND)

    class Comp: Node {} //  2 child nodes Rel|Comp (TokenCategory.COMPARE)

    class Dif: Node {} //  2 child nodes Rel|Comp (TokenCategory.DIFERENT)

    class LessT: Node {} //  2 child nodes Add|Rel (TokenCategory.LESS_T)

    class LessE: Node {} //  2 child nodes Add|Rel (TokenCategory.LESS_E)

    class MoreT: Node {} //  2 child nodes Add|Rel (TokenCategory.MORE_T)

    class MoreE: Node {} //  2 child nodes Add|Rel (TokenCategory.MORE_E)

    class Add: Node {} //  2 child nodes Mul|Add (TokenCategory.PLUS)

    class Minus: Node {} //  2 child nodes Mul1Div|Add|Minus (TokenCategory.MINUS)

    class Mul: Node {} //  2 child nodes Uplus|UMinus|UNot|Mul|Div|Mod (TokenCategory.MULTIPLY)

    class Div: Node {} //  2 child nodes Uplus|UMinus|UNot|Mul|Div|Mod (TokenCategory.DIV)

    class Mod: Node {} //  2 child nodes Uplus|UMinus|UNot|Mul|Mod (TokenCategory.MODULE)

    class UPlus: Node {} // One child node Uplus|UMinus|UNot|True|False|IntLit|CharLit|StringLit (TokenCategory.PLUS)

    class UMinus: Node {} // One child node Uplus|UMinus|UNot|True|False|IntLit|CharLit|StringLit (TokenCategory.MINUS)

    class UNot: Node {} // One child node Uplus|UMinus|UNot|True|False|IntLit|CharLit|StringLit (TokenCategory.NOT)

    class True: Node {} //  TokenCategory.TRUE

    class False: Node {} //  TokenCategory.FALSE

    class IntLit: Node {} //  TokenCategory.INT_LIT

    class CharLit: Node {} //  TokenCategory.CHAR_LIT

    class StringLit: Node {} //  TokenCategory.STRING_LIT
}
