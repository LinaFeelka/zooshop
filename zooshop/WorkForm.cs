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
    enum RowState
    {
        Exited,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class WorkForm : Form
    {
        Database database = new Database();
        private readonly checkUser _user;
        public WorkForm(checkUser user)
        {
            InitializeComponent();
            _user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (clickedRowIndex >= 0 && clickedRowIndex < dataGridView1.Rows.Count)
            {
                string good_name = dataGridView1.Rows[clickedRowIndex].Cells[0].Value.ToString();
                string description = dataGridView1.Rows[clickedRowIndex].Cells[1].Value.ToString();
                decimal price = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[2].Value);
                decimal discount = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[3].Value);
                string manufacturer = dataGridView1.Rows[clickedRowIndex].Cells[4].Value.ToString();

                NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; Database = zooshop; User Id = postgres; Password = assaq123;");

                OrderForm order = new OrderForm(good_name, description, price, discount, manufacturer, npgsqlConnection);
                order.ShowDialog();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            GoodForm goodForm = new GoodForm(_user);
            goodForm.Show();
            this.Close();
        }
        private void IsAdmin()
        {
            buttonDelete.Enabled = _user.IsAdmin;
        }

        private void WorkForm_Load(object sender, EventArgs e)
        {
            buttonAdd.Visible = false;
            createColumns();
            refreshDataGrid(dataGridView1);

            labelUser.Text = $"{_user.Login}:{_user.Status}";
            IsAdmin();
        }

        private void refreshDataGrid(DataGridView dgw)
        {
            dataGridView1.Rows.Clear();

            database.openConnection();

            string query = $"select * from orders";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.getConnection());
            
            NpgsqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                readSingleRow(dgw, reader);
            }
            reader.Close();

            database.closeConnection();
        }

        private void createColumns()
        {
            dataGridView1.Columns.Add("id","ID"); //0
            dataGridView1.Columns.Add("order_date", "Дата заказа"); //1
            dataGridView1.Columns.Add("good_name", "Наименoвание"); //2
            dataGridView1.Columns.Add("discount", "Скидка"); //3
            dataGridView1.Columns.Add("end_price", "Конечная цена"); //4
            dataGridView1.Columns.Add("order_num", "Номер заказа"); //5
            dataGridView1.Columns.Add("location", "Пункт"); //6
            dataGridView1.Columns.Add("code", "Код получения"); //7
            dataGridView1.Columns.Add("status", "Статус");//8
            dataGridView1.Columns.Add("quantity", "Количество"); //9
            dataGridView1.Columns.Add("IsNew",string.Empty); //10
            this.dataGridView1.Columns["IsNew"].Visible = false;
        }
        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = textBox1.Text;
            var order_date = textBox2.Text;
            var good_name = textBox3.Text;
            var discount = textBox4.Text;
            var end_price = textBox5.Text;
            var order_num = textBox6.Text;
            var location = textBox9.Text;
            var code = textBox7.Text;
            var status = textBox10.Text;
            var quantity = textBox8.Text;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[selectedRowIndex].SetValues(id, order_date, good_name, discount, end_price, order_num, location, code, status, quantity );
                dataGridView1.Rows[selectedRowIndex].Cells[10].Value = RowState.Modified;
            }
        }
        private void readSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetDateTime(1), record.GetString(2), 
                record.GetInt32(3), record.GetInt32(4), record.GetInt64(5), record.GetString(6), 
                record.GetInt32(7), record.GetString(8), record.GetInt32(9));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            clearFields();
        }
        public void clearFields ()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }
        int clickedRowIndex = 0;
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                clickedRowIndex = e.RowIndex; //индекс строки

                buttonAdd.Visible = true;
            }
        }
        int selectedRow = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                textBox5.Text = row.Cells[4].Value.ToString();
                textBox6.Text = row.Cells[5].Value.ToString();
                textBox8.Text = row.Cells[9].Value.ToString();
                textBox9.Text = row.Cells[6].Value.ToString();
                textBox10.Text = row.Cells[8].Value.ToString();
                textBox7.Text = row.Cells[7].Value.ToString();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Edit();
            clearFields();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[10].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[10].Value = RowState.Deleted;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[10].Value;

                if (rowState == RowState.Exited)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(textBox1.Text);
                    
                    var deleteQuery = $"delete from orders where id = {id}";

                    var comm = new NpgsqlCommand(deleteQuery, database.getConnection());
                    comm.ExecuteNonQuery();

                }

                if (rowState == RowState.Modified)
                {
                    
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var order_date = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var good_name = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var discount = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var end_price = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var order_num = dataGridView1.Rows[index].Cells[5].Value.ToString();
                    var location = dataGridView1.Rows[index].Cells[6].Value.ToString();
                    var code = dataGridView1.Rows[index].Cells[7].Value.ToString();
                    var status = dataGridView1.Rows[index].Cells[8].Value.ToString();
                    var quantity = dataGridView1.Rows[index].Cells[9].Value.ToString();

                    var changeQuery = $"update orders set order_date = '{order_date}', good_name = '{good_name}', discount = '{discount}'," +
                        $"end_price = '{end_price}', order_num = '{order_num}', location = '{location}', code = '{code}'," +
                        $"status = '{status}', quantity = '{quantity}'  where id = '{id}'";

                    var comm = new NpgsqlCommand(changeQuery, database.getConnection());
                    comm.ExecuteNonQuery();
                }
            }
            database.closeConnection();
        }
    }
}
