using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PrelimsBoy.Services;

namespace PrelimsBoy.Services
{
    public class BillingService
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"
                    SELECT b.billing_id, b.student_id,
                           CONCAT(u.lastname, ', ', u.firstname) AS student_name,
                           b.school_year, b.term, b.total_amount, b.status, b.notes
                    FROM billing b
                    JOIN users u ON u.id = b.student_id
                    WHERE u.role = 'Student'
                    ORDER BY b.billing_id DESC";
                using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetStudentsLookup()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = Database.GetConnection())
            {
                if (conn == null) return dt;
                const string sql = @"SELECT id, username 
                     FROM users 
                     WHERE role = 'Student' AND status = 'Active'
                     ORDER BY username";
                using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool Add(Billing b, out string msg)
        {
            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    if (conn == null) { msg = "No DB connection."; return false; }
                    const string sql = @"
                        INSERT INTO billing (student_id, school_year, term, total_amount, status, notes)
                        VALUES (@sid, @sy, @term, @total, @status, @notes)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", b.StudentId);
                        cmd.Parameters.AddWithValue("@sy", b.SchoolYear);
                        cmd.Parameters.AddWithValue("@term", b.Term);
                        cmd.Parameters.AddWithValue("@total", b.TotalAmount);
                        cmd.Parameters.AddWithValue("@status", b.Status);
                        cmd.Parameters.AddWithValue("@notes", (object)b.Notes ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Billing added.";
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) { msg = "Duplicate record."; return false; }
                msg = ex.Message; return false;
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool Update(Billing b, out string msg)
        {
            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    if (conn == null) { msg = "No DB connection."; return false; }
                    const string sql = @"
                        UPDATE billing
                        SET student_id=@sid, school_year=@sy, term=@term,
                            total_amount=@total, status=@status, notes=@notes
                        WHERE billing_id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", b.BillingId);
                        cmd.Parameters.AddWithValue("@sid", b.StudentId);
                        cmd.Parameters.AddWithValue("@sy", b.SchoolYear);
                        cmd.Parameters.AddWithValue("@term", b.Term);
                        cmd.Parameters.AddWithValue("@total", b.TotalAmount);
                        cmd.Parameters.AddWithValue("@status", b.Status);
                        cmd.Parameters.AddWithValue("@notes", (object)b.Notes ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Billing updated.";
                return true;
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool Delete(int billingId, out string msg)
        {
            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    if (conn == null) { msg = "No DB connection."; return false; }
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM billing WHERE billing_id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", billingId);
                        cmd.ExecuteNonQuery();
                    }
                }
                msg = "Billing deleted.";
                return true;
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }

        public bool RecordPayment(int billingId, decimal amount, string method, out string msg)
        {
            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    if (conn == null) { msg = "No DB connection."; return false; }
                    using (MySqlTransaction tx = conn.BeginTransaction())
                    {
                        using (MySqlCommand ins = new MySqlCommand(@"
                            INSERT INTO billing_payments (billing_id, amount, method, paid_at)
                            VALUES (@bid, @amt, @m, NOW())", conn, tx))
                        {
                            ins.Parameters.AddWithValue("@bid", billingId);
                            ins.Parameters.AddWithValue("@amt", amount);
                            ins.Parameters.AddWithValue("@m", (object)method ?? DBNull.Value);
                            ins.ExecuteNonQuery();
                        }

                        using (MySqlCommand upd = new MySqlCommand(@"
                            UPDATE billing b
                            SET status = CASE
                                WHEN (SELECT IFNULL(SUM(amount),0) FROM billing_payments p WHERE p.billing_id=b.billing_id) >= b.total_amount THEN 'Paid'
                                WHEN (SELECT IFNULL(SUM(amount),0) FROM billing_payments p WHERE p.billing_id=b.billing_id) > 0 THEN 'Partial'
                                ELSE 'Unpaid' END
                            WHERE b.billing_id=@id", conn, tx))
                        {
                            upd.Parameters.AddWithValue("@id", billingId);
                            upd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }
                msg = "Payment recorded.";
                return true;
            }
            catch (Exception ex) { msg = ex.Message; return false; }
        }
    }
}
