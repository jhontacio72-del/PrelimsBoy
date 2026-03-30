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
   public class SubjectServices
    {
        public DataTable GetAll()
        {
            using (var conn = Database.GetConnection())
            {
                var sql = @"SELECT subject_id, subject_code, subject_name, units, description, is_active
                            FROM subjects ORDER BY subject_id DESC;";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public bool Add(Subject s, out string message)
        {
            message = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    var sql = @"INSERT INTO subjects (subject_code, subject_name, units, description)
                                VALUES (@code, @name, @units, @desc);";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", s.SubjectCode.Trim());
                        cmd.Parameters.AddWithValue("@name", s.SubjectName.Trim());
                        cmd.Parameters.AddWithValue("@units", s.Units);
                        cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(s.Description) ? (object)DBNull.Value : s.Description.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }
                message = "Subject added successfully!";
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062) { message = "Subject Code already exists."; return false; }
            catch (Exception ex) { message = ex.Message; return false; }
        }

        public bool Update(Subject s, out string message)
        {
            message = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    var sql = @"UPDATE subjects SET subject_code=@code, subject_name=@name, units=@units, description=@desc
                                WHERE subject_id=@id;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", s.SubjectCode.Trim());
                        cmd.Parameters.AddWithValue("@name", s.SubjectName.Trim());
                        cmd.Parameters.AddWithValue("@units", s.Units);
                        cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(s.Description) ? (object)DBNull.Value : s.Description.Trim());
                        cmd.Parameters.AddWithValue("@id", s.SubjectId);
                        var rows = cmd.ExecuteNonQuery();
                        message = rows > 0 ? "Subject updated." : "No changes were made.";
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) { message = "Another subject already uses that code."; return false; }
            catch (Exception ex) { message = ex.Message; return false; }
        }

        public bool Delete(int subjectId, out string message)
        {
            message = null;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    var sql = @"DELETE FROM subjects WHERE subject_id=@id;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", subjectId);
                        var rows = cmd.ExecuteNonQuery();
                        message = rows > 0 ? "Subject deleted." : "Subject not found.";
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451) { message = "Cannot delete: referenced by other records."; return false; }
            catch (Exception ex) { message = ex.Message; return false; }
        }
    }
}

