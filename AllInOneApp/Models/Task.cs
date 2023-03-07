using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AllInOneApp.Models
{
    public class Task
    {
        public string Title { get; set; }  
        public string Id { get; set; }
        public string DueDateTime { get; set; }
        public Microsoft.Graph.Models.Importance? Importance { get; set; }
        public Symbol TaskPriority{ get; set; }
    }

    //public enum TaskStatus
    //{
    //    NotStarted,
    //    InProgress,
    //    Completed,
    //    WaitingOnOthers,
    //    Deferred,
    //}
}
