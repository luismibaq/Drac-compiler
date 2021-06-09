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

namespace Drac
{
    class SemanticVisitor
    {

        public HashSet<string> GlobalVariableTable, ScopeSymbolsTable;
        public IDictionary<string, int> GlobalFunctionTable;
        public IDictionary<string, bool> GlobalPrimitiveFunctionTable;
        public IDictionary<string, HashSet<string>> LocalSymbolTable = new SortedDictionary<string, HashSet<string>>();
        public int pass, loopDepth;
        public bool functionBody;
        public bool decbody;


        void DeclareAPI()
        {
            GlobalFunctionTable["printi"] = 1;
            GlobalFunctionTable["printc"] = 1;
            GlobalFunctionTable["prints"] = 1;
            GlobalFunctionTable["println"] = 0;
            GlobalFunctionTable["readi"] = 0;
            GlobalFunctionTable["reads"] = 0;
            GlobalFunctionTable["new"] = 1;
            GlobalFunctionTable["size"] = 1;
            GlobalFunctionTable["add"] = 2;
            GlobalFunctionTable["get"] = 2;
            GlobalFunctionTable["set"] = 3;

            GlobalPrimitiveFunctionTable["printi"] = true;
            GlobalPrimitiveFunctionTable["printc"] = true;
            GlobalPrimitiveFunctionTable["prints"] = true;
            GlobalPrimitiveFunctionTable["println"] = true;
            GlobalPrimitiveFunctionTable["readi"] = true;
            GlobalPrimitiveFunctionTable["reads"] = true;
            GlobalPrimitiveFunctionTable["new"] = true;
            GlobalPrimitiveFunctionTable["size"] = true;
            GlobalPrimitiveFunctionTable["add"] = true;
            GlobalPrimitiveFunctionTable["get"] = true;
            GlobalPrimitiveFunctionTable["set"] = true;
        }

        public SemanticVisitor()
        {
            pass = 1;
            loopDepth = 0;
            decbody = false;

            // Create Variable Table
            GlobalVariableTable = new HashSet<string>();
            // Create Function Table
            GlobalFunctionTable = new SortedDictionary<string, int>();
            GlobalPrimitiveFunctionTable = new SortedDictionary<string, bool>();
            // Declare API functions
            DeclareAPI();
        }


        public void VisitBinaryOperator(string op, Node node) {
            VisitChildren(node);
        }

        void VisitChildren(Node node){
            foreach (var n in node)
            {
                Visit((dynamic)n);
            }

        }
        public void Visit(ProgramN node)
        {

            // FIRST PASS ------------------------------------------------------------------
            // 1) Visit AST and register functions and global variables. Omit function bodies.
            VisitChildren(node);
            // Validation 2
            if (!GlobalFunctionTable.ContainsKey("main"))
            {
                throw new SemanticError("No main function declared");
            }

            // Pass 2
            pass = 2;
            VisitChildren(node);
        }
        public void Visit(VarList node){
            VisitChildren(node);
        }
        public void Visit(DeclarationList node){
            decbody = true;
            VisitChildren(node);
            decbody = false;
        }

        public void Visit(VarDef node){
            var variableName = node.AnchorToken.Lexeme;
            if(pass == 1){
                if(GlobalVariableTable.Contains(variableName)){
                        throw new SemanticError("Duplicated variable: " + variableName, node.AnchorToken);
                }
                GlobalVariableTable.Add(variableName);

            }else{

                if(!GlobalVariableTable.Contains(variableName)){
                    if(ScopeSymbolsTable.Contains(variableName)){
                            throw new SemanticError("Duplicated variable: " + variableName, node.AnchorToken);
                    }
                    ScopeSymbolsTable.Add(variableName);
                }else{
                  if(decbody){
                    ScopeSymbolsTable.Add(variableName);
                  }

                }

            }
        }

        public void Visit(FunList node)
        {
            VisitChildren(node);
        }

        public void Visit(FunDef node)
        {

            var functionName = node.AnchorToken.Lexeme;
            functionBody = false;

            if(pass == 1){
                if(GlobalFunctionTable.ContainsKey(functionName)){
                    throw new SemanticError("Duplicated function: " + functionName,node.AnchorToken);

                }else{
                    if(node[0] is ExprList){
                        int arity = 0;
                        foreach (var id in node[0]){
                            arity++;
                        }
                        GlobalFunctionTable[functionName] = arity;
                        GlobalPrimitiveFunctionTable[functionName] = false;
                    }
                    else{
                        GlobalFunctionTable[functionName] = 0;
                        GlobalPrimitiveFunctionTable[functionName] = false;
                    }

                }
            }else{
                ScopeSymbolsTable = new HashSet<string>();

                VisitChildren(node);

                LocalSymbolTable[functionName] = ScopeSymbolsTable;
                ScopeSymbolsTable = new HashSet<string>();
            }

        }
        public void Visit(StatementList node)
        {
            functionBody = true;
            VisitChildren(node);
        }
        public void Visit(FunCall node)
        {

            var functionName = node.AnchorToken.Lexeme;

            if(GlobalFunctionTable.ContainsKey(functionName) && GlobalFunctionTable[functionName] > 0){
                if(node[0] is ExprList){
                    int args = 0;
                    foreach (var id in node[0]){
                        args++;
                    }
                    if(GlobalFunctionTable[functionName] != args){
                        throw new SemanticError("Number of arguments mismatch: " + functionName,node.AnchorToken);
                    }
                }
            }
            VisitChildren(node);
        }
        public void Visit(Assignment node)
        {
            var variableName = node.AnchorToken.Lexeme;
            if(!ScopeSymbolsTable.Contains(variableName) && !GlobalVariableTable.Contains(variableName)){
                throw new SemanticError("Undeclared variable: " + variableName, node[0].AnchorToken);
            }
            VisitChildren(node);

        }
        public void Visit(Var node)
        {
            if(!functionBody){
                var variableName = node.AnchorToken.Lexeme;

                if( ScopeSymbolsTable.Contains(variableName)){
                    throw new SemanticError("Duplicated variable: " + variableName, node.AnchorToken);
                }
                else{
                    ScopeSymbolsTable.Add(variableName);
                }
                VisitChildren((dynamic) node);
            }

        }
        public void Visit(ExprList node)
        {
            VisitChildren(node);
        }
        public void Visit(Inc node)
        {
            VisitChildren(node);
        }
        public void Visit(Dec node)
        {
            VisitChildren(node);
        }
        public void Visit(If node)
        {
            VisitChildren(node);
        }
        public void Visit(Elseif node)
        {
            VisitChildren(node);
        }
        public void Visit(Else node)
        {
            VisitChildren(node);
        }
        public void Visit(While node)
        {
            loopDepth++;
            VisitChildren(node);
            loopDepth--;
        }
        public void Visit(Do node)
        {
            loopDepth++;
            VisitChildren(node);
            loopDepth--;
        }

        public void Visit(Break node)
        {
            // Validation 12
            if(loopDepth == 0){
                throw new SemanticError("Found Break outside loop declaration", node.AnchorToken);
            }
        }
        public void Visit(Return node)
        {
            VisitChildren(node);
        }
        public void Visit(Or node)
        {
            // Or | And
            VisitBinaryOperator("or", node);
        }
        public void Visit(And node)
        {
            // Comp | And
            VisitBinaryOperator("and", node);
        }
        public void Visit(Comp node)
        {
            // Rel | Comp
            VisitBinaryOperator("==", node);
        }
        public void Visit(Dif node)
        {
            // Rel | Comp
            VisitBinaryOperator("!=", node);
        }
        public void Visit(LessT node)
        {
            // Add | Rel
            VisitBinaryOperator("<", node);
        }
        public void Visit(LessE node)
        {
            // Add | Rel
            VisitBinaryOperator("<=", node);
        }
        public void Visit(MoreT node)
        {
            // Add | Rel
            VisitBinaryOperator(">", node);
        }
        public void Visit(MoreE node)
        {
            // Add | Rel
            VisitBinaryOperator(">=", node);
        }
        public void Visit(Add node)
        {
            // Mul|Add
            VisitBinaryOperator("+", node);
        }
        public void Visit(Minus node)
        {
            // Mul|Div|Add|Minus
            VisitBinaryOperator("-", node);
        }
        public void Visit(Mul node)
        {
            // Uplus|UMinus|UNot|Mul|Div|Mod
            VisitBinaryOperator("*", node);
        }
        public void Visit(Div node)
        {
            // Uplus|UMinus|UNot|Mul|Mod
            VisitBinaryOperator("/", node);
        }
        public void Visit(Mod node)
        {
            // UUplus|UMinus|UNot|Mul|Mod
            VisitBinaryOperator("%", node);
        }
        public void Visit(UPlus node)
        {
            // Uplus|UMinus|UNot|True|False|IntLit|CharLit|StringLit
            VisitChildren(node);
        }
        public void Visit(UMinus node)
        {
            // Uplus|UMinus|UNot|True|False|IntLit|CharLit|StringLit
            VisitChildren(node);
        }
        public void Visit(UNot node)
        {
            // Uplus|UMinus|UNot|True|False|IntLit|CharLit|StringLit
            VisitChildren(node);
        }

        public void Visit(True node){
            var lexeme = node.AnchorToken.Lexeme;
            try {
                //Convert.ToInt32(lexeme);

            }catch (OverflowException) {
                throw new SemanticError("Boolean literal too large: " + lexeme, node.AnchorToken);
            }
        }
        public void Visit(False node){
            var lexeme = node.AnchorToken.Lexeme;
            try {
                //Convert.ToInt32(lexeme);

            }catch (OverflowException) {
                throw new SemanticError("Boolean literal too large: " + lexeme, node.AnchorToken);
            }
        }
        public void Visit(IntLit node){
            var lexeme = node.AnchorToken.Lexeme;
            try {
                Convert.ToInt32(lexeme);

            }catch (OverflowException) {
                throw new SemanticError("Integer literal too large: " + lexeme, node.AnchorToken);
            }
        }
        public void Visit(CharLit node){
            var lexeme = node.AnchorToken.Lexeme;
            try {
                //Convert.ToInt32(lexeme);

            }catch (OverflowException) {
                throw new SemanticError("Char literal too large: " + lexeme, node.AnchorToken);
            }
        }
        public void Visit(StringLit node){
            var lexeme = node.AnchorToken.Lexeme;
            try {
                //ToInt32(lexeme);

            }catch (OverflowException) {
                throw new SemanticError("String literal too large: " + lexeme, node.AnchorToken);
            }
        }
    }
}
