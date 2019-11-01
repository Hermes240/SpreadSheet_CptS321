/// <summary>
/// Programmer: Hermes Obiang
/// ID: 011589508
/// Class: CptS 321
/// Programming Assignment: HW4
/// Date: February 25, 2019
/// Description: 
/// </summary>
namespace HW4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Form Class
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Private data members
        /// </summary>
        private int numberOfRows = 50;
        private int numberOfColumns = 26;
        private SpreadsheetEngine.Spreadsheet spreadsheet;
        Random random = new Random();

        /// <summary>
        /// Runs the program
        /// </summary>
        public Form1()
        {
            
            InitializeComponent();
            
            this.spreadsheet = new SpreadsheetEngine.Spreadsheet(numberOfRows, numberOfColumns);
            this.EventHandlers();
           
            this.DrawGrid();
        }

        /// <summary>
        /// Draws the grid into the winform app
        /// </summary>
        private void DrawGrid()
        {
            char header = 'A';

            while(header <= 'Z')
            {
                dataGridView1.Columns.Add("", header.ToString());
                header++;
            }


            dataGridView1.Rows.Add(50);

            for(int i = 1; i<=50; i++)
            {
                dataGridView1.Rows[i-1].HeaderCell.Value = i.ToString();
            }
        }

        /// <summary>
        /// Show demo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void ShowDemo()
        {
            ///set the text in about 50 random cells to "Hello World!"
            for (int i = 0; i < 50; i++)
            {
                SpreadsheetEngine.Cell cell =
                    this.spreadsheet.GetCell(random.Next(numberOfRows), random.Next(numberOfColumns));
                cell.Text = "Hello Wolrd!";
            }

            ///set the text in every cell in column B to “This is cell B#”, where #
            ///number is the row number for the cell.
            for (int i = 0; i < numberOfRows; ++i)
            {
                SpreadsheetEngine.Cell cell = this.spreadsheet.GetCell(i, 1);    
                cell.Text = "This is cell B"+(i + 1).ToString();        
            }

            ///set the text in every cell in column A to “= B#”, where ‘#’ is the
            ///row number of the cell. So in other words you’re setting every cell
            ///in column A to have a value equal to the cell to the right of it in column B.
            for (int i = 0; i < numberOfRows; ++i)
            {
                SpreadsheetEngine.Cell cell = this.spreadsheet.GetCell(i, 0);    
                cell.Text = "=B" + (i + 1).ToString(); 
            }

        }

        /// <summary>
        /// Updates the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SpreadsheetEngine.Cell cell = sender as SpreadsheetEngine.Cell;

            if(e.PropertyName == "Value")
            {
                dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
                dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = this.ConvertUintToColor(cell.BGColor);

            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView gridView = sender as DataGridView;

            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            SpreadsheetEngine.Cell cell = spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            try
            {
                if (gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    cell.Text = gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
            }

            catch
            {

            }

        }

        private void EventHandlers()
        {
            this.spreadsheet.PropertyChanged += new PropertyChangedEventHandler(OnCellPropertyChanged);
            dataGridView1.CellBeginEdit += new DataGridViewCellCancelEventHandler(dataGridView1_CellBeginEdit);
            dataGridView1.CellEndEdit += new DataGridViewCellEventHandler(dataGridView1_CellEndEdit);
        }

        private void cellToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.Title = "Save XML File";
            fileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";

            if(fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                using (FileStream file = new FileStream(fileDialog.FileName, FileMode.Create))
                {
                    this.spreadsheet.Save_File(file);
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Title = "Open XML File";
            openFile.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";

            if(openFile.ShowDialog() != DialogResult.Cancel)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                this.spreadsheet = new SpreadsheetEngine.Spreadsheet(numberOfRows, numberOfColumns);
                this.EventHandlers();

                this.DrawGrid();

                using (FileStream file = new FileStream(openFile.FileName, FileMode.Open)) 
                {
                    this.spreadsheet.Load_file(file);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.AllowFullOpen = true;

            

            if(colorDialog.ShowDialog() == DialogResult.OK)
            {
                uint CellColor = this.ConvertColorToUInt(colorDialog.Color);

                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    cell.Style.BackColor = colorDialog.Color;
                    this.spreadsheet.GetCell(cell.RowIndex, cell.ColumnIndex).BGColor = CellColor;
                }
                    

            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private  Color ConvertUintToColor(uint color )
        {
            return Color.FromArgb((byte)((color >> 24) & 0xFF),
                   (byte)((color >> 16) & 0xFF),
                   (byte)((color >> 8) & 0xFF),
                   (byte)(color & 0xFF));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private uint ConvertColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) |
                          (color.G << 8) | (color.B << 0));
        }

    }
}
