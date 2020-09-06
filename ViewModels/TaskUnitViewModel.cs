using System;
using System.Collections.Generic;
using System.Text;

namespace TaskWpf.ViewModels
{
    class TaskUnitViewModel
    {
        public int? TaskUnitId { get; set; }
        public string TaskName { get; set; }
        public string Steps { get; set; }
        public string UserMemoText { get; set; }
        public int? UserId { get; set; }
        public bool IsComplete { get; set; }
        public bool IsImportant { get; set; }
    }
}
