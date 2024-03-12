using Npgsql;
using Npgsql.Internal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zooshop
{
    public partial class OrderForm : Form
    {
        Database database = new Database();
        private Random random = new Random();
        string good_name, description, manufacturer;
        decimal price, discount;
        public OrderForm(string good_name, string description, decimal price, decimal discount, string manufacturer, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            InitializeComponent();

            this.good_name = good_name;
            this.description = description; 
            this.price = price;
            this.discount = discount;
            this.manufacturer = manufacturer;
            conn = npgsqlConnection;

            richTextBox1.Text = $"Наименование товара: {this.good_name}\n" +
                                $"Описание товара: {this.description}\n" +
                                $"Производитель товара: {this.manufacturer}\n" +
                                $"Цена товара: {price:C}\n" +
                                $"Размер скидки: {discount}\n" +
                                $"Дата заказа: {DateTime.Now:dd/MM/yyyy} \n" +
                                $"Номер заказа: {generateOrderNum()}\n" +
                                $"Код получения: {generateOrderCode()}\n" +
                                $"Пункт выдачи: {comboBox1.SelectedIndex}";

            numericUpDown1.Value = 1;
            this.price = price;
            this.discount = discount;

            comboBox1.Items.Add("Пункт 1");
            comboBox1.Items.Add("Пункт 2");
            comboBox1.Items.Add("Пункт 3");
        }
        private NpgsqlConnection conn;

        private string generateOrderNum()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssff");
        }

        private string generateOrderCode() //генерация случайных чисел
        {
            return random.Next(100, 999).ToString();
        }

        private void updateRichTextBox()
        {
            decimal quantity = numericUpDown1.Value;

            if (quantity <= 0)
            {
                MessageBox.Show("Заказ не может быть пустым!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Goods goods = new Goods();
                goods.Show();
                this.Close();
            } else
            {
                decimal end_price = (this.price * quantity) - this.discount;

                richTextBox1.Text = $"Наименование товара: {this.good_name}\n" +
                                $"Описание товара: {this.description}\n" +
                                $"Производитель товара: {this.manufacturer}\n" +
                                $"Цена товара: {end_price:C}\n" +
                                $"Размер скидки: {this.discount}\n" +
                                $"Дата заказа: {DateTime.Now:dd/MM/yyyy} \n" +
                                $"Номер заказа: {generateOrderNum()}\n" +
                                $"Код получения: {generateOrderCode()}\n" +
                                $"Пункт выдачи: {comboBox1.SelectedIndex}";
                textBox1.Text = end_price.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            updateRichTextBox();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal quantity = numericUpDown1.Value;
            updateRichTextBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //значения из RichTextBox
            string good_name = this.good_name;
            decimal discount = this.discount;
            decimal end_price = decimal.Parse(textBox1.Text); //общая сумма
            string order_num = generateOrderNum();
            string location = comboBox1.SelectedIndex.ToString();
            string code = generateOrderCode();
            decimal quantity = numericUpDown1.Value;
            //сохраненние в бд
            using (NpgsqlCommand comm = new NpgsqlCommand())
            {
                conn.Open();
                comm.Connection = conn;
                //запрос
                string query = $"insert into orders (order_date,good_name, discount, end_price, order_num, location, code, status, quantity) " +
                    $"values (@order_date, @good_name, @discount, @end_price, @order_num, @location, @code, 'новый', @quantity)";
                
                comm.CommandText = query;

                //параметры запроса
                comm.Parameters.Add(new NpgsqlParameter("@order_date", NpgsqlDbType.Date)).Value = DateTime.Now.Date;
                comm.Parameters.AddWithValue("@order_num", long.Parse(order_num));
                comm.Parameters.AddWithValue("@good_name", good_name); // Предполагается, что название товара берется из поля name
                comm.Parameters.AddWithValue("@end_price", end_price);
                comm.Parameters.AddWithValue("@discount", discount);
                comm.Parameters.AddWithValue("@location", location);
                comm.Parameters.AddWithValue("@code", int.Parse(code));
                comm.Parameters.AddWithValue("@quantity", quantity);

                comm.ExecuteNonQuery();
                MessageBox.Show("Заказ успешно оформлен и сохранен в базе данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Goods goods = new Goods();
            goods.Show();
            Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font("Times New Roman", 16, FontStyle.Regular), Brushes.Black, new Point(10, 10));
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }
    }
}
