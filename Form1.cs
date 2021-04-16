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

            string comando = "SELECT Id, name Name, ip ADRESS, status STATUS  FROM ipList";
            SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
            dataAdapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            dataGridView1.Columns[0].Visible = false;

            sqlConnection.Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(executaPing);
            thread.Start();

            buttonStart.Visible = false;
            buttonStop.Visible = true;
        }

        private void executaPing()
        {
            int i = 0;

            while (buttonStart.Visible == false)
            {
                //MessageBox.Show(dataGridView1.Rows[i].Cells[0].Value.ToString());
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(dataGridView1.Rows[i].Cells[2].Value.ToString());

                if (pingReply.Status == IPStatus.Success)
                {
                    //dataGridView1.Rows[i].Cells[1].Value = pingReply.Status;
                    dataGridView1.Rows[i].Cells[3].Value = "ONLINE";
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(50, 205, 50);
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
                }
                else if(pingReply.Status == IPStatus.TimedOut)
                {
                    dataGridView1.Rows[i].Cells[3].Value = pingReply.Status;
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(253, 191, 0);
                }
                else if (pingReply.Status == IPStatus.DestinationHostUnreachable)
                {
                    dataGridView1.Rows[i].Cells[3].Value = pingReply.Status;
                    dataGridView1.Rows[i].Cells[3].Value = "Inacessível";
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 211, 211);
                }
                else
                {
                    dataGridView1.Rows[i].Cells[3].Value = pingReply.Status;
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 105, 180);

                }

                i++;

                if (i == dataGridView1.Rows.Count)
                {
                    i = 0;
                }
            }
            if(buttonStart.Visible == false)
            {
                for (int j = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[j].Cells[3].Value = "OFFLINE";
                    dataGridView1.Rows[j].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
                    dataGridView1.Rows[j].DefaultCellStyle.BackColor = Color.FromArgb(255, 0, 0);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            atualizaDataGrid();
            textBoxIPAdress.Focus();
            textBoxNameIP.Focus();
        }

        private void buttonAdicionar_Click(object sender, EventArgs e)
        {
            if(textBoxNameIP.Text == "" && textBoxIPAdress.Text == "")
            {
                MessageBox.Show("Dados Incompletos!!", "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {                
                string conexao = PingTester.Properties.Settings.Default.BDPing;
                SqlConnection sqlConnection = new SqlConnection(conexao);
                sqlConnection.Open();

                string comando = "SELECT ip FROM ipList WHERE ip = @ip";
                SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@ip", textBoxIPAdress.Text);

                if(sqlCommand.ExecuteScalar() == null)
                {
                    string comando2 = "INSERT INTO ipList(name, ip) VALUES (@name, @ip)";
                    SqlCommand sqlCommand2 = new SqlCommand(comando2, sqlConnection);
                    sqlCommand2.Parameters.AddWithValue("@name", textBoxNameIP.Text);
                    sqlCommand2.Parameters.AddWithValue("@ip", textBoxIPAdress.Text);
                    sqlCommand2.ExecuteNonQuery();

                    sqlConnection.Close();

                    atualizaDataGrid();

                    labelid.Text = "";
                    textBoxNameIP.Text = "";
                    textBoxIPAdress.Text = "";
                }
                else
                {
                    MessageBox.Show("Ja existe um IP igual a este", "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sqlConnection.Close();
                }                
            }            
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStart.Visible = true;
            buttonStop.Visible = false;

            for(int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[3].Value = "OFFLINE";
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 0, 0);
            }
        }

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            string conexao = PingTester.Properties.Settings.Default.BDPing;
            SqlConnection sqlConnection = new SqlConnection(conexao);
            sqlConnection.Open();                                   

            string comando = "UPDATE ipList SET name = @name, ip = @ip WHERE Id = @id";
            SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", labelid.Text);
            sqlCommand.Parameters.AddWithValue("@name", textBoxNameIP.Text);
            sqlCommand.Parameters.AddWithValue("@ip", textBoxIPAdress.Text);

            sqlCommand.ExecuteNonQuery();

            sqlConnection.Close();

            atualizaDataGrid();

            labelid.Text = "";
            textBoxNameIP.Text = "";
            textBoxIPAdress.Text = "";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           labelid.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
           textBoxNameIP.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
           textBoxIPAdress.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string conexao = PingTester.Properties.Settings.Default.BDPing;
            SqlConnection sqlConnection = new SqlConnection(conexao);
            sqlConnection.Open();

            string comando = "DELETE FROM ipList WHERE Id = @id";
            SqlCommand sqlCommand = new SqlCommand(comando, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", labelid.Text);

            sqlCommand.ExecuteNonQuery();

            sqlConnection.Close();

            atualizaDataGrid();

            labelid.Text = "";
            textBoxNameIP.Text = "";
            textBoxIPAdress.Text = "";
        }
    }
}
