using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Data.SqlClient;
using System.Threading;

namespace PingTester
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private void atualizaDataGrid()
        {
            string conexao = PingTester.Properties.Settings.Default.BDPing;
            SqlConnection sqlConnection = new SqlConnection(conexao);
            sqlConnection.Open();

            string comando = "SELECT ip ADRESS, status STATUS  FROM Ips";
            SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
            dataAdapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;

            sqlConnection.Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(executaPing);
            thread.Start();

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void executaPing()
        {
            int i = 0;

            while (buttonStart.Enabled == false)
            {
                //MessageBox.Show(dataGridView1.Rows[i].Cells[0].Value.ToString());
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(dataGridView1.Rows[i].Cells[0].Value.ToString());

                if (pingReply.Status == IPStatus.Success)
                {
                    dataGridView1.Rows[i].Cells[1].Value = pingReply.Status;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[1].Value = pingReply.Status;
                }

                i++;

                if (i == dataGridView1.Rows.Count)
                {
                    i = 0;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            atualizaDataGrid();
        }

        private void buttonAdicionar_Click(object sender, EventArgs e)
        {
            string conexao = PingTester.Properties.Settings.Default.BDPing;
            SqlConnection sqlConnection = new SqlConnection(conexao);
            sqlConnection.Open();

            string comando = "INSERT INTO Ips(ip) VALUES (@ip)";
            SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@ip", textBox1.Text);
            sqlCommand.ExecuteNonQuery();

            sqlConnection.Close();

            atualizaDataGrid();
            textBox1.Text = "";
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            for(int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = "OFFLINE";
            }
        }

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            string conexao = PingTester.Properties.Settings.Default.BDPing;
            SqlConnection sqlConnection = new SqlConnection(conexao);
            sqlConnection.Open();                                   

            string comando = "UPDATE Ips SET ip = @newip WHERE @cellSelected = @id";
            SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@ip", dataGridView1.CurrentRow.Cells[0].Value.ToString());
            sqlCommand.ExecuteNonQuery();

            sqlConnection.Close();

            atualizaDataGrid();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
    }
}
