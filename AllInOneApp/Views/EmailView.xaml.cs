using AllInOneApp.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmailView : Page
    {
        private GraphServiceClient gc;
        public ObservableCollection<MailDetails> myEmails = new ObservableCollection<MailDetails>();
        public EmailView()
        {
            this.InitializeComponent();

            gc = MainPage.graphClient;

            GetMyImportantEmail();
        }

        private async void GetMyImportantEmail()
        {
            try
            {
                var result = await gc.Me.Messages.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = "importance eq 'high'";
                });

                if(result == null || result.Value.Count > 0)
                {
                    for(int i = 0; i < result.Value.Count; i++)
                    {
                        var currValue = result.Value[i];

                        myEmails.Add(new MailDetails
                        {
                            Id = currValue.Id,
                            subject= currValue.Subject,
                            body=currValue.Body.Content,
                            isRead=currValue.IsRead.Value,
                            from=currValue.From.EmailAddress.Address,
                            fromDisplayName = currValue.From.EmailAddress.Name,
                            subjectColor = currValue.IsRead.Value ? Color.Black : Color.Blue
                            //toRecipients=currValue.ToRecipients
                            //ccRecipients=currValue.CcRecipients
                        });
                    }
                    
                }
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = (ListView)sender;
            
            var view = Assembly.GetExecutingAssembly().GetType($"AllInOneApp.Views.EmailContentView");
            //if (string.IsNullOrWhiteSpace(clickedView)) return false;

            EmailContent.Navigate(view, data.SelectedItem, new EntranceNavigationTransitionInfo());
            //return true;
        }
    }
}
