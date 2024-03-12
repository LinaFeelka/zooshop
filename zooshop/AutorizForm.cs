using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zooshop
{
    public partial class AutorizForm : Form
    {
        Database database = new Database();
         
        public AutorizForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var login = textBoxLogin.Text;
            var password = textBoxPass.Text;

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            string query = $"select id, login, password, id_role from users where login = '{login}' and password = '{password}'";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.getConnection());

            adapter.SelectCommand = comm;
            adapter.Fill(table);

            var role = new NpgsqlCommand("select id_role from users where login = '{login}' and password = '{password}'");
            var exec = role.ExecuteScalar();            

            label4.Text = exec.ToString();


            if (table.Rows.Count == 1)
            {
                var user = new checkUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));

                MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
               /* WorkForm workForm = new WorkForm(user);
                this.Hide();
                workForm.Show();*/
            }
            else
            {
                MessageBox.Show("Такого аккаунта не существует!", "Аккаунта не существует!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            database.closeConnection();
        }
    }
}
