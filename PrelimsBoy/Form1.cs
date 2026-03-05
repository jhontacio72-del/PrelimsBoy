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


        private void pnl_right_Paint(object sender, PaintEventArgs e)
        {

        }
        private void StyleTextbox(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(60, 60, 60);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.None;


        }

        private bool passwordVisible = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb_password_TextChanged(object sender, EventArgs e)
        {

        }

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

        private void label2_Click(object sender, EventArgs e)
        {

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

        private void btn_login_Enter(object sender, EventArgs e)
        {

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

        private void label10_Click(object sender, EventArgs e)
        {
            pnl_login.BringToFront();
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
            if (tb_username.Text == "admin" && tb_password.Text == "admin123")
            {
                frm_Dashboard dashboard = new frm_Dashboard();
                dashboard.Show();
                this.Hide();
            }

            else if
                (tb_username.Text == "superadmin" && tb_password.Text == "admin1234")
            {
                frm_superadmin dashboards = new frm_superadmin();
                dashboards.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void tb_createusername_TextChanged(object sender, EventArgs e)
        {

        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
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
                tb_fullname.Text = "Enter Full Name";
                tb_fullname.ForeColor = Color.Gray; // Change text color to gray
            }
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            string role = cmb_role.SelectedItem?.ToString();
            string status = (role == "Instructor") ? "Pending" : "Active";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    // Added username and status to the insert query
                    string query = "INSERT INTO users (name, username, role, status) VALUES (@name, @user, @role, @status)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", tb_fullname.Text);
                        cmd.Parameters.AddWithValue("@user", tb_createusername.Text);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@status", status);

                        cmd.ExecuteNonQuery();

                        if (role == "Instructor")
                        {
                            MessageBox.Show("Registered successfully! Please wait for admin approval.", "Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Student registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        // Refresh the SuperAdmin grid if it's open
                        frm_superadmin openAdminForm = (frm_superadmin)Application.OpenForms["frm_superadmin"];
                        if (openAdminForm != null)
                        {
                            openAdminForm.LoadData();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

    }
}