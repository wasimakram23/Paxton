using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaxtonPasswordGenerator
{
    public partial class Form1 : Form
    {
        EncryptionMD5 crypto = new EncryptionMD5();
        public Form1()
        {
            InitializeComponent();
        }

        private void GenerateCrypto_Click(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            string plainPassword = password.Text;
            string cryptoPassword = crypto.EncryptUsernamePassword(plainPassword);
            textBox2.Text = cryptoPassword;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
