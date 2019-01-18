using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace ETL
{
    public partial class Form1 : Form
    {
        bool 원본 = false, 대상 = false;
        MySqlConnection conn1=null, conn2=null;
        ArrayList 운영테이블;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Click += Button_Click;
            button2.Click += Button_Click;
            button3.Click += Button_Click;
            listView1.FullRowSelect = true;
            운영테이블 = new ArrayList();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            
            switch (btn.Name)
            {
                case "button1":
                    원본 = 개발();
                    if (원본)
                    {
                        btn.BackColor = Color.DarkOrange;
                    }
                    break;
                case "button2":
                    대상 = 운영();
                    if (대상)
                    {
                        btn.BackColor = Color.DarkOrange;
                    }
                    break;
                case "button3":
                    if(원본 && 대상)
                    이행();
                    break;
                default:
                    break;
            }
        }

        private bool 개발()
        {
            try
            {
                MessageBox.Show("개발()");
                string strConnection = string.Format("server={0};user={1};password={2};database={3}","192.168.3.142","root","1234","test");
                conn1 = GetConnection(strConnection);
                if(conn1 == null)
                {
                    MessageBox.Show("DB 연결 오류!");
                    return false;
                }
                conn1.Open();
                MessageBox.Show("DB 연결 성공!");
                string sql = "SHOW TABLES";
                MySqlDataReader sdr = GetReader(sql, conn1);
                listView1.Items.Clear();
                
                for(int i =1; sdr.Read();i++)
                {
                    listView1.Items.Add( new ListViewItem(new string[] {i.ToString(),sdr["Tables_in_test"].ToString()}));
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool 운영()
        {
            try
            {
                MessageBox.Show("운영()");
                string strConnection = string.Format("server={0};user={1};password={2};database={3}", "192.168.3.155", "root", "1234", "test");
                MySqlConnection conn2 = GetConnection(strConnection);
                if (conn2 == null)
                {
                    MessageBox.Show("DB 연결 오류!");
                    return false;
                }
                conn2.Open();
                MessageBox.Show("DB 연결 성공!");
                string sql = "SHOW TABLES";
                MySqlDataReader sdr = GetReader(sql, conn2);
                while (sdr.Read())
                {
                    운영테이블.Add(sdr["Tables_in_test"].ToString());
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool 이행()
        {
            try
            {
                MessageBox.Show("이행");
                SelectedListViewItemCollection col=listView1.SelectedItems;
                for(int i=0;i< col.Count; i++)
                {
                    ListViewItem item = col[i];
                    MessageBox.Show(item.SubItems[1].Text);
                    ArrayList list = Table_Select(item.SubItems[1].Text);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private ArrayList Table_Select(string name)
        {
            try
            {
                ArrayList list = new ArrayList();
                string sql = string.Format("select * from {0};", name);
                MySqlDataReader sdr = GetReader(sql, conn1);
                string result = "[";
                while (sdr.Read())
                {
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        result += sdr[i].ToString();
                    }
                    result += "]\n";
                }
                MessageBox.Show(result);
                return list;
            }
            catch
            {
                return null;
            }
        }

        private MySqlConnection GetConnection(string strConnection)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = strConnection;
                return conn;
            }
            catch
            {
                return null;
            }
        }

        private void CloseConnection(MySqlConnection conn)
        {
            try
            {
                conn.Close();
            }
            catch
            {
                MessageBox.Show("연결해제 실패...");
            }
        }

        private MySqlDataReader GetReader(string sql,MySqlConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    MySqlCommand comm = new MySqlCommand();
                    comm.CommandText = sql;
                    comm.Connection = conn;
                    return comm.ExecuteReader();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private bool NonQuery(string sql,MySqlConnection conn)
        {
            try
            {
                if (conn.State==ConnectionState.Open)
                {
                    MySqlCommand comm = new MySqlCommand(sql, conn);
                    comm.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void ReaderClose(MySqlDataReader reader)
        {
            reader.Close();
        }
    }
}
