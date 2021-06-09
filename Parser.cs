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


/*
 * DracDestroyer LL(1) Grammar:
 *
 *      Program             ::= (VarDef | FunDef)* EOF
 *      VarDef              ::= "var" IDList ";"
 *      IDList              ::= ID ("," ID)*
 *      FunDef              ::= ID "(" IDList? ")" "{" VarDef* StmtList "}"
 *      StmtList            ::= Stmt*
 *      Stmt                ::= StmtAssign | StmtIncr | StmtDecr | X | StmtIf | StmtWhile | StmtDoWhile | StmtBreak | StmtReturn | StmtEmpty
 *      StmtAssign          ::= ID "=" Expr
 *      StmtIncr            ::= "inc" ID ";"
 *      StmtDecr            ::= "dec" ID ";"
 *      StmtFunCall         ::= FunCall ";"
 *      FunCall             ::= ID "(" ExprList? ")"
 *      ExprList            ::= Expr ("," Expr)*
 *      StmtIf              ::= "if" "(" Expr ")" "{" StmtList? "}" ElseIfList? Else?
 *      ElseIfList          ::= ("elif" "(" Expr ")" "{" StmtList? "}")*
 *      Else                ::= "else" "{" StmtList "}"
 *      StmtWhile           ::= "while" "(" Expr ")" "{" StmtList "}"
 *      StmtDoWhile         ::= "do" "{" StmtList "}" "while" "(" Expr ")" ";"
 *      StmtBreak           ::= "break" ";"
 *      StmtReturn          ::= "return" Expr ";"
 *      StmtEmpty           ::= ";"
 *      Expr                ::= ExprOr
 *      ExprOr              ::= ExprAnd | ("or" ExprOr)*
 *      ExprAnd             ::= ExprComp | ("and" ExprComp)*
 *      ExprComp            ::= ExprRel | (OpComp ExprRel)*
 *      OpComp              ::= "==" | "<>"
 *      ExprRel             ::= ExprAdd |(OpRel ExprAdd)*
 *      OpRel               ::= "<" | "<=" | ">" | ">="
 *      ExprAdd             ::= ExprMul | (OpAdd ExprMul)*
 *      OpAdd               ::= "+" | "-"
 *      ExprMul             ::= ExprUnary | (OpMul ExprUnary)*
 *      OpMul               ::= "*" | "/" | "%"
 *      ExprUnary           ::= OpUnary* ExprPrimary
 *      OpUnary             ::= "+" | "-" | "not"
 *      ExprPrimary         ::= ID | FunCall | "[" ExprList"]" | LitBool | LitInt | LitChar | LitStr | "(" Expr ")"
 */

using System;
using System.Collections.Generic;

namespace Drac
{

    class Parser
    {
        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.RETURN,
                TokenCategory.ASSIGN,
                TokenCategory.INC,
                TokenCategory.DEC,
                TokenCategory.IF,
                TokenCategory.ELIF,
                TokenCategory.ELSE,
                TokenCategory.WHILE,
                TokenCategory.DO,
                TokenCategory.BREAK,
                TokenCategory.RETURN,
                TokenCategory.SEMICOLON,

            };

        static readonly ISet<TokenCategory> firstOfPrim =
            new HashSet<TokenCategory>() {
              TokenCategory.PAR_LEFT,
              TokenCategory.ASSIGN
        };
        static readonly ISet<TokenCategory> firstOfDeclaration =
            new HashSet<TokenCategory>() {
              TokenCategory.IDENTIFIER,
              TokenCategory.OPEN_SQUARE_BRACKET,
              TokenCategory.TRUE,
              TokenCategory.FALSE,
              TokenCategory.INT_LIT,
              TokenCategory.CHAR_LIT,
              TokenCategory.STRING_LIT,
              TokenCategory.PAR_LEFT
        };
        static readonly ISet<TokenCategory> firstOfOperatorComp =
            new HashSet<TokenCategory>() {
                TokenCategory.COMPARE,
                TokenCategory.DIFERENT
        };

        static readonly ISet<TokenCategory> firstOfOperatorRel =
            new HashSet<TokenCategory>() {
                TokenCategory.MORE_T,
                TokenCategory.MORE_E,
                TokenCategory.LESS_T,
                TokenCategory.LESS_E,
        };
        static readonly ISet<TokenCategory> firstOfOperatorAdd =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS
        };

        static readonly ISet<TokenCategory> firstOfOperatorMul =
            new HashSet<TokenCategory>() {
                TokenCategory.MULTIPLY,
                TokenCategory.DIV,
                TokenCategory.MODULE
        };
        static readonly ISet<TokenCategory> firstOfOperatorUnary =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS,
                TokenCategory.NOT
        };


        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream)
        {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken
        {
            get { return tokenStream.Current.Category; }
        }

        public Token Expect(TokenCategory category)
        {
            if (CurrentToken == category)
            {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            }
            else
            {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }

        public Node Program()
        {
            var result = new ProgramN();
            var varList = new VarList();
            var funList = new FunList();
            while (CurrentToken != TokenCategory.EOF)
            {
              switch (CurrentToken)
              {
                  case TokenCategory.VAR:
                      varList.AddAll(VarDef());
                      break;

                  case TokenCategory.IDENTIFIER:
                      funList.Add(FunDef());
                      break;
                  default:
                      throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
              }
            }
            Expect(TokenCategory.EOF);
            result.Add(varList);
            result.Add(funList);

            return result;
        }

        public Node VarDef()
        {
            Expect(TokenCategory.VAR);
            var result = IDList();
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node IDList()
        {
            var result = new VarList();
            var newNode = new VarDef();
            newNode.AnchorToken = Expect(TokenCategory.IDENTIFIER);
            result.Add(newNode);
            while (CurrentToken == TokenCategory.COMA)
            {
                newNode = new VarDef();
                Expect(TokenCategory.COMA);
                newNode.AnchorToken = Expect(TokenCategory.IDENTIFIER);
                result.Add(newNode);
            }
            return result;
        }

        public Node FunDef()
        {
            var result = new FunDef();
            var varList = new DeclarationList();
            result.AnchorToken = Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PAR_LEFT);

            if (CurrentToken == TokenCategory.IDENTIFIER)
            {
                result.Add(ExprList());
            }else{
              result.Add(new ExprList());
            }

            Expect(TokenCategory.PAR_RIGHT);
            Expect(TokenCategory.OPEN_CURLY_BRACKET);

            while (CurrentToken == TokenCategory.VAR)
            {
                varList.AddAll(VarDef());
            }
            result.Add(varList);

            result.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY_BRACKET);
            return result;
        }

        public Node StmtList()
        {
            var result = new StatementList();
            while (firstOfStatement.Contains(CurrentToken))
            {
                result.Add(Stmt());
            }
            return result;
        }

        public Node Stmt()
        {
            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                {
                    var idtoken = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.ASSIGN)
                    {
                        var result = new Assignment();
                        result.Add(StmtAssign());
                        result.AnchorToken = idtoken;
                        return result;
                    }
                    else if (CurrentToken == TokenCategory.PAR_LEFT)
                    {
                        var result = new FunCall();
                        result.Add(StmtFunCall());
                        result.AnchorToken = idtoken;
                        return result;
                    }
                    else
                    {
                        throw new SyntaxError(firstOfPrim, tokenStream.Current);
                    }
                  }

                case TokenCategory.INC:
                {
                    return StmtIncr();
                }

                case TokenCategory.DEC:
                {
                    return StmtDecr();
                }

                case TokenCategory.IF:
                {
                    return StmtIf();
                }

                case TokenCategory.WHILE:
                {
                    return StmtWhile();
                }

                case TokenCategory.DO:
                {
                    return StmtDoWhile();
                }

                case TokenCategory.BREAK:
                {
                    return StmtBreak();
                }

                case TokenCategory.RETURN:
                {
                    return StmtReturn();
                }

                case TokenCategory.SEMICOLON:
                {
                    return StmtEmpty();
                }
            }
            return null;
        }


        public Node StmtAssign()
        {
            Expect(TokenCategory.ASSIGN);
            var result = Expr();
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtFunCall()
        {
            var result = FunCall();
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node FunCall()
        {
            Expect(TokenCategory.PAR_LEFT);
            var result = ExprList();
            Expect(TokenCategory.PAR_RIGHT);
            return result;
        }

        public Node ExprList()
        {
            var result = new ExprList();
            if(CurrentToken != TokenCategory.PAR_RIGHT){
                result.Add(Expr());
            }

            while (CurrentToken == TokenCategory.COMA)
            {
                Expect(TokenCategory.COMA);
                result.Add(Expr());
            }
            if(result.childs() == 0){
              return null;
            }
            return result;
        }

        public Node StmtIncr()
        {
            var result = new Inc();
            result.AnchorToken = Expect(TokenCategory.INC);
            var newNode = new Var();
            newNode.AnchorToken = Expect(TokenCategory.IDENTIFIER);
            result.Add(newNode);
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtDecr()
        {
            var result = new Dec();
            result.AnchorToken = Expect(TokenCategory.DEC);
            var newNode = new Var();
            newNode.AnchorToken = Expect(TokenCategory.IDENTIFIER);
            result.Add(newNode);
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtIf()
        {
            var result = new If();
            result.AnchorToken = Expect(TokenCategory.IF);
            Expect(TokenCategory.PAR_LEFT);
            result.Add(Expr());
            Expect(TokenCategory.PAR_RIGHT);
            Expect(TokenCategory.OPEN_CURLY_BRACKET);
            result.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY_BRACKET);
            if (CurrentToken == TokenCategory.ELIF)
            {
                result.Add(ElseIfList());
            }
            else if (CurrentToken == TokenCategory.ELSE)
            {
                result.Add(Else());
            }
            return result;
        }

        public Node ElseIfList()
        {
            var NewNode = new Elseif();
            NewNode.AnchorToken = Expect(TokenCategory.ELIF);
            Expect(TokenCategory.PAR_LEFT);
            NewNode.Add(Expr());
            Expect(TokenCategory.PAR_RIGHT);
            Expect(TokenCategory.OPEN_CURLY_BRACKET);
            NewNode.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY_BRACKET);
            if (CurrentToken == TokenCategory.ELIF)
            {
                NewNode.Add(ElseIfList());
            }
            else if (CurrentToken == TokenCategory.ELSE)
            {
                NewNode.Add(Else());
            }
            return NewNode;
        }

        public Node Else()
        {
            var result = new Else();
            result.AnchorToken = Expect(TokenCategory.ELSE);
            Expect(TokenCategory.OPEN_CURLY_BRACKET);
            result.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY_BRACKET);
            return result;
        }

        public Node StmtWhile()
        {
            var result = new While();
            result.AnchorToken = Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PAR_LEFT);
            result.Add(Expr());
            Expect(TokenCategory.PAR_RIGHT);
            Expect(TokenCategory.OPEN_CURLY_BRACKET);
            result.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY_BRACKET);
            return result;
        }

        public Node StmtDoWhile()
        {
            var result = new Do();
            result.AnchorToken = Expect(TokenCategory.DO);
            Expect(TokenCategory.OPEN_CURLY_BRACKET);
            result.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY_BRACKET);

            var newNode = new While();
            newNode.AnchorToken = Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PAR_LEFT);
            newNode.Add(Expr());
            result.Add(newNode);
            Expect(TokenCategory.PAR_RIGHT);
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtBreak()
        {
            var result = new Break();
            result.AnchorToken = Expect(TokenCategory.BREAK);
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtReturn()
        {
            var result = new Return();
            result.AnchorToken = Expect(TokenCategory.RETURN);
            result.Add(Expr());
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtEmpty()
        {
            Expect(TokenCategory.SEMICOLON);
            return null;
        }


        public Node Expr()
        {
            return ExprOr();
        }

        public Node ExprOr()
        {
            var result = ExprAnd();
            while (CurrentToken == TokenCategory.OR)
            {
                var newNode = new Or();
                newNode.AnchorToken = Expect(TokenCategory.OR);
                newNode.Add(result);
                newNode.Add(ExprAnd());
                result = newNode;
            }
            return result;
        }

        public Node ExprAnd()
        {
            var result = ExprComp();
            while (CurrentToken == TokenCategory.AND)
            {
                var newNode = new And();
                newNode.AnchorToken = Expect(TokenCategory.AND);
                newNode.Add(result);
                newNode.Add(ExprComp());
                result = newNode;
            }
            return result;
        }

        public Node ExprComp()
        {
            var result = ExprRel();
            while (firstOfOperatorComp.Contains(CurrentToken))
            {
                var newToken = OpComp();
                Node newNode;
                if(newToken.Category.ToString() == "COMPARE"){
                  newNode = new Comp();
                }
                else{
                  newNode = new Dif();
                }
                newNode.AnchorToken = newToken;
                newNode.Add(result);
                newNode.Add(ExprRel());
                result = newNode;
            }
            return result;
        }


        public Node ExprRel()
        {
            var result = ExprAdd();
            while (firstOfOperatorRel.Contains(CurrentToken))
            {
                var newToken = OpRel();
                Node newNode;
                if(newToken.Category.ToString() == "LESS_E"){
                  newNode = new LessE();
                }
                else if (newToken.Category.ToString() == "LESS_T") {
                  newNode = new LessT();
                }
                else if (newToken.Category.ToString() == "MORE_E") {
                  newNode = new MoreE();
                }
                else{
                  newNode = new MoreT();
                }
                newNode.AnchorToken = newToken;
                newNode.Add(result);
                newNode.Add(ExprAdd());
                result = newNode;
            }
            return result;
        }


        public Node ExprAdd()
        {
            var result = ExprMul();

            while (firstOfOperatorAdd.Contains(CurrentToken))
            {
                var newToken = OpAdd();
                Node newNode;
                if(newToken.Category.ToString() == "PLUS"){
                  newNode = new Add();
                }
                else{
                  newNode = new Minus();
                }
                newNode.AnchorToken = newToken;
                newNode.Add(result);
                newNode.Add(ExprMul());
                result = newNode;
            }
            return result;
        }

        public Node ExprMul()
        {
            var result = ExprUnary();
            while (firstOfOperatorMul.Contains(CurrentToken))
            {
                var newToken = OpMul();
                Node newNode;
                if(newToken.Category.ToString() == "MULTIPLY"){
                  newNode = new Mul();
                }
                else if (newToken.Category.ToString() == "DIV") {
                  newNode = new Div();
                }
                else{
                  newNode = new Mod();
                }
                newNode.AnchorToken = newToken;
                newNode.Add(result);
                newNode.Add(ExprUnary());
                result = newNode;
            }
            return result;
        }
        public Node ExprUnary()
        {
            if(firstOfOperatorUnary.Contains(CurrentToken)){
              var newToken = OpUnary();
              Node result;
              if(newToken.Category.ToString() == "PLUS"){
                result = new UPlus();
              }
              else if (newToken.Category.ToString() == "MINUS") {
                result = new UMinus();
              }
              else{
                result = new UNot();
              }
              result.AnchorToken = newToken;
              result.Add(ExprUnary());
              return result;
            }
            return ExprPrimary();
        }

        public Token OpComp()
        {

            switch (CurrentToken)
            {

                case TokenCategory.COMPARE:
                    return Expect(TokenCategory.COMPARE);

                case TokenCategory.DIFERENT:
                    return Expect(TokenCategory.DIFERENT);

                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }

        public Token OpRel()
        {

            switch (CurrentToken)
            {

                case TokenCategory.LESS_T:
                    return Expect(TokenCategory.LESS_T);

                case TokenCategory.LESS_E:
                    return Expect(TokenCategory.LESS_E);

                case TokenCategory.MORE_T:
                    return Expect(TokenCategory.MORE_T);

                case TokenCategory.MORE_E:
                    return Expect(TokenCategory.MORE_E);

                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }

        public Token OpAdd()
        {

            switch (CurrentToken)
            {

                case TokenCategory.PLUS:
                    return Expect(TokenCategory.PLUS);

                case TokenCategory.MINUS:
                    return Expect(TokenCategory.MINUS);

                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }


        public Token OpMul()
        {

            switch (CurrentToken)
            {

                case TokenCategory.MULTIPLY:
                    return Expect(TokenCategory.MULTIPLY);

                case TokenCategory.DIV:
                    return Expect(TokenCategory.DIV);

                case TokenCategory.MODULE:
                    return Expect(TokenCategory.MODULE);

                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }

        public Token OpUnary()
        {

            switch (CurrentToken)
            {

                case TokenCategory.PLUS:
                    return Expect(TokenCategory.PLUS);

                case TokenCategory.MINUS:
                    return Expect(TokenCategory.MINUS);

                case TokenCategory.NOT:
                    return Expect(TokenCategory.NOT);

                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }

        public Node ExprPrimary()
        {

            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                {

                    var idToken = Expect(TokenCategory.IDENTIFIER);

                    if (CurrentToken == TokenCategory.PAR_LEFT)
                    {
                        var result = new FunCall();
                        result.AnchorToken = idToken;
                        result.Add(FunCall());
                        return result;
                    }else{
                        var result = new Var
                        ();
                        result.AnchorToken = idToken;
                        return result;
                    }
                }

                case TokenCategory.OPEN_SQUARE_BRACKET:
                {
                    Expect(TokenCategory.OPEN_SQUARE_BRACKET);
                    var result = ExprList();
                    Expect(TokenCategory.CLOSE_SQUARE_BRACKET);
                    return result;
                }

                case TokenCategory.TRUE:
                {
                    var result = new True();
                    result.AnchorToken = Expect(TokenCategory.TRUE);
                    return result;
                }

                case TokenCategory.FALSE:
                {
                    var result = new False();
                    result.AnchorToken = Expect(TokenCategory.FALSE);
                    return result;
                }

                case TokenCategory.INT_LIT:
                {
                    var result = new IntLit();
                    result.AnchorToken = Expect(TokenCategory.INT_LIT);
                    return result;
                }

                case TokenCategory.CHAR_LIT:
                {
                    var result = new CharLit();
                    result.AnchorToken = Expect(TokenCategory.CHAR_LIT);
                    return result;
                }

                case TokenCategory.STRING_LIT:
                {
                  var result = new StringLit();
                  result.AnchorToken = Expect(TokenCategory.STRING_LIT);
                  return result;
                }

                case TokenCategory.PAR_LEFT:
                {
                  Expect(TokenCategory.PAR_LEFT);
                  var result = Expr();
                  Expect(TokenCategory.PAR_RIGHT);
                  return result;
                }

                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }
    }
}
