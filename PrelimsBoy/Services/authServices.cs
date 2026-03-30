using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using PrelimsBoy.Services;
using MySql.Data.MySqlClient;
namespace PrelimsBoy.Services
{
    public class authServices
    {
        public User Login(string username, string password, out string message)
        {
            message = null;
            if (username == "admin" && password == "admin123")
                return SeedSession(new User { Id = -1, Name = "Administrator", Username = "admin", Role = "Admin", Status = "Active" });
            if (username == "superadmin" && password == "admin1234")
                return SeedSession(new User { Id = -2, Name = "Super Administrator", Username = "superadmin", Role = "SuperAdmin", Status = "Active" });

            using (var conn = Database.GetConnection())
            {
                if (conn == null) { message = "Database connection failed."; return null; }

                const string sql = @"SELECT id, name, username, role, status
                                     FROM users WHERE username=@u LIMIT 1;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) { message = "Invalid username or password."; return null; }

                        var u = new User
                        {
                            Id = Convert.ToInt32(r["id"]),
                            Name = Convert.ToString(r["name"]),
                            Username = Convert.ToString(r["username"]),
                            Role = Convert.ToString(r["role"]),
                            Status = Convert.ToString(r["status"])
                        };
                        if (!u.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        {
                            if (u.Role.Equals("Instructor", StringComparison.OrdinalIgnoreCase) &&
                                u.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                            {
                                message = "Login failed. Wait for approval before trying again.";
                                return null;
                            }

                            message = "Your account is not active.";
                            return null;
                        }

                        return SeedSession(u);
                    }
                }
            }
        }
        public bool Register(string fullName, string username, string role, out string message)
        {
            message = null;
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role))
            {
                message = "Please complete all fields.";
                return false;
            }

            var status = role.Equals("Instructor", StringComparison.OrdinalIgnoreCase) ? "Pending" : "Active";
            try
            {
                using (var conn = Database.GetConnection())
                {
                    if (conn == null) { message = "DB connection failed."; return false; }
                    const string sql = @"INSERT INTO users (name, username, role, status)
                                         VALUES (@name, @user, @role, @status);";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", fullName.Trim());
                        cmd.Parameters.AddWithValue("@user", username.Trim());
                        cmd.Parameters.AddWithValue("@role", role.Trim());
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                }
                message = role == "Instructor"
                    ? "Registered successfully! Please wait for admin approval."
                    : "Student registered successfully!";
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                message = "Username already exists.";
                return false;
            }
            catch (Exception ex)
            {
                message = "Error: " + ex.Message;
                return false;
            }
        }

        // Helper used by Login()
        private static User SeedSession(User u)
        {
            SessionHelper.CurrentUserId = u.Id;
            SessionHelper.CurrentUsername = u.Username;
            SessionHelper.CurrentName = u.Name;
            SessionHelper.CurrentRole = u.Role;
            return u;
        }
    }

}
