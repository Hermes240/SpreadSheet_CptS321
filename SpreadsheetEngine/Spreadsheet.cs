/// <summary>
/// Programmer: Hermes Obiang
/// ID: 011589508
/// Class: CptS 321
/// Programming Assignment: Spreadsheet Formula Evaluation
/// Date: April 27, 2019
/// Description: 
/// </summary>
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Linq;

    /// <summary> 
    /// Spreadsheet class
    /// </summary>
    public class Spreadsheet
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// private data members
        /// </summary>
        private spreadsheetCell[,] spreadsheet;
        private readonly int mRowLength;
        private readonly int mColumnLength;
        private Dictionary<Cell, HashSet<Cell>> mDependency;
        private ExpressionTree expressionTree = new ExpressionTree();
        private string expression = string.Empty;
        private Stack<List<Cell>> undos;
        private Stack<List<Cell>> redos;


        /// <summary>
        /// Initializes private data members
        /// </summary>
        public Spreadsheet()
        {
            this.mColumnLength = 1;
            this.mRowLength = 0;
            this.mDependency = new Dictionary<Cell, HashSet<Cell>>();
            this.expression = string.Empty;
        }
        /// <summary>
        /// Parameterized constructor initializes the 2D array that represents the spreadsheet
        /// </summary>
        /// <param name="numberOfRows"></param>
        /// <param name="numberOfColumns"></param>
        public Spreadsheet(int numberOfRows, int numberOfColumns)
        {
            this.mDependency = new Dictionary<Cell, HashSet<Cell>>();
            this.undos = new Stack<List<Cell>>();
            this.redos = new Stack<List<Cell>>();
            char header = 'A';
            this.spreadsheet = new spreadsheetCell[numberOfRows, numberOfColumns];

            for (int row = 0; row < numberOfRows; row++)
            {
                for (int column = 0; column < numberOfColumns; column++)
                {
                    this.spreadsheet[row, column] = new spreadsheetCell(row, column, header.ToString());
                    this.spreadsheet[row, column].PropertyChanged +=
                        new PropertyChangedEventHandler(CellPropertyChanged);

                    if (header == 'Z')
                    {
                        header = 'A';
                    }

                    else
                    {
                        header++;
                    }                  
                }
            }
        }

        /// <summary>
        /// Property that returns the number of rows
        /// </summary>
        public int RowCount { get => this.spreadsheet.GetLength(this.mRowLength); }

        /// <summary>
        /// Property that returns the number of columns
        /// </summary>
        public int ColumnCount { get => this.spreadsheet.GetLength(this.mColumnLength); }

        /// <summary>
        /// Updates datagrid when value has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell cell = sender as Cell;
            cell.Text = cell.Text.ToUpper();

            if (e.PropertyName == "Text")
            {
                // remove dependencies
                this.Eliminate_dependency(cell);

                if (cell.Text.StartsWith("=")) // consider this to be a formula
                {
                    //chop the equal sign
                    this.expression = cell.Text.Substring(1);

                    if(int.TryParse(this.expression, out int numb))
                    {
                        this.EvaluateExpression(cell);

                        if (this.mDependency.Count != 0)
                        {
                            this.Update_Spreadsheet(cell);
                        }
                    }

                    else
                    {
                        
                        if(!this.isSelfReference(cell))
                        {
                            this.EvaluateExpression(cell);
                            this.Add_dependencies(expression, cell);

                            if (this.mDependency.Count != 0)
                            {
                                this.Update_Spreadsheet(cell);
                            }
                        }

                        else
                        {
                            this.Add_dependencies(expression, cell);
                        }

                        
                    }                                                        
                }

                else if(double.TryParse(cell.Text,out double Int))
                {
                    this.expression = cell.Text;
                    this.EvaluateExpression(cell);
                    if (this.mDependency.Count != 0)
                    {
                        this.Update_Spreadsheet(cell);
                    }
                }

                else
                {
                    cell.Value = cell.Text;
                }                
            }
                        
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Value"));
        }

       
        /// <summary>
        /// This function takes a row and a column index and returns the cell at that location
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public Cell GetCell(int rowIndex, int columnIndex)
        {
            if (rowIndex >= this.spreadsheet.GetLowerBound(this.mRowLength) &&
                rowIndex <= this.spreadsheet.GetUpperBound(this.mRowLength) &&
                columnIndex >= this.spreadsheet.GetLowerBound(this.mColumnLength) &&
                columnIndex <= this.spreadsheet.GetUpperBound(this.mColumnLength))
            {
                return this.spreadsheet[rowIndex, columnIndex];
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes dependencies from the dictionary
        /// </summary>
        /// <param name="cell"></param>
        private void Eliminate_dependency(Cell cell)
        {
            if (this.mDependency.ContainsKey(cell))
                {
                    this.mDependency[cell].Clear();
                }

                this.mDependency.Remove(cell);
                     
       }

        /// <summary>
        /// This function evaluates expression
        /// </summary>
        /// <param name="cell"></param>
        private void EvaluateExpression(Cell cell)
        {
            List<string> temp2 = new List<string>();
            int Column = 0, Row = 0;

            string[] temp = this.expression.Split(new char[] { '+', '-', '*', '/' });

            foreach(string s in temp)
            {
                if(s != "")
                {
                    temp2.Add(s);
                }
            }

            //if user enter an empty formula such as "="
            if (temp2.Count == 0)
            {
                cell.Value = string.Empty;
            }

            // check for valid for valid formula
            else if(temp2.Count > 1)
            {
                //construct tree with given expression
                expressionTree = new ExpressionTree(this.expression);

                //get varible names from the tree
                foreach(string var in expressionTree.GetVariableNames())
                {                 
                    //extract each cell
                    Cell newCell = this.RetrieveCell(var);

                    if(newCell == null)
                    {
                        cell.Value = "!(bad reference)";
                        return;
                    }
                    
                    double val = 0;

                    double.TryParse(newCell.Value, out val);

                    //modify tree with values of each cell
                    expressionTree.SetVariables(var, val);                   

                }

                //evaluate tree and set the output to the current cell
                cell.Value = expressionTree.Evaluate().ToString();
               
            }

            else
            {
                if (! double.TryParse(this.expression, out double val))
                {

                    Cell newCell = this.GetCell((Row - 1), Column);

                    if (this.RetrieveCell(this.expression) != null
                        && this.RetrieveCell(this.expression).Value == null)
                    {
                        cell.Value = "0";
                    }
                    else
                    {
                        cell.Value = this.RetrieveCell(this.expression).Value;
                    }                                                                                                               
                }
                else
                {
                    cell.Value = this.expression;
                 
                }
            }
        }

        /// <summary>
        /// This function updates the spreadsheet 
        /// </summary>
        /// <param name="cell"></param>
        private void Update_Spreadsheet(Cell cell)
        {
            List<Cell>list = new List<Cell>(this.mDependency.Keys);
           
                
            //update each dependent cell
                foreach (Cell var in list)
                {
                    this.expression = var.Text.Substring(1);

                    this.EvaluateExpression(var);
                }          
        }

        /// <summary>
        /// Adds dependencies
        /// </summary>
        /// <param name="var"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        private Cell Add_dependencies(string var, Cell cell)
        {
            if (!double.TryParse(this.expression,out double Int))
            {

                
                List<string> list = this.ParseText(var);
                foreach(string s in list)
                {
                    //extract each cell
                    Cell newCell = this.RetrieveCell(s);

                    //add the cell to the dependency dictionary
                    if (!this.mDependency.ContainsKey(cell))
                    {
                        this.mDependency.Add(cell, new HashSet<Cell>());
                        this.mDependency[cell].Add(newCell);

                    }

                }                              
            }

            return null;                      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private Cell RetrieveCell(string exp)
        {
            // get column and index
            int Column = Convert.ToInt16(exp[0]) - 'A';
            int.TryParse(exp.Substring(1), out int Row);

            //extract each cell
            Cell newCell = this.GetCell((Row - 1), Column);

            return newCell;
        }

        /// <summary>
        /// This funtion takes care of circular references
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool isSelfReference( Cell cell)
        {
            //initialize a hashset that holds dependencies of a given cell
            HashSet<Cell> REFCells = new HashSet<Cell>(this.ParseCell(cell.Text));

            List<Cell> Cells = this.ParseCell(cell.Text);

            
            foreach (Cell c in Cells)
            {
                // check if the user entered a valid cell reference
                if (c == null)
                {
                    cell.Value = "!(bad reference)";
                    return true;
                }

                // check if cell contain a self reference
                else if(c == cell)
                {
                    cell.Value = "!(self reference)";
                    return true;
                }
            }
            
            //check circular references
            foreach (Cell c in REFCells)
            {
                if (this.mDependency.ContainsKey(c))
                {
                    foreach(HashSet<Cell> hash in this.mDependency.Values)
                    {
                        if(hash.Contains(cell))
                        {
                            
                            cell.Value = "!(circular reference)";
                            return true;
                        }
                    }                   
                }
            }

            
            return false;           
        }


        /// <summary>
        /// Returns a list of cells that depend on the given cell
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<Cell> ParseCell(string text)
        {
            string[] temp = text.Split(new char[] { '+', '-', '*', '/','(',')','=' });
            List<Cell> ListOfCells = new List<Cell>();

            foreach(string str in temp)
            {
                if(!double.TryParse(str, out double Int) && str != "")
                {
                    Cell newCell = RetrieveCell(str);
                    ListOfCells.Add(newCell);
                }

            }

            return ListOfCells;

        }
        /// <summary>
        /// Returns a list of strings (cell's reference)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<string> ParseText(string text)
        {
            string[] temp = text.Split(new char[] { '+', '-', '*', '/', '(', ')', '=' });
            List<string> ListOfCells = new List<string>();



            foreach (string str in temp)
            {
                if (!double.TryParse(str, out double Int) && str != "")
                {

                    ListOfCells.Add(str);
                }

            }

            return ListOfCells;

        }

        /// <summary>
        /// Save information on spreasheet into an xml file
        /// </summary>
        /// <param name="outfile"></param>
        public void Save_File(Stream outfile)
        {
            //create an xml
            XmlWriter xml = XmlWriter.Create(outfile);

            xml.WriteStartElement("spreadsheet");

            //search thru all cells and save valid data
            foreach (Cell c in this.spreadsheet)
            {
                if(c.Value != null)
                {
                    xml.WriteStartElement("Cell", c.GetName);
                    xml.WriteElementString("Color", c.BGColor.ToString());
                    xml.WriteElementString("Text", c.Text);
                    xml.WriteEndElement();
                   
                }
            }
            // close the file
            xml.Close();
        }

        /// <summary>
        /// Load an xml file into the spreadsheet
        /// </summary>
        /// <param name="file"></param>
        public void Load_file(Stream file)
        {

            string name = string.Empty, color = string.Empty, value = string.Empty;
           

            using (XmlReader reader = XmlReader.Create(file))
            {
                //continue reading until file is empty
                while(reader.Read())
                {
                    if(reader.IsStartElement())
                    {
                        //get cell's name
                       if(reader.Name.ToString() == "Cell")
                        {
                 
                            name = reader.NamespaceURI;
                        }

                       //get cell's color
                       else if(reader.Name.ToString() == "Color")
                        {
                            reader.Read();
                            color = reader.Value;
                        }

                       //get cell's value
                        else if (reader.Name.ToString() == "Text")
                        {
                            reader.Read();
                            value = reader.Value;
                        }

                    }

                    if(value != string.Empty)
                    {
                        Cell cell = RetrieveCell(name);
                        cell.Text = value;
                        uint.TryParse(color, out uint newColor);
                        cell.BGColor = newColor;
                                            
                        //reset strings to empty strings 
                        value = string.Empty;
                        name = string.Empty;
                        color = string.Empty;
                    }
                }
            }
        }

    }
}
