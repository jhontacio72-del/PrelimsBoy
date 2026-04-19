using PrelimsBoy.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrelimsBoy.Models;
using PrelimsBoy.Services;

namespace PrelimsBoy.Services
{
    public class CourseSubjectService
    {
        public DataTable GetCoursesLookup()
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT course_id, CONCAT(course_code, ' - ', course_name) AS fullname 
                                     FROM courses WHERE is_active=1 ORDER BY course_code";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetAvailableSubjects(int courseId)
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT s.subject_id, s.subject_code, s.subject_name, s.units 
                                     FROM subjects s 
                                     WHERE s.is_active=1 
                                     AND s.subject_id NOT IN (SELECT subject_id FROM course_subjects WHERE course_id=@cid)
                                     ORDER BY s.subject_code";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@cid", courseId);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetAssignedSubjects(int courseId)
        {
            DataTable dt = new DataTable();
            using (var conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT cs.course_subject_id, s.subject_id, s.subject_code, s.subject_name, s.units 
                             FROM course_subjects cs 
                             JOIN subjects s ON s.subject_id = cs.subject_id 
                             WHERE cs.course_id=@cid 
                             ORDER BY s.subject_code";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@cid", courseId);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool AssignSubject(int courseId, int subjectId, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    const string sql = @"INSERT INTO course_subjects (course_id, subject_id) VALUES (@cid, @sid)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", courseId);
                        cmd.Parameters.AddWithValue("@sid", subjectId);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Subject assigned.";
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062) { msg = "Subject already assigned to this course."; return false; }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool RemoveSubject(int courseSubjectId, out string msg)
        {
            msg = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    const string sql = @"DELETE FROM course_subjects WHERE course_subject_id=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", courseSubjectId);
                        var rows = cmd.ExecuteNonQuery();
                        msg = rows > 0 ? "Subject removed." : "Record not found.";
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }
    }
}
