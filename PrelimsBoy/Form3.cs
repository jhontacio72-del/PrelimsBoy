using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace PrelimsBoy
{
    public partial class frm_superadmin : Form
    {
        string connString = "server=localhost;database=user_db;uid=root;pwd=;";
        public frm_superadmin()
        {
            InitializeComponent();
            SetupGrid();
            LoadData();
            SetupInstructorGrid();
        }
        private void SetupGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("colNumber", "Registered User");
            dataGridView1.Columns.Add("colName", "Name");
            dataGridView1.Columns.Add("colUsername", "Username"); // New Column
            dataGridView1.Columns.Add("colRole", "Role");
            dataGridView1.Columns.Add("colStatus", "Status");    // New Column

            dataGridView1.Columns["colNumber"].DataPropertyName = "id";
            dataGridView1.Columns["colName"].DataPropertyName = "name";
            dataGridView1.Columns["colUsername"].DataPropertyName = "username";
            dataGridView1.Columns["colRole"].DataPropertyName = "role";
            dataGridView1.Columns["colStatus"].DataPropertyName = "status";

            // Make it Read-Only
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoGenerateColumns = false; // Prevents duplicate columns
        }
        private void SetupInstructorGrid()
        {
            dt_manageinstructor.Columns.Clear();
            dt_manageinstructor.AutoGenerateColumns = false;

            dt_manageinstructor.Columns.Add("instId", "Registered User");
            dt_manageinstructor.Columns["instId"].DataPropertyName = "id";

            dt_manageinstructor.Columns.Add("instName", "Instructor Name");
            dt_manageinstructor.Columns["instName"].DataPropertyName = "name";

            dt_manageinstructor.Columns.Add("instStatus", "Status");
            dt_manageinstructor.Columns["instStatus"].DataPropertyName = "status";

            // Optional: Add a Button Column so you can click "Approve" directly in the grid
            DataGridViewButtonColumn btn_approve = new DataGridViewButtonColumn();
            btn_approve.Name = "colApprove";
            btn_approve.HeaderText = "Action";
            btn_approve.Text = "Approve";
            btn_approve.UseColumnTextForButtonValue = true;
            dt_manageinstructor.Columns.Add(btn_approve);



            // === NEW: Add a Reject Button Column ===
            DataGridViewButtonColumn btn_reject = new DataGridViewButtonColumn();
            btn_reject.Name = "colReject"; // IMPORTANT: Unique name for this button
            btn_reject.HeaderText = ""; // No header text for the reject column, or "Reject"
            btn_reject.Text = "Reject";
            btn_reject.UseColumnTextForButtonValue = true;
            dt_manageinstructor.Columns.Add(btn_reject);
            // =====================================
        }
        public void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    // GRID 1: Main Users (Students and Admins who are Active)
                    string queryActive = "SELECT id, name, username, role, status FROM users WHERE role = 'Student' AND status = 'Active' ORDER BY id DESC";
                    MySqlDataAdapter adapter1 = new MySqlDataAdapter(queryActive, conn);
                    DataTable dtActive = new DataTable();
                    adapter1.Fill(dtActive);
                    dataGridView1.DataSource = dtActive;

                    // GRID 2: Manage Instructors (Only those with Role 'Instructor')
                    // You can choose to show both 'Pending' and 'Active' instructors here
                    string queryInstructors = "SELECT id, name, username, role, status FROM users WHERE role = 'Instructor' ORDER BY id DESC";
                    MySqlDataAdapter adapter2 = new MySqlDataAdapter(queryInstructors, conn);
                    DataTable dtInstructors = new DataTable();
                    adapter2.Fill(dtInstructors);
                    dt_manageinstructor.DataSource = dtInstructors;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
           

            // Change the header text specifically
         
            dataGridView1.Columns["colNumber"].DataPropertyName = "id";
            
            
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            dataGridView1.BorderStyle = BorderStyle.None;
            pictureBox1.SizeMode=PictureBoxSizeMode.StretchImage;   
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.LightGray;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Transparent;
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            panel1.BackColor = Color.LightGray;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.DimGray;
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            pnl_maninstructor.BackColor = Color.LightGray;
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            pnl_maninstructor.BackColor = Color.DimGray;
        }

        private void panel3_MouseEnter(object sender, EventArgs e)
        {
            panel3.BackColor = Color.LightGray;
        }

        private void panel3_MouseLeave(object sender, EventArgs e)
        {
            panel3.BackColor = Color.DimGray;
        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {
            panel4.BackColor = Color.LightGray;
        }

        private void panel4_MouseLeave(object sender, EventArgs e)
        {
            panel4.BackColor = Color.DimGray;
        }

        private void panel5_MouseEnter(object sender, EventArgs e)
        {
            panel5.BackColor = Color.LightGray;
        }

        private void panel5_MouseLeave(object sender, EventArgs e)
        {
            panel5.BackColor = Color.DimGray;
        }

        private void panel2_MouseLeave_1(object sender, EventArgs e)
        {
            pnl_maninstructor.BackColor = Color.DimGray;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            frm_Home homeForm = new frm_Home();
            homeForm.Show();
            this.Hide();
        }

        private void dt_manageinstructor_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a button column was clicked and it's a valid row
            if (e.RowIndex >= 0)
            {
                // Declare userId here so it's accessible to both if/else if blocks
                int userId = Convert.ToInt32(dt_manageinstructor.Rows[e.RowIndex].Cells["instId"].Value);
                // If the clicked cell is the Approve button
                if (dt_manageinstructor.Columns[e.ColumnIndex].Name == "colApprove")
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();
                        string query = "UPDATE users SET status = 'Active' WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Instructor Approved!");
                    LoadData(); // Refresh both grids
                }
                // If the clicked cell is the Reject button
                else if (dt_manageinstructor.Columns[e.ColumnIndex].Name == "colReject")
                {
                    DialogResult dialogResult = MessageBox.Show(
                        "Are you sure you want to reject this instructor? This will permanently delete their account.",
                        "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            using (MySqlConnection conn = new MySqlConnection(connString))
                            {
                                conn.Open();
                                // Query to delete the user from the database
                                string query = "DELETE FROM users WHERE id = @id";
                                MySqlCommand cmd = new MySqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@id", userId);
                                cmd.ExecuteNonQuery();
                            }
                            MessageBox.Show("Instructor Rejected and Account Deleted.");
                            LoadData(); // Refresh both grids to show the change
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error rejecting instructor: " + ex.Message);
                        }
                    }
                }
            }
            }


        private void panel6_Click(object sender, EventArgs e)
        {
            pnl_content1.BringToFront();
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            pnl_manageinstructor.BringToFront();
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            pnl_billing.BringToFront();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Click(object sender, EventArgs e)
        {
            pnl_account.BringToFront();
        }

        private void panel4_Click(object sender, EventArgs e)
        {
            pnl_manageEnrollment.BringToFront();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {
            pnl_manageCourse.BringToFront();
        }

        private void pnl_homepage_MouseEnter(object sender, EventArgs e)
        {
            pnl_homepage.BackColor = Color.LightGray;
        }

        private void pnl_homepage_MouseLeave(object sender, EventArgs e)
        {
            pnl_homepage.BackColor = Color.DimGray;
        }
    }
}
