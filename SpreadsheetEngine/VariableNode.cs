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

    internal class VariableNode: BaseNode
    {
        private string mName;
        private double mValue;

        /// <summary>
        /// Default constructor
        /// </summary>
        public VariableNode()
        {
            this.mName = string.Empty;
            this.mValue = 0.0;
        }

        
        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="variableName"></param>
        public VariableNode(string variableName)
        {
            this.mName = variableName;
        }

        public string Name { get; set; }

        public override double Evaluate()
        {           

            return ExpressionTree.DICT[this.mName];

        }
    }
}
