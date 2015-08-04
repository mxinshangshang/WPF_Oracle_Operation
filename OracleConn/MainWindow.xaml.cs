using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.ObjectModel;
using System.Reflection;

namespace OracleConn
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string sql = "select * from " + TableName.Text;
                OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                oda.Fill(ds);
                conn.Close();

                int i = ds.Tables[0].Rows.Count;
                int j = ds.Tables[0].Columns.Count;

                for (int k = 0; k < i; k++)
                {
                    for (int m = 0; m < j; m++)
                    {
                        if (int.Parse(ds.Tables[0].Rows[k][m].ToString()) < 0)
                            ds.Tables[0].Rows[k][m] = "0";

                    }
                }
                dataGrid.DataContext = ds.Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void DataGrid_TextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);
            try
            {
                int index = dataGrid.SelectedIndex;
                /*
                string del = "delete from User_Table where [User_id] = '" + dataGrid.Rows[index].Cells[0].Value.ToString() + "'";
                conn.Open();
                string sql = "select * from " + TableName.Text;
                OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                 */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedCells.Count > 0)
            {
                DataGridCell cell = DataGridHelper.GetCell(dataGrid.SelectedCells[0]);
                this.Title = DataGridHelper.GetRowIndex(cell).ToString();
            }
        }
    }

    public static class DataGridHelper
    {
        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }

            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }
        public static int GetRowIndex(DataGridCell dataGridCell)
        {
            // Use reflection to get DataGridCell.RowDataItem property value.
            PropertyInfo rowDataItemProperty = dataGridCell.GetType().GetProperty("RowDataItem", BindingFlags.Instance | BindingFlags.NonPublic);

            DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

            return dataGrid.Items.IndexOf(rowDataItemProperty.GetValue(dataGridCell, null));
        }
        public static DataGrid GetDataGridFromChild(DependencyObject dataGridPart)
        {
            if (VisualTreeHelper.GetParent(dataGridPart) == null)
            {
                throw new NullReferenceException("Control is null.");
            }
            if (VisualTreeHelper.GetParent(dataGridPart) is DataGrid)
            {
                return (DataGrid)VisualTreeHelper.GetParent(dataGridPart);
            }
            else
            {
                return GetDataGridFromChild(VisualTreeHelper.GetParent(dataGridPart));
            }
        }
    }
}
