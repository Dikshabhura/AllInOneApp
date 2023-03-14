using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AllInOneApp.Converters
{
    public class TaskStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            if (value != null)
            {
                if(value is Microsoft.Graph.Models.TaskStatus status)
                {
                    if(status == Microsoft.Graph.Models.TaskStatus.Completed)
                    {
                        return "True";
                    }
                }
            }
            return "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
