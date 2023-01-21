using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Button
{

    public partial class ErrorForm : Form
    {
        public ErrorForm(string errorMessage)
        {
            InitializeComponent();
            label1.Text = errorMessage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ErrorForm_Load(object sender, EventArgs e)
        {

        }
    }

}

