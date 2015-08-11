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
using System.Data.SqlClient;

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
            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ats)));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                dt = conn.GetSchema("Tables");

                //foreach (Table column in dt.)
                //{
                //    TableComboBox.Items.Add(column.ToString());
                //}

                //foreach (DataRow dr in dt.Rows)
                //{
                //    ListViewItem li = new ListViewItem();
                //    li.SubItems.Clear();

                //    li.SubItems[0].Text = dr[0].ToString();

                //    for (int i = 1; i < dt.Columns.Count; i++)
                //    {

                //        li.SubItems.Add(dr[i].ToString());
                //    }

                //    listView1.Items.Add(li);
                //}  

                string sql = "select * from " + TableName.Text;
                OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                oda.Fill(ds);
                conn.Close();

                int i = ds.Tables[0].Rows.Count;
                int j = ds.Tables[0].Columns.Count;

                //for (int k = 0; k < i; k++)
                //{
                //    for (int m = 0; m < j; m++)
                //    {
                //        if (int.Parse(ds.Tables[0].Rows[k][m].ToString()) < 0)
                //            ds.Tables[0].Rows[k][m] = "0";

                //    }
                //}
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

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string row = null;
            string column = null;
            string content = null;
            DataGrid dg = sender as DataGrid;
            var cell = dg.CurrentCell;
            DataRowView item = cell.Item as DataRowView;
            DataGridTextColumn dgcol = dataGrid.Columns[cell.Column.DisplayIndex] as DataGridTextColumn;//表示一个 DataGrid 列，该列在其单元格中承载文本内容。
            Binding binding = dgcol.Binding as Binding;
            row = binding.Path.Path;

            if (dataGrid.SelectedCells.Count > 0)
            {
                DataGridCell cel = DataGridHelper.GetCell(dataGrid.SelectedCells[0]);
                DataGridRow firstItem = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(0);
                column = (dataGrid.Columns[0].GetCellContent(dataGrid.Items[DataGridHelper.GetRowIndex(cel)]) as TextBlock).Text.ToString();
                content = (e.EditingElement as TextBox).Text;
            }
            this.Title = "（" + row + "," + column + "）  Changed" + "   " + item[row].ToString() + "--->" + content + "  First:"+ (dataGrid.Columns[0].GetCellContent(dataGrid.Items[0]) as TextBlock).Text.ToString();

            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ats)));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string sql = "update " + TableName.Text + " set " + row + "= " + content + " where " + (dataGrid.Columns[0].GetCellContent(dataGrid.Items[0]) as TextBlock).Text.ToString()+"=" + column;
                OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                oda.Fill(ds);
                conn.Close();
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

        public List<string> GetTables(string connection)
        {
            List<string> tablelist = new List<string>();
            OracleConnection objConnetion = new OracleConnection(connection);
            try
            {
                if (objConnetion.State == ConnectionState.Closed)
                {
                    objConnetion.Open();
                    DataTable objTable = objConnetion.GetSchema("Tables");
                    foreach (DataRow row in objTable.Rows)
                    {
                        TableComboBox.Items.Add(row[0].ToString());
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (objConnetion != null && objConnetion.State == ConnectionState.Closed)
                {
                    objConnetion.Dispose();
                }

            }
            return tablelist;
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
