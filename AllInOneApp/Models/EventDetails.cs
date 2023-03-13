using Microsoft.Graph.Models;
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
        public DateTimeTimeZone Starttime { get; set; }
        public DateTimeTimeZone Endtime { get; set; }
        public string Attendees { get; set; }
        public string Organizer { get; set; }
    }

    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Attendee
    {
        public EmailAddress EmailAddress { get; set; }
        public Microsoft.Graph.Models.AttendeeType Type { get; set; }
    }
}
