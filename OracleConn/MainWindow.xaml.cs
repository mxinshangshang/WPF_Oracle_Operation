/**************************************************************************************
*  File name:      MainWindow.xaml.cs
*  Author:         Mxin Chiang
*  Version:        1.0
*  Date:           13.08.2015
*  Description:    Design a tool to show tables from Database of TRAINRUNINFO 
*  Others:
*  Function List:
***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private void Button_Click(object sender, RoutedEventArgs e)                            //Load DB
        {
            this.TableComboBox.Items.Clear();
            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=" + dbName.Text + ")));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);

            try
            {
                OracleCommand cmd = new OracleCommand("select table_name from user_tables", conn);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    IDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        TableComboBox.Items.Add(dr["table_name"].ToString());
                    }
                    dr.Close();
                }
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

        private void table_SelectionChanged(object sender, SelectionChangedEventArgs e)          //Choose Table
        {
            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=" + dbName.Text + ")));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string sql = "select * from " + TableComboBox.SelectedItem.ToString();
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

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)    //Change Value
        {
            string row = null;
            string column = null;
            string content = null;
            DataGrid dg = sender as DataGrid;
            var cell = dg.CurrentCell;
            DataRowView item = cell.Item as DataRowView;
            DataGridTextColumn dgcol = dataGrid.Columns[cell.Column.DisplayIndex] as DataGridTextColumn;
            Binding binding = dgcol.Binding as Binding;
            row = binding.Path.Path;

            if (dataGrid.SelectedCells.Count > 0)
            {
                DataGridCell cel = DataGridHelper.GetCell(dataGrid.SelectedCells[0]);
                DataGridRow firstItem = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(0);
                column = (dataGrid.Columns[0].GetCellContent(dataGrid.Items[DataGridHelper.GetRowIndex(cel)]) as TextBlock).Text.ToString();
                content = (e.EditingElement as TextBox).Text;
            }
            this.Title = "（" + row + "," + column + "）  Changed" + "   " + item[row].ToString() + "--->" + content;

            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerName.Text + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=" + dbName.Text + ")));user id=" + UserName.Text + ";password=" + Passwd.Text + ";";
            OracleConnection conn = new OracleConnection(constring);
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string sql = "update " + TableComboBox.SelectedItem.ToString() + " set " + row + "= " + content + " where pn=" + column;//待修改 pn
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
    }

    public static class DataGridHelper           //Get row
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