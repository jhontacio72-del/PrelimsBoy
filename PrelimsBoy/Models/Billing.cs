using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using PrelimsBoy.Services;

namespace PrelimsBoy.Models
{
    public class Billing
    {
        public int BillingId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public string SchoolYear { get; set; } = "";
        public string Term { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Unpaid";
        public string Notes { get; set; } = "";
    }

    public class BillingPayment
    {
        public int PaymentId { get; set; }
        public int BillingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "";
        public DateTime PaidAt { get; set; }
    }
}
