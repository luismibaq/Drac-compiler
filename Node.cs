/*
  Drac compiler - Parent Node class for the AST (Abstract Syntax Tree).
  Copyright (C) 2013-2020 Ariel Ortiz, ITESM CEM

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

    class Node: IEnumerable<Node> {

        IList<Node> children = new List<Node>();

        public int childs(){
          int index = 0;
          foreach(var child in children){
            index += 1;
          }
          return index;
        }

        public Node this[int index] {
            get {
                return children[index];
            }
        }

        public Token AnchorToken { get; set; }

        public void Add(Node node) {
          if(node != null){
            children.Add(node);
          }
        }

        public void AddAll(Node node){
          foreach(var child in node.children){
            this.Add(child);
          }
        }

        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return $"{GetType().Name} {AnchorToken}";
        }

        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }

        static void TreeTraversal(Node node, string indent, StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
}
