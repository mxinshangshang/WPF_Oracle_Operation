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
            string constring = "data source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST="+ServerName.Text+ ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)));user id="+UserName.Text+ ";password="+Passwd.Text+";";
            OracleConnection conn = new OracleConnection(constring);
            try 
            {  
                conn.Open();
                DataSet ds = new DataSet();
                string sql = "select * from "+TableName.Text;
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
                MessageBox.Show(ex.ToString());
            }  
            finally 
            {  
                conn.Close();
            }
        }
    }
}
