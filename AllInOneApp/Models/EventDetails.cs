using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllInOneApp.Models
{
    public class EventDetails
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }
        public string Attendees { get; set; }
        public string Organizer { get; set; }
    }
}
