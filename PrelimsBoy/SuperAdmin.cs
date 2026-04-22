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
using PrelimsBoy.Models;
using PrelimsBoy.Services;
namespace PrelimsBoy
{
    public partial class frm_superadmin : Form
    {
        private readonly BillingService _billing = new BillingService();
        private int selectedBillingId = -1;
        private readonly SuperadminServices _users = new SuperadminServices();
        private readonly CourseServices _courses = new CourseServices();
        private readonly SubjectServices _subjects = new SubjectServices();
        private int selectedSubjectId = -1;
        private readonly CourseSubjectService _courseSubject = new CourseSubjectService();
        private int selectedCourseId = -1;
        private Stack<int> undoStack = new Stack<int>();
        private readonly ClassOfferingService _classOffering = new ClassOfferingService();
        private int selectedClassId = -1;
        public frm_superadmin()
        {
            InitializeComponent();
            SetupGrid();
            SetupBillingGrid();
            LoadBilling();
            LoadStudentCombo();
            SetupInstructorGrid();
            SetupCourseGrid();
            SetupSubjectGrid();
            InitUnitsCombo();
            LoadUsers();
            LoadInstructors();
            LoadCourses();
            LoadSubjects();
            SetupCourseSubjectGrids();
            LoadCourseCombo();
            SetupClassOfferingGrid();
            LoadClassOfferingCombos();
            LoadClassOfferings();
            cb_course.SelectedIndexChanged += cb_course_SelectedIndexChanged;
            btn_classoffcreate.Click += btn_classoffcreate_Click;
            btn_classoffupdate.Click += btn_classoffupdate_Click;
            btn_classoffclear.Click += btn_classoffclear_Click;
            btn_deleteorenable.Click += btn_deleteorenable_Click;
            btn_searchuser.Click += (s, e) => LoadUsers(tb_searchuser.Text?.Trim());
            tb_searchuser.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadUsers(tb_searchuser.Text?.Trim()); } };
            cb_pendingonly.CheckedChanged += (s, e) => LoadInstructors(cb_pendingonly.Checked, tb_search.Text?.Trim());
            btn_search.Click += (s, e) => LoadInstructors(cb_pendingonly.Checked, tb_search.Text?.Trim());
            tb_search.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadInstructors(cb_pendingonly.Checked, tb_search.Text?.Trim()); } };
            dt_course.CellClick += dt_course_CellClick;
            dg_subjects.CellClick += dg_subjects_CellClick;
            dt_manageinstructor.CellContentClick += dt_manageinstructor_CellContentClick; // <-- here

        }
        private void SetupGrid()
        {

            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNumber", HeaderText = "ID", DataPropertyName = "id" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Name", DataPropertyName = "name" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUsername", HeaderText = "Username", DataPropertyName = "username" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRole", HeaderText = "Role", DataPropertyName = "role" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", DataPropertyName = "status" });
        }
        private void SetupInstructorGrid()
        {

            dt_manageinstructor.Columns.Clear();
            dt_manageinstructor.AutoGenerateColumns = false;
            dt_manageinstructor.ReadOnly = true;
            dt_manageinstructor.AllowUserToAddRows = false;
            dt_manageinstructor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dt_manageinstructor.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_manageinstructor.Columns.Add(new DataGridViewTextBoxColumn { Name = "instId", HeaderText = "ID", DataPropertyName = "id" });
            dt_manageinstructor.Columns.Add(new DataGridViewTextBoxColumn { Name = "instName", HeaderText = "Instructor Name", DataPropertyName = "name" });
            dt_manageinstructor.Columns.Add(new DataGridViewTextBoxColumn { Name = "instStatus", HeaderText = "Status", DataPropertyName = "status" });

            dt_manageinstructor.Columns.Add(new DataGridViewButtonColumn { Name = "colApprove", HeaderText = "Action", Text = "Approve", UseColumnTextForButtonValue = true });
            dt_manageinstructor.Columns.Add(new DataGridViewButtonColumn { Name = "colReject", HeaderText = "", Text = "Reject", UseColumnTextForButtonValue = true });

        }

        private void SetupCourseGrid()
        {

            dt_course.Columns.Clear();
            dt_course.AutoGenerateColumns = false;
            dt_course.ReadOnly = true;
            dt_course.AllowUserToAddRows = false;
            dt_course.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_course.Columns.Add(new DataGridViewTextBoxColumn { Name = "courseId", HeaderText = "ID", DataPropertyName = "course_id" });
            dt_course.Columns.Add(new DataGridViewTextBoxColumn { Name = "courseCode", HeaderText = "Course Code", DataPropertyName = "course_code" });
            dt_course.Columns.Add(new DataGridViewTextBoxColumn { Name = "courseName", HeaderText = "Course Name", DataPropertyName = "course_name" });
            dt_course.Columns.Add(new DataGridViewTextBoxColumn { Name = "courseDesc", HeaderText = "Description", DataPropertyName = "description" });
            dt_course.Columns.Add(new DataGridViewTextBoxColumn { Name = "courseStatus", HeaderText = "Active", DataPropertyName = "is_active" });
        }
        private void SetupSubjectGrid()
        {

            dg_subjects.Columns.Clear();
            dg_subjects.AutoGenerateColumns = false;
            dg_subjects.ReadOnly = true;
            dg_subjects.AllowUserToAddRows = false;
            dg_subjects.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dg_subjects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dg_subjects.Columns.Add(new DataGridViewTextBoxColumn { Name = "subId", HeaderText = "ID", DataPropertyName = "subject_id" });
            dg_subjects.Columns.Add(new DataGridViewTextBoxColumn { Name = "subCode", HeaderText = "Subject Code", DataPropertyName = "subject_code" });
            dg_subjects.Columns.Add(new DataGridViewTextBoxColumn { Name = "subName", HeaderText = "Subject Name", DataPropertyName = "subject_name" });
            dg_subjects.Columns.Add(new DataGridViewTextBoxColumn { Name = "subUnits", HeaderText = "Units", DataPropertyName = "units" });
            dg_subjects.Columns.Add(new DataGridViewTextBoxColumn { Name = "subDesc", HeaderText = "Description", DataPropertyName = "description" });
            dg_subjects.Columns.Add(new DataGridViewTextBoxColumn { Name = "subActive", HeaderText = "Active", DataPropertyName = "is_active" });
        }
        private void InitUnitsCombo()
        {
            cb_units.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_units.Items.Clear();
            cb_units.Items.AddRange(new object[] { "1.0", "2.0", "3.0", "4.0", "5.0", "6.0" });
            cb_units.SelectedItem = "3.0";
        }
        public void LoadData()
        {

            LoadUsers(tb_searchuser.Text?.Trim());
            LoadInstructors(cb_pendingonly.Checked, tb_search.Text?.Trim());
            LoadCourses();
            LoadSubjects();

        }
        private void LoadUsers(string keyword = null) => dataGridView1.DataSource = _users.GetUsers(keyword);
        private void LoadInstructors(bool pendingOnly = false, string keyword = null)
            => dt_manageinstructor.DataSource = _users.GetUsers(keyword, roleFilter: "Instructor", statusFilter: pendingOnly ? "Pending" : null);
        private void LoadCourses() => dt_course.DataSource = _courses.GetAll();
        private void LoadSubjects() => dg_subjects.DataSource = _subjects.GetAll();
    private void dt_manageinstructor_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;
            var id = Convert.ToInt32(dt_manageinstructor.Rows[e.RowIndex].Cells["instId"].Value);
            var col = dt_manageinstructor.Columns[e.ColumnIndex].Name;

            if (col == "colApprove")
            {
                if (_users.ApproveInstructor(id, out var msg)) { MessageBox.Show(msg); LoadInstructors(cb_pendingonly.Checked, tb_search.Text?.Trim()); LoadUsers(tb_searchuser.Text?.Trim()); }
                else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (col == "colReject")
            {
                if (MessageBox.Show("Reject this instructor? This deletes the account.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                if (_users.RejectInstructor(id, out var msg)) { MessageBox.Show(msg); LoadInstructors(cb_pendingonly.Checked, tb_search.Text?.Trim()); LoadUsers(tb_searchuser.Text?.Trim()); }
                else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dt_course_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;
            var row = dt_course.Rows[e.RowIndex];
            tb_coursecode.Text = row.Cells["courseCode"].Value?.ToString() ?? "";
            tb_coursename.Text = row.Cells["courseName"].Value?.ToString() ?? "";
            tb_description.Text = row.Cells["courseDesc"].Value?.ToString() ?? "";
        }
        private void ClearCourseFields() { tb_coursecode.Clear(); tb_coursename.Clear(); tb_description.Clear(); dt_course.ClearSelection(); }

        private void btn_addcourse_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_coursecode.Text) || string.IsNullOrWhiteSpace(tb_coursename.Text))
            { MessageBox.Show("Course Code and Course Name cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var c = new Course { CourseCode = tb_coursecode.Text.Trim(), CourseName = tb_coursename.Text.Trim(), Description = tb_description.Text.Trim() };
            if (_courses.Add(c, out var msg)) { MessageBox.Show(msg); ClearCourseFields(); LoadCourses(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_clearcourse_Click_1(object sender, EventArgs e)
        {
            ClearCourseFields();

        }
        private void btn_updatecourse_Click_1(object sender, EventArgs e)
        {

            if (dt_course.CurrentRow?.Cells["courseId"].Value == null) { MessageBox.Show("Select a course to update."); return; }
            if (string.IsNullOrWhiteSpace(tb_coursecode.Text) || string.IsNullOrWhiteSpace(tb_coursename.Text))
            { MessageBox.Show("Course Code and Course Name cannot be empty."); return; }

            var c = new Course
            {
                CourseId = Convert.ToInt32(dt_course.CurrentRow.Cells["courseId"].Value),
                CourseCode = tb_coursecode.Text.Trim(),
                CourseName = tb_coursename.Text.Trim(),
                Description = tb_description.Text.Trim()
            };
            if (_courses.Update(c, out var msg)) { MessageBox.Show(msg); LoadCourses(); ClearCourseFields(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ClearSubjectFields() { tb_subcode.Clear(); tb_subname.Clear(); tb_subdesc.Clear(); cb_units.SelectedItem = "3.0"; dg_subjects.ClearSelection(); selectedSubjectId = -1; }

        private void btn_deletecourse_Click_1(object sender, EventArgs e)
        {

            if (dt_course.CurrentRow?.Cells["courseId"].Value == null) { MessageBox.Show("Select a course to delete."); return; }
            var id = Convert.ToInt32(dt_course.CurrentRow.Cells["courseId"].Value);
            var code = dt_course.CurrentRow.Cells["courseCode"].Value?.ToString() ?? "";
            if (MessageBox.Show($"Delete course '{code}'? This cannot be undone.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            if (_courses.Delete(id, out var msg)) { MessageBox.Show(msg); LoadCourses(); ClearCourseFields(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void dg_subjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;
            var row = dg_subjects.Rows[e.RowIndex];
            selectedSubjectId = Convert.ToInt32(row.Cells["subId"].Value ?? -1);
            tb_subcode.Text = row.Cells["subCode"].Value?.ToString() ?? "";
            tb_subname.Text = row.Cells["subName"].Value?.ToString() ?? "";
            tb_subdesc.Text = row.Cells["subDesc"].Value?.ToString() ?? "";
            var unitsStr = row.Cells["subUnits"].Value?.ToString() ?? "3.0";
            cb_units.SelectedItem = decimal.TryParse(unitsStr, out var u) ? u.ToString("0.0") : "3.0";
        }

       
        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_subcode.Text) || string.IsNullOrWhiteSpace(tb_subname.Text))
            { MessageBox.Show("Subject Code and Subject Name cannot be empty."); return; }
            if (!decimal.TryParse(cb_units.SelectedItem?.ToString(), out var units)) { MessageBox.Show("Select valid Units."); return; }

            var s = new Subject { SubjectCode = tb_subcode.Text.Trim(), SubjectName = tb_subname.Text.Trim(), Units = units, Description = tb_subdesc.Text.Trim() };
            if (_subjects.Add(s, out var msg)) { MessageBox.Show(msg); ClearSubjectFields(); LoadSubjects(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearSubjectFields();
        }
        private void btn_update_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId <= 0) { MessageBox.Show("Select a subject to update."); return; }
            if (string.IsNullOrWhiteSpace(tb_subcode.Text) || string.IsNullOrWhiteSpace(tb_subname.Text))
            { MessageBox.Show("Subject Code and Subject Name cannot be empty."); return; }
            if (!decimal.TryParse(cb_units.SelectedItem?.ToString(), out var units)) { MessageBox.Show("Select valid Units."); return; }

            var s = new Subject { SubjectId = selectedSubjectId, SubjectCode = tb_subcode.Text.Trim(), SubjectName = tb_subname.Text.Trim(), Units = units, Description = tb_subdesc.Text.Trim() };
            if (_subjects.Update(s, out var msg)) { MessageBox.Show(msg); LoadSubjects(); ClearSubjectFields(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId <= 0) { MessageBox.Show("Select a subject to delete."); return; }
            if (MessageBox.Show($"Delete subject '{tb_subcode.Text}'? This cannot be undone.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            if (_subjects.Delete(selectedSubjectId, out var msg)) { MessageBox.Show(msg); LoadSubjects(); ClearSubjectFields(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    
        private void Form3_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns["colNumber"].DataPropertyName = "id";


            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.BorderStyle = BorderStyle.None;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.DimGray;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.White;
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            panel1.BackColor = Color.DimGray;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.White;
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            pnl_maninstructor.BackColor = Color.DimGray;
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            pnl_maninstructor.BackColor = Color.DimGray;
        }

        private void panel3_MouseEnter(object sender, EventArgs e)
        {
            panel3.BackColor = Color.DimGray;
        }

        private void panel3_MouseLeave(object sender, EventArgs e)
        {
            panel3.BackColor = Color.White;
        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {
            panel4.BackColor = Color.DimGray;
        }

        private void panel4_MouseLeave(object sender, EventArgs e)
        {
            panel4.BackColor = Color.White;
        }

        private void panel5_MouseEnter(object sender, EventArgs e)
        {
            panel5.BackColor = Color.DimGray;
        }

        private void panel5_MouseLeave(object sender, EventArgs e)
        {
            panel5.BackColor = Color.White;
        }

        private void panel2_MouseLeave_1(object sender, EventArgs e)
        {
            pnl_maninstructor.BackColor = Color.White;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            frm_Home homeForm = new frm_Home();
            homeForm.Show();
            this.Hide();
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
        private void panel5_Click(object sender, EventArgs e)
        {
            pnl_account.BringToFront();
        }

        private void panel4_Click(object sender, EventArgs e)
        {
            pnl_manageEnrollment.BringToFront();
        }
        private void panel1_Click(object sender, EventArgs e)
        {
            pnl_manageCourse.BringToFront();
        }

        private void pnl_homepage_MouseEnter(object sender, EventArgs e)
        {
            pnl_homepage.BackColor = Color.DimGray;
        }

        private void pnl_homepage_MouseLeave(object sender, EventArgs e)
        {
            pnl_homepage.BackColor = Color.White;
        }
      private void LoadInstructors()
        {
            var pendingOnly = cb_pendingonly.Checked;
            var keyword = (tb_search.Text ?? "").Trim();
            dt_manageinstructor.DataSource = _users.GetUsers(keyword, roleFilter: "Instructor", statusFilter: pendingOnly ? "Pending" : null);
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
        private void SetupBillingGrid()
        {
            dt_billing.Columns.Clear();
            dt_billing.AutoGenerateColumns = false; dt_billing.ReadOnly = true; dt_billing.AllowUserToAddRows = false;
            dt_billing.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dt_billing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "billId", HeaderText = "ID", DataPropertyName = "billing_id" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "studId", HeaderText = "Student ID", DataPropertyName = "student_id" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "studName", HeaderText = "Student", DataPropertyName = "student_name" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "sy", HeaderText = "School Year", DataPropertyName = "school_year" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "term", HeaderText = "Term", DataPropertyName = "term" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "total", HeaderText = "Total Amount", DataPropertyName = "total_amount" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "Status", DataPropertyName = "status" });
            dt_billing.Columns.Add(new DataGridViewTextBoxColumn { Name = "notes", HeaderText = "Notes", DataPropertyName = "notes" });
            dt_billing.CellClick += dt_billing_CellClick;
            cb_status.DropDownStyle = ComboBoxStyle.DropDownList; cb_status.Items.Clear(); cb_status.Items.AddRange(new object[] { "Unpaid", "Partial", "Paid", "Void" }); cb_status.SelectedItem = "Unpaid";
            cb_term.DropDownStyle = ComboBoxStyle.DropDownList; cb_term.Items.Clear(); cb_term.Items.AddRange(new object[] { "1st", "2nd", "Summer" });
        }

        private void LoadBilling()
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                const string sql = @"SELECT b.billing_id AS billing_id, 
                             b.student_id AS student_id,
                             u.username AS student_name,
                             b.school_year AS school_year, 
                             b.term AS term, 
                             b.total_amount AS total_amount, 
                             b.status AS status, 
                             b.notes AS notes
                             FROM billing b
                             JOIN users u ON u.id = b.student_id
                             ORDER BY b.created_at DESC";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            dt_billing.DataSource = dt;
        }
        private void LoadStudentCombo() { var dt = _billing.GetStudentsLookup(); cb_student.DisplayMember = "username"; cb_student.ValueMember = "id"; cb_student.DataSource = dt; }
        private void ClearBillingFields() { selectedBillingId = -1; cb_student.SelectedIndex = -1; tb_schoolyear.Clear(); cb_term.SelectedIndex = -1; tb_totalamount.Text = "0.00"; cb_status.SelectedItem = "Unpaid"; tb_notes.Clear(); tb_paymentamount.Clear(); tb_paymentmethod.Clear(); dt_billing.ClearSelection(); }
        private Billing ReadBilling(int id) { decimal t; decimal.TryParse(tb_totalamount.Text, out t); return new Billing { BillingId = id, StudentId = Convert.ToInt32(cb_student.SelectedValue), SchoolYear = tb_schoolyear.Text.Trim(), Term = cb_term.SelectedItem?.ToString() ?? "", TotalAmount = t, Status = cb_status.SelectedItem?.ToString() ?? "Unpaid", Notes = tb_notes.Text?.Trim() }; }

        private void dt_billing_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; var r = dt_billing.Rows[e.RowIndex];
            selectedBillingId = Convert.ToInt32(r.Cells["billId"].Value ?? -1);
            cb_student.SelectedValue = Convert.ToInt32(r.Cells["studId"].Value ?? -1);
            tb_schoolyear.Text = Convert.ToString(r.Cells["sy"].Value) ?? "";
            cb_term.SelectedItem = Convert.ToString(r.Cells["term"].Value);
            tb_totalamount.Text = Convert.ToDecimal(r.Cells["total"].Value ?? 0).ToString("0.00");
            cb_status.SelectedItem = Convert.ToString(r.Cells["status"].Value) ?? "Unpaid";
            tb_notes.Text = Convert.ToString(r.Cells["notes"].Value) ?? "";
        }

        private void btn_addbilling_Click(object sender, EventArgs e)
        {
            if (cb_student.SelectedValue == null || string.IsNullOrWhiteSpace(tb_schoolyear.Text) || cb_term.SelectedItem == null) { MessageBox.Show("Student, School Year, and Term are required."); return; }
            string m; var ok = _billing.Add(ReadBilling(0), out m); MessageBox.Show(ok ? m : (m ?? "Failed."), ok ? "OK" : "Error", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error); if (ok) { LoadBilling(); ClearBillingFields(); }
        }

        private void btn_updatebilling_Click(object sender, EventArgs e)
        {
            if (selectedBillingId <= 0) { MessageBox.Show("Select a billing record to update."); return; }
            string m; var ok = _billing.Update(ReadBilling(selectedBillingId), out m); MessageBox.Show(ok ? m : (m ?? "Failed."), ok ? "OK" : "Error", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error); if (ok) { LoadBilling(); ClearBillingFields(); }
        }

        private void btn_deletebilling_Click(object sender, EventArgs e)
        {
            if (selectedBillingId <= 0) { MessageBox.Show("Select a billing record to delete."); return; }
            if (MessageBox.Show("Delete this billing record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            string m; var ok = _billing.Delete(selectedBillingId, out m); MessageBox.Show(ok ? m : (m ?? "Failed."), ok ? "OK" : "Error", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error); if (ok) { LoadBilling(); ClearBillingFields(); }
        }

        private void btn_recordpayment_Click(object sender, EventArgs e)
        {
            if (selectedBillingId <= 0) { MessageBox.Show("Select a billing record."); return; }
            decimal p; if (!decimal.TryParse(tb_paymentamount.Text, out p) || p <= 0) { MessageBox.Show("Enter a valid payment amount."); return; }
            string m; var ok = _billing.RecordPayment(selectedBillingId, p, tb_paymentmethod.Text?.Trim() ?? "", out m); MessageBox.Show(ok ? m : (m ?? "Failed."), ok ? "OK" : "Error", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error); if (ok) { LoadBilling(); tb_paymentamount.Clear(); tb_paymentmethod.Clear(); }
        }

        private void btn_clearbilling_Click(object sender, EventArgs e)
        {
            ClearBillingFields();
        }

        private void pnl_coursesubjects_Click(object sender, EventArgs e)
        {
            pnl_coursesub.BringToFront();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            btn_coursesubLoad.Click += btn_coursesubLoad_Click;
            btn_coursesubAdd.Click += btn_coursesubAdd_Click;
        }

        private void pnl_coursesubjects_MouseEnter(object sender, EventArgs e)
        {
            pnl_coursesubjects.BackColor = Color.DimGray;
        }

        private void pnl_coursesubjects_MouseLeave(object sender, EventArgs e)
        {
            pnl_coursesubjects.BackColor = Color.White;
        }
        private void SetupCourseSubjectGrids()
        {
            dt_availablesubject.Columns.Clear();
            dt_availablesubject.AutoGenerateColumns = false;
            dt_availablesubject.ReadOnly = true;
            dt_availablesubject.AllowUserToAddRows = false;
            dt_availablesubject.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dt_availablesubject.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_availablesubject.Columns.Add(new DataGridViewTextBoxColumn { Name = "availSubId", HeaderText = "ID", DataPropertyName = "subject_id" });
            dt_availablesubject.Columns.Add(new DataGridViewTextBoxColumn { Name = "availSubCode", HeaderText = "Code", DataPropertyName = "subject_code" });
            dt_availablesubject.Columns.Add(new DataGridViewTextBoxColumn { Name = "availSubName", HeaderText = "Subject Name", DataPropertyName = "subject_name" });
            dt_availablesubject.Columns.Add(new DataGridViewTextBoxColumn { Name = "availUnits", HeaderText = "Units", DataPropertyName = "units" });

            dt_assignedSub.Columns.Clear();
            dt_assignedSub.AutoGenerateColumns = false;
            dt_assignedSub.ReadOnly = true;
            dt_assignedSub.AllowUserToAddRows = false;
            dt_assignedSub.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dt_assignedSub.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_assignedSub.Columns.Add(new DataGridViewTextBoxColumn { Name = "csId", HeaderText = "CSID", DataPropertyName = "course_subject_id", Visible = false });
            dt_assignedSub.Columns.Add(new DataGridViewTextBoxColumn { Name = "assignSubId", HeaderText = "Subject ID", DataPropertyName = "subject_id", Visible = false });
            dt_assignedSub.Columns.Add(new DataGridViewTextBoxColumn { Name = "assignSubCode", HeaderText = "Code", DataPropertyName = "subject_code" });
            dt_assignedSub.Columns.Add(new DataGridViewTextBoxColumn { Name = "assignSubName", HeaderText = "Subject Name", DataPropertyName = "subject_name" });
            dt_assignedSub.Columns.Add(new DataGridViewTextBoxColumn { Name = "assignUnits", HeaderText = "Units", DataPropertyName = "units" });
        }

        private void LoadCourseCombo()
        {
            var dt = _courseSubject.GetCoursesLookup();
            cb_selectcourse.DisplayMember = "fullname";
            cb_selectcourse.ValueMember = "course_id";
            cb_selectcourse.DataSource = dt;
            cb_selectcourse.SelectedIndex = -1;
        }

        private void LoadCourseSubjectGrids()
        {
            if (selectedCourseId <= 0) return;
            dt_availablesubject.DataSource = _courseSubject.GetAvailableSubjects(selectedCourseId);
            dt_assignedSub.DataSource = _courseSubject.GetAssignedSubjects(selectedCourseId);
        }
        private void btn_coursesubLoad_Click(object sender, EventArgs e)
        {
            if (cb_selectcourse.SelectedValue == null) { MessageBox.Show("Select a course first."); return; }
            selectedCourseId = Convert.ToInt32(cb_selectcourse.SelectedValue);
            undoStack.Clear();
            LoadCourseSubjectGrids();
        }
        private void btn_coursesubAdd_Click(object sender, EventArgs e)
        {
            if (selectedCourseId <= 0) { MessageBox.Show("Load a course first."); return; }
            if (dt_availablesubject.CurrentRow?.Cells["availSubId"].Value == null) { MessageBox.Show("Select a subject to add."); return; }

            var subjectId = Convert.ToInt32(dt_availablesubject.CurrentRow.Cells["availSubId"].Value);
            if (_courseSubject.AssignSubject(selectedCourseId, subjectId, out var msg))
            {
                LoadCourseSubjectGrids();
  
                var dt = _courseSubject.GetAssignedSubjects(selectedCourseId);
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["subject_id"]) == subjectId)
                    {
                        undoStack.Push(Convert.ToInt32(row["course_subject_id"]));
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_coursesubRemove_Click(object sender, EventArgs e)
        {
            if (dt_assignedSub.CurrentRow?.Cells["csId"].Value == null) { MessageBox.Show("Select an assigned subject to remove."); return; }
            var csId = Convert.ToInt32(dt_assignedSub.CurrentRow.Cells["csId"].Value);
            if (_courseSubject.RemoveSubject(csId, out var msg))
            {
                LoadCourseSubjectGrids();
            }
            else
            {
                MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_coursesubAdd_Click_1(object sender, EventArgs e)
        {
            btn_coursesubLoad.Click += btn_coursesubLoad_Click;
            btn_coursesubAdd.Click += btn_coursesubAdd_Click;
        }
        private void btn_coursesubRemove_Click_1(object sender, EventArgs e)
        {
            if (dt_assignedSub.CurrentRow?.Cells["csId"].Value == null) { MessageBox.Show("Select an assigned subject to remove."); return; }
            var csId = Convert.ToInt32(dt_assignedSub.CurrentRow.Cells["csId"].Value);
            if (_courseSubject.RemoveSubject(csId, out var msg))
            {
                LoadCourseSubjectGrids();
            }
            else
            {
                MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cb_course_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_course.SelectedValue == null) return;
            int courseId = Convert.ToInt32(cb_course.SelectedValue);
            var dt = _classOffering.GetSubjectsByCourse(courseId);
            cb_subjects.DataSource = dt;
            cb_subjects.DisplayMember = "fullname";
            cb_subjects.ValueMember = "subject_id";
        }
        private void SetupClassOfferingGrid()
        {
            dt_classoffering.Columns.Clear();
            dt_classoffering.AutoGenerateColumns = false;
            dt_classoffering.ReadOnly = true;
            dt_classoffering.AllowUserToAddRows = false;
            dt_classoffering.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dt_classoffering.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "classId", HeaderText = "ID", DataPropertyName = "class_id", Visible = true });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "course_id", HeaderText = "CourseID", DataPropertyName = "course_id", Visible = false });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "subject_id", HeaderText = "SubjectID", DataPropertyName = "subject_id", Visible = false });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "instructor_id", HeaderText = "InstructorID", DataPropertyName = "instructor_id", Visible = false });

            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "courseCode", HeaderText = "Course", DataPropertyName = "course_code" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "subjectCode", HeaderText = "Subject", DataPropertyName = "subject_code" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "section", HeaderText = "Section", DataPropertyName = "section" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "sy", HeaderText = "SY", DataPropertyName = "school_year" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "term", HeaderText = "Term", DataPropertyName = "term" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "instructor", HeaderText = "Instructor", DataPropertyName = "instructor_name" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "capacity", HeaderText = "Cap", DataPropertyName = "capacity" });
            dt_classoffering.Columns.Add(new DataGridViewTextBoxColumn { Name = "active", HeaderText = "Active", DataPropertyName = "is_active" });

            dt_classoffering.CellClick += dt_classoffering_CellClick;
        }

        private void LoadClassOfferingCombos()
        {
            cb_course.DataSource = _courseSubject.GetCoursesLookup();
            cb_course.DisplayMember = "username";
            cb_course.ValueMember = "course_id";
            cb_course.SelectedIndex = -1;

            cb_terms.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_terms.Items.Clear();
            cb_terms.Items.AddRange(new object[] { "1st", "2nd", "Summer" });

            var inst = _classOffering.GetInstructors();
            cb_instructor.DataSource = inst;
            cb_instructor.DisplayMember = "fullname";
            cb_instructor.ValueMember = "id";
            cb_instructor.SelectedIndex = -1;

            numericUpDown_capacity.Value = 30;
        }

        private void LoadClassOfferings()
        {
            dt_classoffering.DataSource = _classOffering.GetAll();
        }

        private void ClearClassOfferingFields()
        {
            selectedClassId = -1;
            cb_course.SelectedIndex = -1;
            cb_subjects.DataSource = null;
            cb_terms.SelectedIndex = -1;
            cb_instructor.SelectedIndex = -1;
            tb_schoolsyear.Clear();
 
            numericUpDown_capacity.Value = 30;
            dt_classoffering.ClearSelection();
        }

        private ClassOffering ReadClassOffering(int id)
        {
            return new ClassOffering
            {
                ClassId = id,
                CourseId = Convert.ToInt32(cb_course.SelectedValue),
                SubjectId = Convert.ToInt32(cb_subjects.SelectedValue),
                InstructorId = cb_instructor.SelectedValue == null ? (int?)null : Convert.ToInt32(cb_instructor.SelectedValue),
                SchoolYear = tb_schoolsyear.Text.Trim(),
                Term = cb_terms.SelectedItem?.ToString() ?? "",
                Section = "A", 
                Capacity = (int)numericUpDown_capacity.Value
            };
        }

        private void btn_classoffcreate_Click(object sender, EventArgs e)
        {
            if (cb_course.SelectedValue == null || cb_subjects.SelectedValue == null || string.IsNullOrWhiteSpace(tb_schoolsyear.Text) || cb_terms.SelectedItem == null)
            { MessageBox.Show("Course, Subject, School Year, and Term are required."); return; }

            if (_classOffering.Add(ReadClassOffering(0), out var msg))
            { MessageBox.Show(msg); ClearClassOfferingFields(); LoadClassOfferings(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_classoffupdate_Click(object sender, EventArgs e)
        {
            if (selectedClassId <= 0) { MessageBox.Show("Select a class offering to update."); return; }
            if (_classOffering.Update(ReadClassOffering(selectedClassId), out var msg))
            { MessageBox.Show(msg); ClearClassOfferingFields(); LoadClassOfferings(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btn_classoffclear_Click(object sender, EventArgs e)
        {
            ClearClassOfferingFields();
        }

        private void btn_deleteorenable_Click(object sender, EventArgs e)
        {
            if (selectedClassId <= 0) { MessageBox.Show("Select a class offering."); return; }
            if (_classOffering.ToggleActive(selectedClassId, out var msg))
            { MessageBox.Show(msg); LoadClassOfferings(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dt_classoffering_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dt_classoffering.Rows[e.RowIndex];
            selectedClassId = Convert.ToInt32(row.Cells["classId"].Value);

            cb_course.SelectedValue = row.Cells["course_id"].Value;
            cb_subjects.SelectedValue = row.Cells["subject_id"].Value;
            cb_instructor.SelectedValue = row.Cells["instructor_id"].Value == DBNull.Value ? -1 : Convert.ToInt32(row.Cells["instructor_id"].Value);

            tb_schoolsyear.Text = row.Cells["sy"].Value?.ToString() ?? "";
            cb_terms.SelectedItem = row.Cells["term"].Value?.ToString();

            numericUpDown_capacity.Value = Convert.ToDecimal(row.Cells["capacity"].Value);
        }

        private void btn_classoffcreate_Click_1(object sender, EventArgs e)
        {
            if (cb_course.SelectedValue == null || cb_subjects.SelectedValue == null || string.IsNullOrWhiteSpace(tb_schoolsyear.Text) || cb_terms.SelectedItem == null)
            { MessageBox.Show("Course, Subject, School Year, and Term are required."); return; }

            if (_classOffering.Add(ReadClassOffering(0), out var msg))
            { MessageBox.Show(msg); ClearClassOfferingFields(); LoadClassOfferings(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_classoffupdate_Click_1(object sender, EventArgs e)
        {
            if (selectedClassId <= 0) { MessageBox.Show("Select a class offering to update."); return; }
            if (_classOffering.Update(ReadClassOffering(selectedClassId), out var msg))
            { MessageBox.Show(msg); ClearClassOfferingFields(); LoadClassOfferings(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_classoffclear_Click_1(object sender, EventArgs e)
        {
            ClearClassOfferingFields();
        }

        private void btn_deleteorenable_Click_1(object sender, EventArgs e)
        {
            if (selectedClassId <= 0) { MessageBox.Show("Select a class offering."); return; }
            if (_classOffering.ToggleActive(selectedClassId, out var msg))
            { MessageBox.Show(msg); LoadClassOfferings(); }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

