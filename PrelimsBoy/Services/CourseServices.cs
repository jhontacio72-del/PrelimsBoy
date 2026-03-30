using MySql.Data.MySqlClient;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using PrelimsBoy.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PrelimsBoy.Services
{
   public class CourseServices
    {
        public DataTable GetAll()
        {
            using (var conn = Database.GetConnection())
            {
                var sql = @"SELECT course_id, course_code, course_name, description, is_active
                            FROM courses ORDER BY course_id DESC;";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public bool Add(Course c, out string message)
        {
            message = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    var sql = @"INSERT INTO courses (course_code, course_name, description)
                                VALUES (@code, @name, @desc);";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", c.CourseCode.Trim());
                        cmd.Parameters.AddWithValue("@name", c.CourseName.Trim());
                        cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(c.Description) ? (object)DBNull.Value : c.Description.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }
                message = "Course added successfully!";
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062) { message = "Course Code already exists."; return false; }
            catch (Exception ex) { message = ex.Message; return false; }
        }

        public bool Update(Course c, out string message)
        {
            message = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    var sql = @"UPDATE courses SET course_code=@code, course_name=@name, description=@desc
                                WHERE course_id=@id;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", c.CourseCode.Trim());
                        cmd.Parameters.AddWithValue("@name", c.CourseName.Trim());
                        cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(c.Description) ? (object)DBNull.Value : c.Description.Trim());
                        cmd.Parameters.AddWithValue("@id", c.CourseId);
                        var rows = cmd.ExecuteNonQuery();
                        message = rows > 0 ? "Course updated." : "No changes were made.";
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) { message = "Another course already uses that code."; return false; }
            catch (Exception ex) { message = ex.Message; return false; }
        }

        public bool Delete(int courseId, out string message)
        {
            message = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    var sql = @"DELETE FROM courses WHERE course_id=@id;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", courseId);
                        var rows = cmd.ExecuteNonQuery();
                        message = rows > 0 ? "Course deleted." : "Course not found.";
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451) { message = "Cannot delete: referenced by other records."; return false; }
            catch (Exception ex) { message = ex.Message; return false; }
        }
    }
}

