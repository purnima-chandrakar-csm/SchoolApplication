using HomeEF;
using MasterEF;
using System.Collections.Generic;

namespace DashboardApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        public List<Student> Students { get; set; } = new List<Student>();
    }
}
