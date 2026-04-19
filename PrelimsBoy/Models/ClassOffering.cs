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
    public class ClassOffering
    {

        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int? InstructorId { get; set; }
        public string SchoolYear { get; set; } = "";
        public string Term { get; set; } = "";
        public string Section { get; set; } = "";
        public int Capacity { get; set; } = 30;
        public bool IsActive { get; set; } = true;

        public string CourseCode { get; set; } = "";
        public string SubjectCode { get; set; } = "";
        public string InstructorName { get; set; } = "";
    }
}
