/// <summary>
/// Programmer: Hermes Obiang
/// Class: CptS 321
/// Date: March 28, 2019
/// Programming Assignment: Expression Tree
/// </summary>
/// 
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ExpressionTreeFactory
    {

        public static bool IsValidOperator(string op)
        {
            switch (op)
            {
                case "-":
                    return true;
                case "+":
                    return true;
                case "*":
                    return true;
                case "/":
                    return true;
            }

            return false;
        }
    }
}
