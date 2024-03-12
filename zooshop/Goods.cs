using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using Npgsql.Internal;

namespace zooshop
{
    public partial class Goods : Form
    {
       Database database = new Database();
        public Goods()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void createColumns()
        {
            dataGridView1.Columns.Add("good_name", "Наименование товара"); //0
            dataGridView1.Columns.Add("description", "Описание товара"); //1
            dataGridView1.Columns.Add("price", "Цена"); //2
            dataGridView1.Columns.Add("discount", "Скидка"); //3
            dataGridView1.Columns.Add("manufacturer", "Производитель"); //4
            dataGridView1.Columns.Add("photo", "Фотография"); //5
        }

        public void refreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = $"select * from goods";
            NpgsqlCommand cmd = new NpgsqlCommand(query, database.getConnection());

            database.openConnection();
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                readSingreRow(dgw, reader);
            }
            reader.Close();
        }

        private void readSingreRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetString(0), record.GetString(1), record.GetInt32(2), record.GetInt32(3), record.GetString(4), record.GetString(5));
        }
        private void Goods_Load(object sender, EventArgs e)
        {
            createColumns();
            refreshDataGrid(dataGridView1);
            addOrder.Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string image = row.Cells[5].Value.ToString();

                if (!string.IsNullOrEmpty(image))
                {
                    pictureBox1.ImageLocation = image;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private int clickedRowIndex = -1;

        private void addOrder_Click(object sender, EventArgs e)
        {
            if(clickedRowIndex >= 0 && clickedRowIndex < dataGridView1.Rows.Count)
            {
                string good_name = dataGridView1.Rows[clickedRowIndex].Cells[0].Value.ToString();
                string description = dataGridView1.Rows[clickedRowIndex].Cells[1].Value.ToString();
                decimal price = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[2].Value);
                decimal discount = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[3].Value);
                string manufacturer =  dataGridView1.Rows[clickedRowIndex].Cells[4].Value.ToString();

                NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; Database = zooshop; User Id = postgres; Password = assaq123;");
                
                OrderForm order = new OrderForm(good_name, description, price, discount, manufacturer, npgsqlConnection);
                order.ShowDialog();
            } 
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                clickedRowIndex = e.RowIndex; //индекс строки

                addOrder.Visible = true;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }
    }
}
