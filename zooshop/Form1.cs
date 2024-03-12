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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Goods goods = new Goods();
            goods.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AutorizForm autorizForm = new AutorizForm();
            autorizForm.Show();
            Hide();
        }
    }
}
