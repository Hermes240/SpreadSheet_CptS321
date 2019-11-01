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

    /// <summary>
    /// This is the main class. It uses all of the other classes.
    /// </summary>
    /// 
    public class ExpressionTree
    {
        /// <summary>
        /// Private fields
        /// </summary>
        private string expression;
        private BaseNode mRoot;
        private static Dictionary<string, double> mVariables = new Dictionary<string, double>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExpressionTree()
        {
            this.expression = string.Empty;
            this.mRoot = null;
            this.mRoot = ConstructTree(this.expression);
        }

        /// <summary>
        /// Parameterized constructor. It construct the tree
        /// </summary>
        /// <param name="expression"></param>
        public ExpressionTree(string expression)
        {
            this.expression = expression;
            this.expression = expression.Replace(" ", "");
            this.mRoot = ConstructTree(this.expression);

        }

        /// <summary>
        /// Property. It returns a dictionary
        /// </summary>
        public static Dictionary<string, double> DICT { get { return mVariables; } }

        /// <summary>
        /// Returns a list of all variable names in the expression
        /// </summary>
        /// <returns></returns>
        public List<string> GetVariableNames()
        {
            List<string> variableNames = new List<string>(DICT.Keys);

            return variableNames;
        }

        /// <summary>
        /// This function contructs a tree using a stack
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private BaseNode ConstructTree(string expression)
        {
            List<string> Postfix = new List<string>();
            Postfix = this.Shunting_Yard(expression);

            //Initialize a node
            OperatorNode T;
            ConstantNode T1;
            VariableNode T2;
            //declare a stack of nodes

            Stack<BaseNode> TreeStack = new Stack<BaseNode>();

            try
            {
                double number;

                foreach (string s in Postfix)
                {
                    //check if it is an operand and push it into the stack
                    if (double.TryParse(s, out number))
                    {
                        // Node representing a constant numerical value
                        T1 = new ConstantNode(number);
                        TreeStack.Push(T1);
                    }

                    else if (ExpressionTreeFactory.IsValidOperator(s) == true)
                    {
                        // if operator, construct a new node
                        T = new OperatorNode(s);

                        //pop two nodes from the stack and set it to left and right of the operator

                        T.Right = TreeStack.Pop();
                        T.Left = TreeStack.Pop();

                        TreeStack.Push(T);
                    }

                    // Node representing a variable
                    else
                    {

                        mVariables[s] = 0; // add variable to the dictionary
                        T2 = new VariableNode(s);
                        TreeStack.Push(T2);
                    }
                }

                //set the element at top of the stack as the root of the tree
                T = (OperatorNode)TreeStack.Peek();
                TreeStack.Pop();

                //return tree
                return T;

            }
            catch
            {
                Console.WriteLine("Unable to construct tree");
                Console.WriteLine("Hermes");

                return null;


            }
        }

        /// <summary>
        /// sets the specified variable within the Expression tree variable dictionary
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="variableValue"></param>
        public bool SetVariables(string variableName, double variableValue)
        {
            if (!mVariables.ContainsKey(variableName))
            {
                mVariables.Add(variableName, variableValue);
                return false;
            }

            else
            {
                mVariables[variableName] = variableValue;
                return true;
            }


        }

        /// <summary>
        /// Helper funtion. This function format the string into desired form. It adds spaces between operators  
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private List<string> SplitString(string expression)
        {
            char[] array = expression.ToCharArray();
            string newExpresssion = string.Empty;

            foreach (char c in array)
            {
                if (ExpressionTreeFactory.IsValidOperator(c.ToString()) == true || c == '(' || c == ')')
                {
                    newExpresssion += " ";
                    newExpresssion += c;
                    newExpresssion += " ";
                }

                else
                {
                    newExpresssion += c;

                }
            }

            string[] Infix = newExpresssion.Split(new char[] { ' ' });
            List<string> list = new List<string>();

            foreach (string s in Infix)
            {
                if (s != "" && s != " ")
                {
                    list.Add(s);
                }
            }

            return list;
        }

        /// <summary>
        /// This function defines precedence of operators
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private int Precedence(string op)
        {
            if (op == "*" || op == "/")
            {
                return 2;
            }

            else if (op == "+" || op == "-")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// This function converts an infix expression to a postfix using shunting yard algorithm
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private List<string> Shunting_Yard(string expression)
        {
            List<string> Infix = this.SplitString(expression);

            Stack<string> stack = new Stack<string>();
            List<string> postfix = new List<string>();

            try
            {
                double number;

                foreach (string s in Infix)
                {
                    // if incoming symbol is an operand, print it
                    if (double.TryParse(s, out number))
                    {
                        postfix.Add(s.ToString());

                    }
                    // if incoming symbol is a left parenthesis, push it to the stack
                    else if (s == "(")
                    {

                        stack.Push(s);
                    }

                    /// if incoming symbol is a right parenthesis, discard it. pop and print stack symbols
                    /// until left parenthesis is encountered.

                    else if (s == ")")
                    {
                        while (stack.Count != 0 && stack.Peek() != "(")
                        {
                            postfix.Add(stack.Pop());
                        }

                        // pop left parenthesis
                        stack.Pop();
                    }

                    else if (ExpressionTreeFactory.IsValidOperator(s) == true)
                    {
                        while (stack.Count != 0 && Precedence(stack.Peek()) >= Precedence(s.ToString()))
                        {
                            postfix.Add(stack.Pop());
                        }

                        stack.Push(s.ToString());
                    }

                    else
                    {
                        postfix.Add(s);
                    }
                }

                /// print and pop all operators from the stack
                while (stack.Count != 0)
                {
                    postfix.Add(stack.Pop());
                }

                foreach (string s in postfix)
                {
                    if (s == "(")
                    {
                        return null;
                    }
                }

                return postfix;
            }
            catch
            {
                Console.WriteLine("Invalid expression");
                return postfix;
            }

        }

        /// <summary>
        /// This function evaluates the tree and returns the result
        /// </summary>
        /// <returns></returns>
        public double Evaluate()
        {
            try
            {
                return this.mRoot.Evaluate();
            }
            catch
            {
                Console.WriteLine("Unable to evaluate the tree");
                return 0;
            }

        }

        /// <summary>
        /// This function displays the menu
        /// </summary>
        public void displayMenu()
        {
            Console.WriteLine("Menu(Current expression = \"" + this.expression + "\"");
            Console.WriteLine("   1.Enter a new expression");
            Console.WriteLine("   2.Set a variable value");
            Console.WriteLine("   3.Evaluate tree");
            Console.WriteLine("   4.Quit");
        }
    }
}
