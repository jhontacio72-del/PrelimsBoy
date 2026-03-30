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
   public class SuperadminServices
    {
        public DataTable GetUsers(string keyword = null, string roleFilter = null, string statusFilter = null)
        {
            using (var conn = Database.GetConnection())
            {
                var sb = new StringBuilder(@"
                    SELECT id, name, username, role, status
                    FROM users WHERE 1=1 ");
                using (var cmd = new MySqlCommand() { Connection = conn })
                {
                    if (!string.IsNullOrWhiteSpace(roleFilter))
                    { sb.Append(" AND role=@role"); cmd.Parameters.AddWithValue("@role", roleFilter); }
                    if (!string.IsNullOrWhiteSpace(statusFilter))
                    { sb.Append(" AND status=@status"); cmd.Parameters.AddWithValue("@status", statusFilter); }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    { sb.Append(" AND (name LIKE @kw OR CAST(id AS CHAR) LIKE @kw)"); cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%"); }
                    sb.Append(" ORDER BY id DESC;");
                    cmd.CommandText = sb.ToString();
                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public bool ApproveInstructor(int userId, out string message)
        {
            message = null;
            using (var conn = Database.GetConnection())
            {
                var sql = "UPDATE users SET status='Active' WHERE id=@id AND role='Instructor';";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    var rows = cmd.ExecuteNonQuery();
                    message = rows > 0 ? "Instructor approved." : "No matching instructor.";
                    return rows > 0;
                }
            }
        }

        public bool RejectInstructor(int userId, out string message)
        {
            message = null;
            using (var conn = Database.GetConnection())
            {
                var sql = "DELETE FROM users WHERE id=@id AND role='Instructor';";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    var rows = cmd.ExecuteNonQuery();
                    message = rows > 0 ? "Instructor rejected and removed." : "No matching instructor.";
                    return rows > 0;
                }
            }
        }
    }
}

