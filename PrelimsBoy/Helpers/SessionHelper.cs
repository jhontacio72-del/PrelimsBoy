using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrelimsBoy.Helpers;
using PrelimsBoy.Models;
using PrelimsBoy.Services;
namespace PrelimsBoy.Helpers
{
    public static class SessionHelper
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUsername { get; set; }
        public static string CurrentName { get; set; }
        public static string CurrentRole { get; set; } // "Student","Instructor","Admin","SuperAdmin"

        public static void SignOut()
        {
            CurrentUserId = 0;
            CurrentUsername = null;
            CurrentName = null;
            CurrentRole = null;
        }

        public static bool IsInRole(string role) =>
            !string.IsNullOrEmpty(CurrentRole) &&
            CurrentRole.Equals(role, System.StringComparison.OrdinalIgnoreCase);
    }
}

