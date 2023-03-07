using AllInOneApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

            // parameters.Name
            // parameters.Text
            // ...

            // Create run and set text
            Run run = new Run();
            run.Text = parameters.body;


            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(run);
            this.from.Text = parameters.from;
            this.mailBody.Blocks.Add(paragraph);
            this.mailBody.html

        }
    }
}
