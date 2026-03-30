using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using PrelimsBoy.Services;
namespace PrelimsBoy
{
    public partial class frm_Home : Form
    {
        string connString = "server=localhost;database=User_db;uid=root;pwd=;";
        private frm_superadmin _gridForm;
        public frm_Home()
        {
            InitializeComponent();

        }


       
        private void StyleTextbox(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(60, 60, 60);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.None;


        }

        private bool passwordVisible = false;
        

        private void Form1_Load(object sender, EventArgs e)
        {

            cmb_role.BackColor = Color.FromArgb(60, 60, 60);
            StyleTextbox(tb_username);
            StyleTextbox(tb_password);
            StyleTextbox(tb_createusername);
            StyleTextbox(tb_createpassword);
            StyleTextbox(tb_confirmpassword);
            StyleTextbox(tb_fullname);
            btn_login.FlatStyle = FlatStyle.Flat;
            btn_login.FlatAppearance.BorderSize = 0;
            btn_login.BackColor = Color.FromArgb(200, 30, 30);
            btn_login.ForeColor = Color.White;
            btn_login.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            tb_username.Text = "Enter Username";
            tb_username.ForeColor = Color.Gray;

            tb_password.Text = "Enter Password";
            tb_password.ForeColor = Color.Gray;

            tb_createusername.Text = "Create Username";
            tb_createusername.ForeColor = Color.Gray;

            tb_createpassword.Text = "Create Password";
            tb_createpassword.ForeColor = Color.Gray;

            tb_confirmpassword.Text = "Confirm Password";
            tb_confirmpassword.ForeColor = Color.Gray;

            tb_fullname.Text = "Enter Full Name";
            tb_fullname.ForeColor = Color.Gray;


        }

       

        private void pb_visibilityoff_Click(object sender, EventArgs e)
        {
            pb_visibilityon.BringToFront();
            tb_password.PasswordChar = '\0';
            pb_visibilityoff.Visible = false;
            pb_visibilityon.Visible = true;
            passwordVisible = true;
        }

        private void pb_visibilityon_Click(object sender, EventArgs e)
        {
            pb_visibilityoff.BringToFront();
            tb_password.PasswordChar = '*';
            pb_visibilityoff.Visible = true;
            pb_visibilityon.Visible = false;
            passwordVisible = false;
        }

        private void tb_username_Enter(object sender, EventArgs e)
        {
            if (tb_username.Text == "Enter Username")
            {
                tb_username.Text = "";
                tb_username.ForeColor = Color.White; // Change text color to black
            }
        }

        private void tb_username_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_username.Text))
            {
                tb_username.Text = "Enter Username";
                tb_username.ForeColor = Color.Gray; // Change text color to gray
            }
        }

        private void tb_password_Enter(object sender, EventArgs e)
        {
            if (tb_password.Text == "Enter Password")
            {
                tb_password.Text = "";
                tb_password.PasswordChar = '*';
                tb_password.ForeColor = Color.White; // Change text color to black
            }
        }

        private void tb_password_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_password.Text))
            {
                tb_password.PasswordChar = '\0';
                tb_password.Text = "Enter Password";
                tb_password.ForeColor = Color.Gray; // Change text color to gray
            }
        }

       
        private void btn_login_MouseEnter(object sender, EventArgs e)
        {
            btn_login.BackColor = Color.FromArgb(255, 50, 50);
        }

        private void btn_login_Leave(object sender, EventArgs e)
        {
            btn_login.BackColor = Color.FromArgb(200, 30, 30);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            pnl_register.BringToFront();
        }

     

        private void pb_vis2_Click(object sender, EventArgs e)
        {
            pb_vison2.BringToFront();
            tb_createpassword.PasswordChar = '*';
            pb_vison2.Visible = true;
            pb_visoff2.Visible = false;
            passwordVisible = false;
        }

        private void pb_vison2_Click(object sender, EventArgs e)
        {
            pb_visoff2.BringToFront();
            tb_createpassword.PasswordChar = '\0';
            pb_visoff2.Visible = true;
            pb_vison2.Visible = false;
            passwordVisible = false;
        }

        private void pb_vison3_Click(object sender, EventArgs e)
        {
            pb_visoff3.BringToFront();
            tb_confirmpassword.PasswordChar = '*';
            pb_visoff3.Visible = true;
            pb_vison3.Visible = false;
            passwordVisible = false;
        }

        private void pb_visoff3_Click(object sender, EventArgs e)
        {
            pb_vison3.BringToFront();
            tb_confirmpassword.PasswordChar = '\0';
            pb_vison3.Visible = true;
            pb_visoff3.Visible = false;
            passwordVisible = false;
        }

        private void tb_createusername_Enter(object sender, EventArgs e)
        {
            if (tb_createusername.Text == "Create Username")
            {
                tb_createusername.Text = "";
                tb_createusername.ForeColor = Color.White; // Change text color to black
            }
        }

        private void tb_createusername_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_createusername.Text))
            {
                tb_createusername.Text = "Create Username";
                tb_createusername.ForeColor = Color.Gray; // Change text color to gray
            }
        }

        private void tb_createpassword_Enter(object sender, EventArgs e)
        {
            if (tb_createpassword.Text == "Create Password")
            {
                tb_createpassword.Text = "";
                tb_createpassword.PasswordChar = '*';
                tb_createpassword.ForeColor = Color.White; // Change text color to black
            }
        }

        private void tb_createpassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_createpassword.Text))
            {
                tb_createpassword.PasswordChar = '\0';
                tb_createpassword.Text = "Create Password";
                tb_createpassword.ForeColor = Color.Gray; // Change text color to gray
            }
        }

        private void tb_confirmpassword_Enter(object sender, EventArgs e)
        {
            if (tb_confirmpassword.Text == "Confirm Password")
            {
                tb_confirmpassword.Text = "";
                tb_confirmpassword.PasswordChar = '*';
                tb_confirmpassword.ForeColor = Color.White; // Change text color to black
            }
        }

        private void tb_confirmpassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_confirmpassword.Text))
            {
                tb_confirmpassword.PasswordChar = '\0';
                tb_confirmpassword.Text = "Confirm Password";
                tb_confirmpassword.ForeColor = Color.Gray; // Change text color to gray
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            var auth = new PrelimsBoy.Services.authServices();
            var user = auth.Login(tb_username.Text.Trim(), tb_password.Text, out var msg);

            if (user == null)
            {
                MessageBox.Show(msg ?? "Invalid username or password.",
                    "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (user.Role == "Admin" || user.Role == "Student")
            {
                new frm_Dashboard().Show();
                this.Hide();
            }
            else if (user.Role == "SuperAdmin")
            {
                new frm_superadmin().Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Unknown role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lbl_BckLgin_Click(object sender, EventArgs e)
        {
            pnl_login.BringToFront();
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            var role = cmb_role.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(tb_fullname.Text) ||
                tb_fullname.Text == "Enter Full Name" ||
                string.IsNullOrWhiteSpace(tb_createusername.Text) ||
                tb_createusername.Text == "Create Username" ||
                string.IsNullOrWhiteSpace(tb_createpassword.Text) ||
                tb_createpassword.Text == "Create Password" ||
                string.IsNullOrWhiteSpace(tb_confirmpassword.Text) ||
                tb_confirmpassword.Text == "Confirm Password" ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please complete all fields.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tb_createpassword.Text != tb_confirmpassword.Text)
            {
                MessageBox.Show("Passwords do not match.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var auth = new authServices();
            if (auth.Register(tb_fullname.Text.Trim(), tb_createusername.Text.Trim(), role.Trim(), out var msg))
            {
                MessageBox.Show(msg, "Register", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // clear inputs
                tb_fullname.Text = "Enter Full Name"; tb_fullname.ForeColor = System.Drawing.Color.Gray;
                tb_createusername.Text = "Create Username"; tb_createusername.ForeColor = System.Drawing.Color.Gray;
                tb_createpassword.Text = "Create Password"; tb_createpassword.PasswordChar = '\0'; tb_createpassword.ForeColor = System.Drawing.Color.Gray;
                tb_confirmpassword.Text = "Confirm Password"; tb_confirmpassword.PasswordChar = '\0'; tb_confirmpassword.ForeColor = System.Drawing.Color.Gray;
                cmb_role.SelectedIndex = -1;

                // refresh superadmin grids if open
                var openAdmin = (frm_superadmin)Application.OpenForms["frm_superadmin"];
                if (openAdmin != null) openAdmin.LoadData();
            }
            else
            {
                MessageBox.Show(msg ?? "Registration failed.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tb_fullname_Enter(object sender, EventArgs e)
        {
            if (tb_fullname.Text == "Enter Full Name")
            {
                tb_fullname.Text = "";
                tb_fullname.ForeColor = Color.White; // Change text color to black
            }
        }

        private void tb_fullname_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_fullname.Text))
            {
                tb_fullname.Text = "Enter Full name";
                tb_fullname.ForeColor = Color.Gray; // Change text color to gray
            }
        }
    }
}