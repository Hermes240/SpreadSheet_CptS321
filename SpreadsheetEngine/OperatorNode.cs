/// <summary>
/// Programmer: Hermes Obiang
/// Class: CptS 321
/// Date: March 28, 2019
/// Programming Assignment: Expression Tree
/// </summary>

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal  class OperatorNode: BaseNode
    {
        public OperatorNode(string c)
        {
            Operator = c;
            Left = Right = null;
        }

        public string Operator { get; set; }

        public BaseNode Left { get; set; }
        public BaseNode Right { get; set; }

        public override double Evaluate()
        {
            switch(this.Operator)
            {
                case "-":
                    return this.Left.Evaluate() - this.Right.Evaluate();
                case "+":
                    return this.Left.Evaluate() + this.Right.Evaluate();
                case "*":
                    return this.Left.Evaluate() * this.Right.Evaluate();
                case "/":
                    return this.Left.Evaluate() / this.Right.Evaluate();
                default:
                    return 0;
                

            }
      
        }
    }
}
