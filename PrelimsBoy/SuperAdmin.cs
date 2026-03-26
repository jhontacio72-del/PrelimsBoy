using System;
using PrelimsBoy.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public frm_superadmin()
        {
            InitializeComponent();
            btn_searchuser.Click += btn_searchuser_Click;
            tb_searchuser.KeyDown += tb_searchuser_KeyDown;

            // initial load for users grid (replaces the students part of LoadData if you prefer)
            LoadUsers();
            cb_pendingonly.CheckedChanged += cb_pendingonly_CheckedChanged;
            btn_search.Click += btn_search_Click;
            tb_search.KeyDown += tb_search_KeyDown;

            // initial load (replace the instructor part of LoadData with this call)
            LoadInstructors();
            SetupGrid();
            LoadData();
            SetupInstructorGrid();
            dt_course.CellClick += dt_course_CellClick;
            SetupCourseGrid();

            SetupSubjectGrid();
            InitUnitsCombo();
            LoadSubjects();

            // Wire subject buttons (safe to re-wire)
            btn_add.Click -= btn_add_Click; btn_add.Click += btn_add_Click;
            btn_clear.Click -= btn_clear_Click; btn_clear.Click += btn_clear_Click;
            btn_update.Click -= btn_update_Click; btn_update.Click += btn_update_Click;
            btn_delete.Click -= btn_delete_Click; btn_delete.Click += btn_delete_Click;
        }
        private void SetupGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("colNumber", "ID");
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

            dt_manageinstructor.Columns.Add("instId", "ID");
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




            DataGridViewButtonColumn btn_reject = new DataGridViewButtonColumn();
            btn_reject.Name = "colReject";
            btn_reject.HeaderText = "";
            btn_reject.Text = "Reject";
            btn_reject.UseColumnTextForButtonValue = true;
            dt_manageinstructor.Columns.Add(btn_reject);

        }

        private void SetupCourseGrid()
        {
            dt_course.Columns.Clear(); // Clear existing columns
            dt_course.AutoGenerateColumns = false; // We'll define them manually

            // Add columns for your courses table
            dt_course.Columns.Add("courseId", "ID");
            dt_course.Columns["courseId"].DataPropertyName = "course_id"; // Matches database column name

            dt_course.Columns.Add("courseCode", "Course Code");
            dt_course.Columns["courseCode"].DataPropertyName = "course_code";

            dt_course.Columns.Add("courseName", "Course Name");
            dt_course.Columns["courseName"].DataPropertyName = "course_name";

            dt_course.Columns.Add("courseDesc", "Description");
            dt_course.Columns["courseDesc"].DataPropertyName = "description";

            dt_course.Columns.Add("courseStatus", "Active"); // For is_active field
            dt_course.Columns["courseStatus"].DataPropertyName = "is_active";

            dt_course.ReadOnly = true;
            dt_course.AllowUserToAddRows = false;
            dt_course.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Makes columns fill the grid
        }
        // --- END NEW: Setup for Courses DataGridView ---
        public void LoadData()
        {
            try
            {
                // Use the shared Database.GetConnection() method for user data
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) // Check if connection was successful
                    {
                        MessageBox.Show("Failed to establish database connection for user data.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // GRID 1: Main Users (Students and Admins who are Active)
                    string queryActive = "SELECT id, name, username, role, status FROM users WHERE role = 'Student' AND status = 'Active' ORDER BY id DESC";
                    MySqlDataAdapter adapter1 = new MySqlDataAdapter(queryActive, conn);
                    DataTable dtActive = new DataTable();
                    adapter1.Fill(dtActive);
                    dataGridView1.DataSource = dtActive;

                    // GRID 2: Manage Instructors (Only those with Role 'Instructor')
                    string queryInstructors = "SELECT id, name, username, role, status FROM users WHERE role = 'Instructor' ORDER BY id DESC";
                    MySqlDataAdapter adapter2 = new MySqlDataAdapter(queryInstructors, conn);
                    DataTable dtInstructors = new DataTable();
                    adapter2.Fill(dtInstructors);
                    dt_manageinstructor.DataSource = dtInstructors;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user data: " + ex.Message);
            }

        }
        // --- NEW: Load Courses into dt_course ---
        private void LoadCourses()
        {
            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null)
                    {
                        MessageBox.Show("Failed to establish database connection for courses.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string query = "SELECT course_id, course_code, course_name, description, is_active FROM courses ORDER BY course_id DESC";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dt_course.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading courses: " + ex.Message);
            }
        }
        // --- END NEW: Load Courses into dt_course ---
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {


            // Change the header text specifically

            dataGridView1.Columns["colNumber"].DataPropertyName = "id";


            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.BorderStyle = BorderStyle.None;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.LightGray;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Gray;
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
                int userId = Convert.ToInt32(dt_manageinstructor.Rows[e.RowIndex].Cells["instId"].Value);
                if (dt_manageinstructor.Columns[e.ColumnIndex].Name == "colApprove")
                {
                    // Use Helpers.Database.GetConnection() here
                    using (MySqlConnection conn = Helpers.Database.GetConnection())
                    {
                        if (conn == null) return; // Connection failed, message already shown
                        string query = "UPDATE users SET status = 'Active' WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Instructor Approved!");
                    LoadData(); // Refresh both grids
                }
                else if (dt_manageinstructor.Columns[e.ColumnIndex].Name == "colReject")
                {
                    DialogResult dialogResult = MessageBox.Show(
                        "Are you sure you want to reject this instructor? This will permanently delete their account.",
                        "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            // Use Helpers.Database.GetConnection() here
                            using (MySqlConnection conn = Helpers.Database.GetConnection())
                            {
                                if (conn == null) return; // Connection failed, message already shown
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
        // --- NEW: Event Handlers for Course Management ---

        private void btn_addcourse_Click(object sender, EventArgs e)
        {
            // Basic input validation
            if (string.IsNullOrWhiteSpace(tb_coursecode.Text) || string.IsNullOrWhiteSpace(tb_coursename.Text))
            {
                MessageBox.Show("Course Code and Course Name cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return; // Connection failed, message already shown by Helpers.Database.GetConnection()

                    string query = "INSERT INTO courses (course_code, course_name, description) VALUES (@code, @name, @description)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@code", tb_coursecode.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", tb_coursename.Text.Trim());
                    // Description can be NULL, so check if textbox is empty
                    cmd.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(tb_description.Text) ? (object)DBNull.Value : tb_description.Text.Trim());

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Course added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearCourseFields(); // Clear textboxes after successful add
                        LoadCourses(); // Refresh the DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Failed to add course.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle specific MySQL errors, e.g., duplicate course_code
                if (ex.Number == 1062) // MySQL error code for duplicate entry
                {
                    MessageBox.Show("A course with this Course Code already exists. Please use a unique code.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database error adding course: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_clearcourse_Click(object sender, EventArgs e)
        {
            ClearCourseFields();
        }

        private void ClearCourseFields()
        {
            tb_coursecode.Clear();
            tb_coursename.Clear();
            tb_description.Clear();
            // You might want to clear any selection in the DataGridView too
            dt_course.ClearSelection();
        }

        // You'll need to implement btn_updatecourse and btn_deletecourse similarly
        // For update, you'd likely get the selected row's data, populate the textboxes,
        // then update when the button is clicked.
        // For delete, you'd confirm and then delete the selected row's course.

        // Placeholder for update (you'll need logic to get selected row data into textboxes first)
        private void btn_updatecourse_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Update functionality not yet implemented.");
            // Example:
            // if (dt_course.SelectedRows.Count > 0)
            // {
            // int courseId = Convert.ToInt32(dt_course.SelectedRows[0].Cells["courseId"].Value);
            // //... validation...
            // using (MySqlConnection conn = Helpers.Database.GetConnection()) {... UPDATE statement... }
            // }
        }

        // Placeholder for delete
        private void btn_deletecourse_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Delete functionality not yet implemented.");
            // Example:
            // if (dt_course.SelectedRows.Count > 0)
            // {
            // DialogResult confirm = MessageBox.Show("Are you sure you want to delete this course?", "Confirm Delete", MessageBoxButtons.YesNo);
            // if (confirm == DialogResult.Yes)
            // {
            // int courseId = Convert.ToInt32(dt_course.SelectedRows[0].Cells["courseId"].Value);
            // using (MySqlConnection conn = Helpers.Database.GetConnection()) {... DELETE statement... }
            // }
            // }
        }

        private void dt_course_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dt_course_Click(object sender, EventArgs e)
        {

        }

        private void dt_course_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dt_course.Rows[e.RowIndex].Cells["courseId"].Value != null)
            {
                DataGridViewRow row = dt_course.Rows[e.RowIndex];
                tb_coursecode.Text = row.Cells["courseCode"].Value?.ToString() ?? "";
                tb_coursename.Text = row.Cells["courseName"].Value?.ToString() ?? "";
                tb_description.Text = row.Cells["courseDesc"].Value?.ToString() ?? "";
            }
        }

        private void btn_addcourse_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_coursecode.Text) || string.IsNullOrWhiteSpace(tb_coursename.Text))
            {
                MessageBox.Show("Course Code and Course Name cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return;

                    string query = "INSERT INTO courses (course_code, course_name, description) VALUES (@code, @name, @description)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@code", tb_coursecode.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", tb_coursename.Text.Trim());
                    cmd.Parameters.AddWithValue("@description",
                        string.IsNullOrWhiteSpace(tb_description.Text) ? (object)DBNull.Value : tb_description.Text.Trim());

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Course added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearCourseFields();
                        LoadCourses();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add course.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // duplicate course_code
                    MessageBox.Show("A course with this Course Code already exists. Please use a unique code.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Database error adding course: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        private void btn_clearcourse_Click_1(object sender, EventArgs e)
        {
            tb_coursecode.Clear();
            tb_coursename.Clear();
            tb_description.Clear();
            dt_course.ClearSelection();
        }

        private void btn_updatecourse_Click_1(object sender, EventArgs e)
        {
            if (dt_course.CurrentRow == null || dt_course.CurrentRow.Cells["courseId"].Value == null)
            {
                MessageBox.Show("Please select a course from the list to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(tb_coursecode.Text) || string.IsNullOrWhiteSpace(tb_coursename.Text))
            {
                MessageBox.Show("Course Code and Course Name cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int courseId = Convert.ToInt32(dt_course.CurrentRow.Cells["courseId"].Value);

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"UPDATE courses 
                             SET course_code = @code, 
                                 course_name = @name, 
                                 description = @description 
                             WHERE course_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@code", tb_coursecode.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", tb_coursename.Text.Trim());
                    cmd.Parameters.AddWithValue("@description",
                        string.IsNullOrWhiteSpace(tb_description.Text) ? (object)DBNull.Value : tb_description.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", courseId);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Course updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCourses();
                        ClearCourseFields();
                    }
                    else
                    {
                        MessageBox.Show("No changes were made.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    MessageBox.Show("Another course already uses that Course Code.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Database error updating course: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_deletecourse_Click_1(object sender, EventArgs e)
        {
            if (dt_course.CurrentRow == null || dt_course.CurrentRow.Cells["courseId"].Value == null)
            {
                MessageBox.Show("Please select a course from the list to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int courseId = Convert.ToInt32(dt_course.CurrentRow.Cells["courseId"].Value);
            string courseCode = dt_course.CurrentRow.Cells["courseCode"].Value?.ToString() ?? "";

            var confirm = MessageBox.Show($"Delete course '{courseCode}'? This cannot be undone.",
                                          "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return;

                    string query = "DELETE FROM courses WHERE course_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", courseId);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Course deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCourses();
                        ClearCourseFields();
                    }
                    else
                    {
                        MessageBox.Show("Delete failed—course not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451) // foreign key constraint
                    MessageBox.Show("This course can’t be deleted because it’s referenced by other records.", "Constraint", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Database error deleting course: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int selectedSubjectId = -1; // tracks the row you clicked

        private void SetupSubjectGrid()
        {
            dg_subjects.Columns.Clear();
            dg_subjects.AutoGenerateColumns = false;

            dg_subjects.Columns.Add("subId", "ID");
            dg_subjects.Columns["subId"].DataPropertyName = "subject_id";

            dg_subjects.Columns.Add("subCode", "Subject Code");
            dg_subjects.Columns["subCode"].DataPropertyName = "subject_code";

            dg_subjects.Columns.Add("subName", "Subject Name");
            dg_subjects.Columns["subName"].DataPropertyName = "subject_name";

            dg_subjects.Columns.Add("subUnits", "Units");
            dg_subjects.Columns["subUnits"].DataPropertyName = "units";

            dg_subjects.Columns.Add("subDesc", "Description");
            dg_subjects.Columns["subDesc"].DataPropertyName = "description";

            dg_subjects.Columns.Add("subActive", "Active");
            dg_subjects.Columns["subActive"].DataPropertyName = "is_active";

            dg_subjects.ReadOnly = true;
            dg_subjects.AllowUserToAddRows = false;
            dg_subjects.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dg_subjects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // wire events (safe to re-wire)
            dg_subjects.CellClick -= dg_subjects_CellClick;
            dg_subjects.CellClick += dg_subjects_CellClick;
        }
        private void LoadSubjects()
        {
            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null)
                    {
                        MessageBox.Show("Failed to establish database connection for subjects.",
                            "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string query = @"SELECT subject_id, subject_code, subject_name, units, description, is_active
                             FROM subjects
                             ORDER BY subject_id DESC";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dg_subjects.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subjects: " + ex.Message);
            }
        }

        private void InitUnitsCombo()
        {
            cb_units.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_units.Items.Clear();
            // common unit values; adjust if you need others
            cb_units.Items.AddRange(new object[] { "1.0", "2.0", "3.0", "4.0", "5.0", "6.0" });
            cb_units.SelectedItem = "3.0";
        }
        private void ClearSubjectFields()
        {
            tb_subcode.Clear();
            tb_subname.Clear();
            tb_subdesc.Clear();
            if (cb_units.Items.Count > 0) cb_units.SelectedItem = "3.0";
            dg_subjects.ClearSelection();
            selectedSubjectId = -1;
        }

        private void dg_subjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dg_subjects.Rows[e.RowIndex];
            if (row.Cells["subId"].Value == null) return;

            selectedSubjectId = Convert.ToInt32(row.Cells["subId"].Value);
            tb_subcode.Text = row.Cells["subCode"].Value?.ToString() ?? "";
            tb_subname.Text = row.Cells["subName"].Value?.ToString() ?? "";
            tb_subdesc.Text = row.Cells["subDesc"].Value?.ToString() ?? "";

            string unitsStr = row.Cells["subUnits"].Value?.ToString() ?? "3.0";
            if (decimal.TryParse(unitsStr, out decimal u))
                cb_units.SelectedItem = u.ToString("0.0");
            else
                cb_units.SelectedItem = "3.0";
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_subcode.Text) || string.IsNullOrWhiteSpace(tb_subname.Text))
            {
                MessageBox.Show("Subject Code and Subject Name cannot be empty.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cb_units.SelectedItem == null || !decimal.TryParse(cb_units.SelectedItem.ToString(), out decimal units))
            {
                MessageBox.Show("Please select a valid Units value.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return;

                    // INSERT with description
                    string q = @"INSERT INTO subjects (subject_code, subject_name, units, description)
                         VALUES (@code, @name, @units, @desc)";
                    using (var cmd = new MySqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", tb_subcode.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", tb_subname.Text.Trim());
                        cmd.Parameters.AddWithValue("@units", units);
                        cmd.Parameters.AddWithValue("@desc",
                            string.IsNullOrWhiteSpace(tb_subdesc.Text) ? (object)DBNull.Value : tb_subdesc.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Subject added successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearSubjectFields();
                            LoadSubjects();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add subject.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    MessageBox.Show("A subject with this Subject Code already exists. Please use a unique code.",
                        "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Database error adding subject: " + ex.Message,
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearSubjectFields();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId <= 0)
            {
                MessageBox.Show("Please select a subject from the list to update.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(tb_subcode.Text) || string.IsNullOrWhiteSpace(tb_subname.Text))
            {
                MessageBox.Show("Subject Code and Subject Name cannot be empty.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cb_units.SelectedItem == null || !decimal.TryParse(cb_units.SelectedItem.ToString(), out decimal units))
            {
                MessageBox.Show("Please select a valid Units value.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return;

                    // UPDATE with description
                    string q2 = @"UPDATE subjects
                          SET subject_code=@code, subject_name=@name, units=@units, description=@desc
                          WHERE subject_id=@id";
                    using (var cmd = new MySqlCommand(q2, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", tb_subcode.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", tb_subname.Text.Trim());
                        cmd.Parameters.AddWithValue("@units", units);
                        cmd.Parameters.AddWithValue("@desc",
                            string.IsNullOrWhiteSpace(tb_subdesc.Text) ? (object)DBNull.Value : tb_subdesc.Text.Trim());
                        cmd.Parameters.AddWithValue("@id", selectedSubjectId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Subject updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSubjects();
                            ClearSubjectFields();
                        }
                        else
                        {
                            MessageBox.Show("No changes were made.", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    MessageBox.Show("Another subject already uses that Subject Code.",
                        "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Database error updating subject: " + ex.Message,
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId <= 0)
            {
                MessageBox.Show("Please select a subject from the list to delete.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string code = tb_subcode.Text.Trim();
            var confirm = MessageBox.Show($"Delete subject '{code}'? This cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = Helpers.Database.GetConnection())
                {
                    if (conn == null) return;

                    string query = "DELETE FROM subjects WHERE subject_id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedSubjectId);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Subject deleted.", "Deleted",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSubjects();
                            ClearSubjectFields();
                        }
                        else
                        {
                            MessageBox.Show("Delete failed—subject not found.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451) // FK constraint
                    MessageBox.Show("This subject can’t be deleted because it’s referenced by other records.",
                        "Constraint", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Database error deleting subject: " + ex.Message,
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadInstructors()
        {
            bool pendingOnly = cb_pendingonly.Checked;
            string keyword = (tb_search.Text ?? "").Trim();
            LoadInstructors(pendingOnly, keyword);
        }

        private void LoadInstructors(bool pendingOnly, string keyword)
        {
            try
            {
                using (var conn = Helpers.Database.GetConnection())
                {
                    if (conn == null)
                    {
                        MessageBox.Show("Failed to establish database connection for instructors.",
                            "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var sql = new StringBuilder(@"
                SELECT id, name, username, role, status
                FROM users
                WHERE role = 'Instructor'
            ");

                    using (var cmd = new MySqlCommand() { Connection = conn })
                    {
                        if (pendingOnly)
                        {
                            sql.Append(" AND status = 'Pending'");
                        }

                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            // search by id OR name (case-insensitive)
                            sql.Append(" AND (name LIKE @kw OR CAST(id AS CHAR) LIKE @kw)");
                            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                        }

                        sql.Append(" ORDER BY id DESC");
                        cmd.CommandText = sql.ToString();

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            dt_manageinstructor.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading instructors: " + ex.Message);
            }
        }

        private void cb_pendingonly_CheckedChanged(object sender, EventArgs e)
        {
            LoadInstructors();
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            LoadInstructors();
        }

        private void tb_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadInstructors();
            }
        }
        private void LoadUsers()
        {
            string keyword = (tb_searchuser.Text ?? "").Trim();
            LoadUsers(keyword);
        }

        private void LoadUsers(string keyword)
        {
            try
            {
                using (var conn = Helpers.Database.GetConnection())
                {
                    if (conn == null)
                    {
                        MessageBox.Show("Failed to establish database connection for user data.",
                            "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var sql = new StringBuilder(@"
                SELECT id, name, username, role, status
                FROM users
                WHERE 1=1
            ");

                    using (var cmd = new MySqlCommand() { Connection = conn })
                    {
                        // If you want to limit to Students only, uncomment the next line:
                        // sql.Append(" AND role = 'Student'");

                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            // Match by ID or Name (case-insensitive LIKE)
                            sql.Append(" AND (name LIKE @kw OR CAST(id AS CHAR) LIKE @kw)");
                            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                        }

                        sql.Append(" ORDER BY id DESC");
                        cmd.CommandText = sql.ToString();

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void btn_searchuser_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void tb_searchuser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadUsers();
            }
        }
        // --- END NEW: Event Handlers for Course Management ---
    }
}

