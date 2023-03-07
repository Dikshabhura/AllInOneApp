using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllInOneApp.Models
{
    public class MailDetails
    {
        public string Id { get; set; }
        public string Importance { get; set; }
        public string subject { get; set; }
        public Color subjectColor { get; set; }
        public bool isRead { get; set; }
        public string body { get; set; }
        public string from { get; set; }
        public string fromDisplayName { get; set; }
        public string toRecipients { get; set; }
        public string ccRecipients { get; set; }
        public string createdDateTime { get; set; }
    }
}
