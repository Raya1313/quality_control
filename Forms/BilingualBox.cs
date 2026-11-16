using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    public partial class BilingualBox : UserControl
    {
        public BilingualBox() { InitializeComponent(); }

        public string Fa { get => txtFa.Text; set => txtFa.Text = value; }
        public string En { get => txtEn.Text; set => txtEn.Text = value; }

        private void BilingualBox_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutpnlBanner_Paint(object sender, PaintEventArgs e)
        {

        }

        private void IbIFa_Click(object sender, EventArgs e)
        {

        }

        private void IbIEn_Click(object sender, EventArgs e)
        {

        }
    }
}
