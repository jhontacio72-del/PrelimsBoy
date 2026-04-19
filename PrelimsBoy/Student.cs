using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using PrelimsBoy.Services;


namespace PrelimsBoy
{
    public partial class frm_Dashboard : Form
    {
        private bool isDragging = false;
        private int initialWidth;
        private int offsetX;
        private readonly EnrollmentService _enrollment = new EnrollmentService();
        public frm_Dashboard()
        {
            InitializeComponent();
            lbl_studentId.Text = "Student ID: " + SessionHelper.CurrentUserId;
            lbl_username.Text = "Username: " + SessionHelper.CurrentUsername;

            SetupEnrollmentGrids();
            LoadTerms();

        }
        private void SetupEnrollmentGrids()
        {
            // Left grid
            dt_availableclasses.Columns.Clear();
            dt_availableclasses.AutoGenerateColumns = false;
            dt_availableclasses.ReadOnly = true;
            dt_availableclasses.AllowUserToAddRows = false;
            dt_availableclasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dt_availableclasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "classId", HeaderText = "ID", DataPropertyName = "class_id", Visible = false });
            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "subjectCode", HeaderText = "Code", DataPropertyName = "subject_code", FillWeight = 20 });
            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "subjectName", HeaderText = "Subject", DataPropertyName = "subject_name", FillWeight = 40 });
            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "units", HeaderText = "Units", DataPropertyName = "units", FillWeight = 10 });
            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "instructor", HeaderText = "Instructor", DataPropertyName = "instructor_name", FillWeight = 30 });
            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "section", HeaderText = "Sec", DataPropertyName = "section", FillWeight = 10 });
            dt_availableclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "slots", HeaderText = "Slots", DataPropertyName = "enrolled", FillWeight = 10 });

            // Right grid
            dt_myclasses.Columns.Clear();
            dt_myclasses.AutoGenerateColumns = false;
            dt_myclasses.ReadOnly = true;
            dt_myclasses.AllowUserToAddRows = false;
            dt_myclasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dt_myclasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "enrollId", HeaderText = "ID", DataPropertyName = "enrollment_id", Visible = false });
            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "myClassId", HeaderText = "ClassID", DataPropertyName = "class_id", Visible = false });
            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "mySubjectCode", HeaderText = "Code", DataPropertyName = "subject_code", FillWeight = 20 });
            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "mySubjectName", HeaderText = "Subject", DataPropertyName = "subject_name", FillWeight = 40 });
            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "myUnits", HeaderText = "Units", DataPropertyName = "units", FillWeight = 10 });
            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "myInstructor", HeaderText = "Instructor", DataPropertyName = "instructor_name", FillWeight = 30 });
            dt_myclasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "mySection", HeaderText = "Sec", DataPropertyName = "section", FillWeight = 10 });
        }

        private void LoadTerms()
        {
            cb_terms.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_terms.Items.Clear();
            cb_terms.Items.AddRange(new object[] { "1st", "2nd", "Summer" });
            cb_terms.SelectedIndex = 0;
        }

        private void frm_Dashboard_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 1000; // set the interval to 1 second
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm_Home back =new frm_Home();
            back.Show();
            this.Hide();
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            pnl_subjects.BackColor = Color.DarkGray;
        }

        private void pnl_subjects_MouseLeave(object sender, EventArgs e)
        {
            pnl_subjects.BackColor = Color.LightGray;
        }

        private void pnl_grades_MouseEnter(object sender, EventArgs e)
        {
            pnl_grades.BackColor = Color.DarkGray;
        }

        private void pnl_grades_MouseLeave(object sender, EventArgs e)
        {
            pnl_grades.BackColor = Color.LightGray;
        }

        private void pnl_billing_MouseEnter(object sender, EventArgs e)
        {
            pnl_billing.BackColor = Color.DarkGray;
        }

        private void pnl_billing_MouseLeave(object sender, EventArgs e)
        {
            pnl_billing.BackColor = Color.LightGray;
        }

        private void pnl_course_MouseEnter(object sender, EventArgs e)
        {
            pnl_course.BackColor = Color.DarkGray;
        }

        private void pnl_course_MouseLeave(object sender, EventArgs e)
        {
            pnl_course.BackColor = Color.LightGray;
        }

        private void pnl_enrollments_MouseEnter(object sender, EventArgs e)
        {
            pnl_enrollments.BackColor = Color.DarkGray;

        }

        private void pnl_enrollments_MouseLeave(object sender, EventArgs e)
        {
            pnl_enrollments.BackColor = Color.LightGray;
        }

        private void pnl_schedule_MouseEnter(object sender, EventArgs e)
        {
            pnl_schedule.BackColor = Color.DarkGray;
        }

        private void pnl_schedule_MouseLeave(object sender, EventArgs e)
        {
            pnl_schedule.BackColor = Color.LightGray;
        }

     

        private void pnl_left_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > pnl_left.Width - 10) 
            {
               
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
            {
               
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
          
        }
        private void pnl_left_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void pnl_left_MouseEnter(object sender, EventArgs e)
        {
            
        }

        private void pnl_left_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void btn_loadclasses_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_schoolyear.Text) || cb_terms.SelectedItem == null)
            { MessageBox.Show("Enter School Year and select Term."); return; }

            int studentId = SessionHelper.CurrentUserId;
            string sy = tb_schoolyear.Text.Trim();
            string term = cb_terms.SelectedItem.ToString();

            dt_availableclasses.DataSource = _enrollment.GetAvailableClasses(studentId, sy, term);
            dt_myclasses.DataSource = _enrollment.GetMyClasses(studentId, sy, term);
        }

        private void btn_enroll_Click(object sender, EventArgs e)
        {
            if (dt_availableclasses.CurrentRow == null) { MessageBox.Show("Select a class to enroll."); return; }
            int classId = Convert.ToInt32(dt_availableclasses.CurrentRow.Cells["classId"].Value);

            if (_enrollment.Enroll(SessionHelper.CurrentUserId, classId, out var msg))
            {
                MessageBox.Show(msg);
                btn_loadclasses_Click(null, null); // refresh both grids
            }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_drop_Click(object sender, EventArgs e)
        {
            if (dt_myclasses.CurrentRow == null) { MessageBox.Show("Select a class to drop."); return; }
            int enrollId = Convert.ToInt32(dt_myclasses.CurrentRow.Cells["enrollId"].Value);

            if (MessageBox.Show("Drop this class?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            if (_enrollment.Drop(enrollId, out var msg))
            {
                MessageBox.Show(msg);
                btn_loadclasses_Click(null, null); // refresh both grids
            }
            else MessageBox.Show(msg ?? "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    }

 
