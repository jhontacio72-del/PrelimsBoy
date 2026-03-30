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
        public frm_Dashboard()
        {
            InitializeComponent();
            
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

        private void pnl_history_MouseEnter(object sender, EventArgs e)
        {
            pnl_history.BackColor = Color.DarkGray;
        }

        private void pnl_history_MouseLeave(object sender, EventArgs e)
        {
            pnl_history.BackColor = Color.LightGray;
        }

        private void pnl_location_MouseEnter(object sender, EventArgs e)
        {
            pnl_location.BackColor = Color.DarkGray;
        }

        private void pnl_location_MouseLeave(object sender, EventArgs e)
        {
            pnl_location.BackColor = Color.LightGray;
        }

        private void pnl_awards_MouseEnter(object sender, EventArgs e)
        {
            pnl_awards.BackColor = Color.DarkGray;
        }

        private void pnl_awards_MouseLeave(object sender, EventArgs e)
        {
            pnl_awards.BackColor = Color.LightGray;
        }

        private void pnl_left_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > pnl_left.Width - 10) // only allow dragging from the right edge
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
    }

    }
