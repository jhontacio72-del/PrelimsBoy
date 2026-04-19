using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrelimsBoy
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

      

        private void btn_logout_Click(object sender, EventArgs e)
        {
            frm_Home go=new frm_Home();
            go.Show();
            this.Hide();
        }

        private void btn_logout_MouseEnter(object sender, EventArgs e)
        {
            btn_logout.BackColor = Color.Gray;
        }

        private void btn_logout_MouseLeave(object sender, EventArgs e)
        {
            btn_logout.BackColor = Color.SlateGray;
        }
    }
}
