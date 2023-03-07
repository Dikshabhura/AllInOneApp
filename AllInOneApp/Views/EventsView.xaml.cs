using AllInOneApp.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventsView : Page
    {
        private GraphServiceClient gc;
        public ObservableCollection<EventDetails> myEvents = new ObservableCollection<EventDetails>();
        public EventsView()
        {
            this.InitializeComponent();

            // Get the Graph client from MainPage;
            gc = MainPage.graphClient;

            GetMyEvents();

        }

        private async void GetMyEvents()
        {
            try
            {
                var result = await gc.Me.Events.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "subject", "body", "bodyPreview", "organizer", "attendees", "start", "end", "location" };
                });
                if(result != null || result.Value.Count > 0)
                {
                    for(int i = 0; i < result.Value.Count; i++)
                    {
                        var currValue = result.Value[i];
                        myEvents.Add(new EventDetails
                        {
                            Id = currValue.Id,
                            Subject = currValue.Subject,
                            Starttime = currValue.Start.DateTime,
                            Endtime = currValue.End.DateTime,
                            //Attendees = currValue.Attendees.ToList(),
                            Organizer = "By: "+currValue.Organizer.EmailAddress.Name,

                        });
                    }
                    
                }
                
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
