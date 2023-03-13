using AllInOneApp.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmailContentView : Page
    {
        public EmailContentView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var parameters = (MailDetails)e.Parameter;

            string[] strings = parameters.body.Split('\\');


            string toRecipients = string.Empty;
            foreach(PersonDetail pd in parameters.toRecipients)
            {
                toRecipients = string.IsNullOrEmpty(toRecipients) ? pd.Address : toRecipients + ";\n      " + pd.Address;
            }

            string ccRecipients = string.Empty;
            foreach (PersonDetail pd in parameters.ccRecipients)
            {
                ccRecipients = string.IsNullOrEmpty(ccRecipients) ? pd.Address : ccRecipients + "; " + pd.Address;
            }

            this.subject.Text= parameters.subject;
            this.from.Text = "From: "+parameters.from;
            this.to.Text = "To: " + toRecipients;
            this.cc.Text = string.IsNullOrEmpty(ccRecipients)? "":"cc: " + ccRecipients;
            
            //this.mailBody.Blocks.Add(paragraph);
            this.mailBody.Text = strings[0];
            
        }
    }
}
