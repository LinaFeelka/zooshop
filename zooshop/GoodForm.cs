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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace zooshop
{
    public partial class GoodForm : Form
    {
        private readonly checkUser _user;
        Database database = new Database();
        public GoodForm(checkUser user)
        {
            InitializeComponent();
            _user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void GoodForm_Load(object sender, EventArgs e)
        {
            labelUser.Text = $"{_user.Login}:{_user.Status}";
            IsAdmin();

            createColumns();
            refreshDataGrid(dataGridView1);
        }

        private void IsAdmin()
        {
            buttonDelete.Enabled = _user.IsAdmin;
            buttonAdd.Enabled = _user.IsAdmin;
            buttonEdit.Enabled = _user.IsAdmin;
            buttonSave.Enabled = _user.IsAdmin;
        }

        private void createColumns()
        {
            dataGridView1.Columns.Add("good_name", "Наименование товара"); //0
            dataGridView1.Columns.Add("description", "Описание товара"); //1
            dataGridView1.Columns.Add("price", "Цена"); //2
            dataGridView1.Columns.Add("discount", "Скидка"); //3
            dataGridView1.Columns.Add("manufacturer", "Производитель"); //4
            dataGridView1.Columns.Add("photo", "Фотография"); //5
            dataGridView1.Columns.Add("IsNew", string.Empty); //6
            this.dataGridView1.Columns["IsNew"].Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                    
                textBox1.Text = row.Cells[0].Value.ToString(); //название       good_name
                textBox2.Text = row.Cells[1].Value.ToString(); //описание       description
                textBox3.Text = row.Cells[2].Value.ToString(); //цена           price
                textBox4.Text = row.Cells[3].Value.ToString(); //скидка         discount
                textBox5.Text = row.Cells[4].Value.ToString(); // производитель manufacturer
                textBox6.Text = row.Cells[5].Value.ToString(); //фото           photo
            }
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

        private void button10_Click(object sender, EventArgs e)
        {
            WorkForm work = new WorkForm(_user);
            work.Show();
            this.Close();
        }
        int selectedRow = 0;

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Edit();
            clearFields();
        }

        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var good_name = textBox1.Text;
            var description = textBox2.Text;
            var price = textBox3.Text;
            var discount = textBox4.Text;
            var manufacturer = textBox5.Text;
            var photo = textBox6.Text;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[selectedRowIndex].SetValues(good_name, description, price, discount, manufacturer, photo);
                dataGridView1.Rows[selectedRowIndex].Cells[6].Value = RowState.Modified;
            }
        }

        private void Delete()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
        }

        private void Update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[6].Value;

                if (rowState == RowState.Exited)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var name = textBox1.Text;

                    var deleteQuery = $"delete from goods where good_name = {name}";

                    var comm = new NpgsqlCommand(deleteQuery, database.getConnection());
                    comm.ExecuteNonQuery();

                }

                if (rowState == RowState.Modified)
                {

                    var good_name = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var description = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var price = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var discount = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var manufacturer = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var photo = dataGridView1.Rows[index].Cells[5].Value.ToString();

                    var changeQuery = $"update goods set good_name = '{good_name}', description = '{description}', price = '{price}'," +
                        $"discount = '{discount}', manufacturer = '{manufacturer}', photo = '{photo}' where good_name = '{good_name}'";

                    var comm = new NpgsqlCommand(changeQuery, database.getConnection());
                    comm.ExecuteNonQuery();
                }
            }
            database.closeConnection();
        }
        public void clearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddGoodForm addGoodForm = new AddGoodForm(_user);
            addGoodForm.Show();
            this.Close();
        }
    }
}
