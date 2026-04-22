using MySql.Data.MySqlClient;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrelimsBoy.Services
{
    public class ClassOfferingService
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT co.class_id, co.course_id, co.subject_id, co.instructor_id,
                                     c.course_code, s.subject_code, s.subject_name,
                                     CONCAT(u.lastname, ', ', u.firstname) AS instructor_name,
                                     co.school_year, co.term, co.section, co.capacity, co.is_active
                                     FROM class_offerings co
                                     JOIN courses c ON c.course_id = co.course_id
                                     JOIN subjects s ON s.subject_id = co.subject_id
                                     LEFT JOIN users u ON u.id = co.instructor_id
                                     ORDER BY co.class_id DESC";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetSubjectsByCourse(int courseId)
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT s.subject_id, CONCAT(s.subject_code, ' - ', s.subject_name) AS fullname
                                     FROM course_subjects cs
                                     JOIN subjects s ON s.subject_id = cs.subject_id
                                     WHERE cs.course_id=@cid AND s.is_active=1
                                     ORDER BY s.subject_code";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@cid", courseId);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetInstructors()
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT id, username 
                     FROM users 
                     WHERE role = 'Instructor' AND status = 'Active'
                     ORDER BY username";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool Add(ClassOffering co, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    const string sql = @"INSERT INTO class_offerings (course_id, subject_id, instructor_id, school_year, term, section, capacity)
                                         VALUES (@cid, @sid, @iid, @sy, @term, @sec, @cap)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", co.CourseId);
                        cmd.Parameters.AddWithValue("@sid", co.SubjectId);
                        cmd.Parameters.AddWithValue("@iid", (object)co.InstructorId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sy", co.SchoolYear);
                        cmd.Parameters.AddWithValue("@term", co.Term);
                        cmd.Parameters.AddWithValue("@sec", co.Section);
                        cmd.Parameters.AddWithValue("@cap", co.Capacity);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Class offering created.";
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062) { msg = "This class offering already exists."; return false; }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool Update(ClassOffering co, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    const string sql = @"UPDATE class_offerings 
                                         SET course_id=@cid, subject_id=@sid, instructor_id=@iid, 
                                             school_year=@sy, term=@term, section=@sec, capacity=@cap
                                         WHERE class_id=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", co.ClassId);
                        cmd.Parameters.AddWithValue("@cid", co.CourseId);
                        cmd.Parameters.AddWithValue("@sid", co.SubjectId);
                        cmd.Parameters.AddWithValue("@iid", (object)co.InstructorId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sy", co.SchoolYear);
                        cmd.Parameters.AddWithValue("@term", co.Term);
                        cmd.Parameters.AddWithValue("@sec", co.Section);
                        cmd.Parameters.AddWithValue("@cap", co.Capacity);
                        var rows = cmd.ExecuteNonQuery();
                        msg = rows > 0 ? "Class offering updated." : "No changes made.";
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool ToggleActive(int classId, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    const string sql = @"UPDATE class_offerings SET is_active = NOT is_active WHERE class_id=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", classId);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Status toggled.";
                return true;
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }
    }
}
