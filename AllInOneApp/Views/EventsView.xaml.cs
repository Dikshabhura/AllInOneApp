using AllInOneApp.Helper;
using AllInOneApp.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
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
using Attendee = Microsoft.Graph.Models.Attendee;
using EmailAddress = Microsoft.Graph.Models.EmailAddress;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventsView : Page
    {
        DateTimeConversion dateTimeConversion = new DateTimeConversion();
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
                            Starttime = currValue.Start,
                            Endtime = currValue.End,
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

        private async void AddNewEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                var retreivedAttendees = eventAttendee;
                string[] allattendees = eventAttendee.Text.Split(';');
                List<Microsoft.Graph.Models.Attendee> attendees = new List<Microsoft.Graph.Models.Attendee>();

                for(int index=0; index< allattendees.Length; index++)
                {
                    Attendee attendee = new Attendee();
                    attendee.EmailAddress.Address = allattendees[index];
                    attendee.Type = AttendeeType.Required;
                    attendees.Add(attendee);
                }

                var requestBody = new Event
                {
                    Subject = Convert.ToString(this.eventTitle),
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = "",
                    },
                    Start = new DateTimeTimeZone
                    {
                        DateTime = dateTimeConversion.DateTimeConverter(eventStartDate.Date.Value),
                        TimeZone = "Eastern Standard Time",
                    },
                    End = new DateTimeTimeZone
                    {
                        DateTime = dateTimeConversion.DateTimeConverter(eventEndDate.Date.Value),
                        TimeZone = "Eastern Standard Time",
                    },
                    Location = new Location
                    {
                        DisplayName = "Microsoft Teams Meeting",
                    },
                    Attendees = attendees,
                    AllowNewTimeProposals = true,
                    TransactionId = Convert.ToString(new Guid()),
                };

                var result = await gc.Me.Events.PostAsync(requestBody, (requestConfiguration) =>
                {
                    requestConfiguration.Headers.Add("Prefer", "outlook.timezone=\"Pacific Standard Time\"");
                });

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
