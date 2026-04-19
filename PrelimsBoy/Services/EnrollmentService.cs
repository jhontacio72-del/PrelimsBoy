using MySql.Data.MySqlClient;
using PrelimsBoy.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrelimsBoy.Models;
using PrelimsBoy.Services;

namespace PrelimsBoy.Services
{
    public class EnrollmentService
    {
        public int GetStudentCourseId(int studentId)
        {
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return -1;
                const string sql = "SELECT course_id FROM users WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", studentId);
                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? -1 : Convert.ToInt32(result);
                }
            }
        }

        // Left grid: Classes available to enroll - shows ALL classes for the term/SY
        public DataTable GetAvailableClasses(int studentId, string schoolYear, string term)
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT co.class_id, co.course_id, co.subject_id, c.course_code, 
                                     s.subject_code, s.subject_name, s.units,
                                     CONCAT(u.lastname, ', ', u.firstname) AS instructor_name,
                                     co.section, co.capacity,
                                     (SELECT COUNT(*) FROM enrollments e WHERE e.class_id = co.class_id) AS enrolled
                                     FROM class_offerings co
                                     JOIN courses c ON c.course_id = co.course_id
                                     JOIN subjects s ON s.subject_id = co.subject_id
                                     LEFT JOIN users u ON u.id = co.instructor_id
                                     WHERE co.is_active=1 
                                     AND co.school_year=@sy AND co.term=@term
                                     AND NOT EXISTS (SELECT 1 FROM enrollments e WHERE e.student_id=@sid AND e.class_id=co.class_id)
                                     ORDER BY c.course_code, s.subject_code";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@sid", studentId);
                    da.SelectCommand.Parameters.AddWithValue("@sy", schoolYear);
                    da.SelectCommand.Parameters.AddWithValue("@term", term);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // Right grid: Classes student is already enrolled in
        public DataTable GetMyClasses(int studentId, string schoolYear, string term)
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT e.enrollment_id, co.class_id, s.subject_code, s.subject_name, s.units,
                                     CONCAT(u.lastname, ', ', u.firstname) AS instructor_name,
                                     co.section, e.enrolled_at
                                     FROM enrollments e
                                     JOIN class_offerings co ON co.class_id = e.class_id
                                     JOIN subjects s ON s.subject_id = co.subject_id
                                     LEFT JOIN users u ON u.id = co.instructor_id
                                     WHERE e.student_id=@sid AND co.school_year=@sy AND co.term=@term
                                     ORDER BY e.enrolled_at DESC";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@sid", studentId);
                    da.SelectCommand.Parameters.AddWithValue("@sy", schoolYear);
                    da.SelectCommand.Parameters.AddWithValue("@term", term);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool Enroll(int studentId, int classId, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    // Check capacity first
                    const string checkSql = @"SELECT co.capacity, (SELECT COUNT(*) FROM enrollments WHERE class_id=@cid) AS enrolled
                                              FROM class_offerings co WHERE co.class_id=@cid";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@cid", classId);
                        using (var rdr = checkCmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                int capacity = Convert.ToInt32(rdr["capacity"]);
                                int enrolled = Convert.ToInt32(rdr["enrolled"]);
                                if (enrolled >= capacity) { msg = "Class is full."; return false; }
                            }
                        }
                    }

                    const string sql = "INSERT INTO enrollments (student_id, class_id) VALUES (@sid, @cid)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", studentId);
                        cmd.Parameters.AddWithValue("@cid", classId);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Enrolled successfully.";
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062) { msg = "Already enrolled in this class."; return false; }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool Drop(int enrollmentId, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    const string sql = "DELETE FROM enrollments WHERE enrollment_id=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", enrollmentId);
                        var rows = cmd.ExecuteNonQuery();
                        msg = rows > 0 ? "Class dropped." : "Enrollment not found.";
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }
    }
}
