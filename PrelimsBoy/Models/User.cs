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
   public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }     // Student, Instructor, Admin, SuperAdmin
        public string Status { get; set; }   //
    }
}
