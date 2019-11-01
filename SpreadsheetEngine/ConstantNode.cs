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

    internal class ConstantNode:BaseNode
    {
        private double mValue;

        public ConstantNode(double value)
        {
            this.mValue = value;
        }
        public double Value { get; set; }

        public override double Evaluate()
        {
           
            return this.mValue;
        }
    }
}
