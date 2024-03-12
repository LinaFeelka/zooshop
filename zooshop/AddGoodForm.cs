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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace zooshop
{
    public partial class AddGoodForm : Form
    {
        Database database = new Database();
        private readonly checkUser _user;
        public AddGoodForm(checkUser user)
        {
            InitializeComponent();
            _user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AddGoodForm_Load(object sender, EventArgs e)
        {
            labelUser.Text = $"{_user.Login}:{_user.Status}";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var good_name = textBox1.Text;
            var description = textBox2.Text;
            var price = textBox3.Text;
            var discount = textBox4.Text;
            var manufacturer = textBox5.Text;
            var photo = textBox6.Text;

            string query = $"insert into goods (good_name, description, price, discount, manufacturer, photo) values ('{good_name}','{description}','{price}','{discount}','{manufacturer}','{photo}')";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, database.getConnection());
            npgsqlCommand.ExecuteNonQuery();

            MessageBox.Show("Данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            database.closeConnection();

            GoodForm goods = new GoodForm(_user);
            goods.ShowDialog();
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            GoodForm goods = new GoodForm(_user);
            goods.ShowDialog();
            this.Close();
        }
    }
}
