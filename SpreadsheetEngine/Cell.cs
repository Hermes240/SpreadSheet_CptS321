/// <summary>
/// Programmer: Hermes Obiang
/// ID: 011589508
/// Class: CptS 321
/// Programming Assignment: Spreadsheet Formula Evaluation
/// Date: April 12, 2019
/// Description: 
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// The Cell class is an abstract base class that is declared publicly
    /// so the outside world of the class can see it
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int mRowIndex;
        private readonly int mColumnIndex;
        protected string mText;
        protected string mValue;
        protected uint Color = 0xFFFFFFFF;
        private string CellName;


        /// <summary>
        /// Default constructor that initializes data members
        /// </summary>
        public Cell(int row_index, int col_index, string data)
        {
            this.mRowIndex = row_index;
            this.mColumnIndex = col_index;
            this.mText = data;
            this.CellName = data;
            this.CellName += (this.mRowIndex +1).ToString();
        }

        public string GetName { get { return this.CellName; } }

        /// <summary>
        /// This function is a column index property that is readonly
        /// </summary>
        public int ColumnIndex { get => mColumnIndex; }

        /// <summary>
        /// This fucntion is a row index property that is readonly
        /// </summary>
        public int RowIndex { get => mRowIndex; }


        /// <summary>
        /// Implementation of the INotifyPropertyChanged interface
        /// </summary>
        /// <param name="property"></param>
        public void onPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// This fucntion is a string Data property that represents the actual data that's 
        /// typed into the cell
        /// </summary>
        public string Text
        {
            get => this.mText;

            set
            {
                if (this.mText != value)
                {
                    this.mText = value;
                    onPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// This function is a string property similar to the Text property
        /// </summary>
        public string Value
        {
            get => this.mValue;


            internal set
            {
                if (this.mValue != value)
                {
                    this.mValue = value;

                    onPropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// This function is a uint property
        /// </summary>
        public uint BGColor
        {
            get { return this.Color; }

            set
            {
                if(this.Color == value)
                {
                    return;
                }

                else
                {
                    this.Color = value;

                    this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }       
        
    }

    /// <summary>
    /// Class allow me to initialize cell class which is abstract
    /// </summary>
    public class spreadsheetCell : Cell
    {
        public spreadsheetCell(int row, int column, string header) : base(row, column, header) { }
    }
}
