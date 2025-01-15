using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace Library_App
{
    public partial class FrmLogin : Form
    {
        string strConnection = "Server=localhost ; port=5432 ; user id=postgres ; password=postgres ; database=testdb ;";

        NpgsqlConnection connectNpg = new NpgsqlConnection();
        NpgsqlDataReader reader;

        public FrmLogin()
        {
            InitializeComponent();
        }
        private void closeConnect()
        {
            if (connectNpg.State == ConnectionState.Open) connectNpg.Close();
        }
        private void openConnect()
        {
            if (connectNpg.State == ConnectionState.Closed) connectNpg.Open();
        }
        private void closeButton_Click(object sender, EventArgs e)
        {
            //sql.connectionLeave();
            this.Close();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            connectNpg.ConnectionString = strConnection;
            
            loginField.Text = string.Empty;
            passField.Text = string.Empty;

            loginField.Text = "ADMIN";  //For test
            passField.Text = "123";   //For test
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            openConnect();
            if (loginField.Text.Trim() != string.Empty || passField.Text.Trim() != string.Empty)
            {
                NpgsqlCommand comm = new NpgsqlCommand("SELECT user_id,login,pass FROM users", connectNpg);
                reader = comm.ExecuteReader();
                
                while (reader.Read())
                {
                    if (loginField.Text == reader["login"].ToString() && passField.Text==reader["pass"].ToString())
                    {
                        //MessageBox.Show("Correct");

                        if (reader["admin"].ToString()=="True")
                        {
                            //MessageBox.Show("Admin Enter");
                            this.Hide();
                            FrmShowData newWindow = new FrmShowData();
                            newWindow.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            //MessageBox.Show("User Enter");
                            this.Hide();
                            FrmTable newWindow = new FrmTable();
                            newWindow.ShowDialog();
                            this.Close();
                        }
                    }
                }
                if (comm != null) comm.Dispose();
            }

            closeConnect();
        }

    }
}
