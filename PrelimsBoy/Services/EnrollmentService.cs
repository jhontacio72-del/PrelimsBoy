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
            const decimal RATE_PER_UNIT = 400.00m;

            using (var conn = Database.GetConnection())
            {
                if (conn == null) { msg = "DB connection failed."; return false; }
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Check capacity + get class details for billing
                        int capacity = 0, enrolled = 0, units = 0;
                        string subjectCode = "", subjectName = "", sy = "", term = "";

                        const string checkSql = @"SELECT co.capacity, co.school_year, co.term,
                                          s.units, s.subject_code, s.subject_name,
                                          (SELECT COUNT(*) FROM enrollments WHERE class_id=@cid) AS enrolled
                                          FROM class_offerings co
                                          JOIN subjects s ON s.subject_id = co.subject_id
                                          WHERE co.class_id=@cid";
                        using (var checkCmd = new MySqlCommand(checkSql, conn, trans))
                        {
                            checkCmd.Parameters.AddWithValue("@cid", classId);
                            using (var rdr = checkCmd.ExecuteReader())
                            {
                                if (!rdr.Read()) { msg = "Class not found."; trans.Rollback(); return false; }
                                capacity = Convert.ToInt32(rdr["capacity"]);
                                enrolled = Convert.ToInt32(rdr["enrolled"]);
                                units = Convert.ToInt32(rdr["units"]);
                                subjectCode = rdr["subject_code"].ToString();
                                subjectName = rdr["subject_name"].ToString();
                                sy = rdr["school_year"].ToString();
                                term = rdr["term"].ToString();
                            }
                        }

                        if (enrolled >= capacity) { msg = "Class is full."; trans.Rollback(); return false; }

                        // 2. Insert enrollment and get the new ID
                        long enrollmentId;
                        const string insertEnroll = "INSERT INTO enrollments (student_id, class_id) VALUES (@sid, @cid); SELECT LAST_INSERT_ID();";
                        using (var cmd = new MySqlCommand(insertEnroll, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@sid", studentId);
                            cmd.Parameters.AddWithValue("@cid", classId);
                            enrollmentId = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        // 3. Auto-create billing linked to enrollment_id
                        decimal amountDue = units * RATE_PER_UNIT;
                        string notes = $"{subjectCode} - {subjectName} ({units} units @ ₱{RATE_PER_UNIT}/unit)";

                        const string insertBill = @"INSERT INTO billing 
                    (enrollment_id, student_id, school_year, term, total_amount, status, notes) 
                    VALUES (@eid, @sid, @sy, @term, @amount, 'Unpaid', @notes)";
                        using (var cmd = new MySqlCommand(insertBill, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@eid", enrollmentId);
                            cmd.Parameters.AddWithValue("@sid", studentId);
                            cmd.Parameters.AddWithValue("@sy", sy);
                            cmd.Parameters.AddWithValue("@term", term);
                            cmd.Parameters.AddWithValue("@amount", amountDue);
                            cmd.Parameters.AddWithValue("@notes", notes);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        msg = $"Enrolled successfully. Billed: ₱{amountDue:N2} for {subjectCode}";
                        return true;
                    }
                    catch (MySqlException ex) when (ex.Number == 1062)
                    {
                        trans.Rollback();
                        msg = "Already enrolled in this class.";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        msg = ex.Message;
                        return false;
                    }
                }
            }
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
                        msg = rows > 0 ? "Class dropped. Billing automatically removed." : "Enrollment not found.";
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }
    }
        }
 