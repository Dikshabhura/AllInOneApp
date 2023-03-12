using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllInOneApp.Helper
{
    public class DateTimeConversion
    {
        public DateTimeConversion() { }

        public string DateTimeConverter(DateTimeOffset dateTimeOffset)
        {
            string formattedDate = string.Empty;
            try
            {
                DateTime dateTime = dateTimeOffset.DateTime;
                string date = dateTime.ToString("yyyy-MM-dd");
                string time = dateTime.ToString("hh:mm:ss");
                formattedDate = date + "T" + time;
                System.Diagnostics.Debug.WriteLine(formattedDate);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error while converting date & time. Error Message:{0}", ex.Message);
            }
            return formattedDate;
        }
    }
}
