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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions;
using System.Threading;
using AllInOneApp.Views;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AllInOneApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Below are the clientId (Application Id) of your app registration and the tenant information.
        // You have to replace:
        // - the content of ClientID with the Application Id for your app registration
        private const string ClientId = "d6c9fb35-bfd3-4a4b-a7a8-3f565a6c9839";

        //Set the scope for API call to user.read
        private string[] scopes = new string[] { "user.read", "Tasks.ReadWrite", "Calendars.ReadWrite", "Mail.ReadWrite" };
        public static GraphServiceClient graphClient;
        public static User user;
        public static BitmapImage userPicture = new BitmapImage();
        public static bool isPictureExist = false;
        private const string Tenant = "common"; // Alternatively "[Enter your tenant, as obtained from the Azure portal, e.g. kko365.onmicrosoft.com]"
        private const string Authority = "https://login.microsoftonline.com/" + Tenant;

        // The MSAL Public client app
        private static IPublicClientApplication PublicClientApp;

        private static string MSGraphURL = "https://graph.microsoft.com/v1.0/";
        private static AuthenticationResult authResult;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Call AcquireTokenAsync - to acquire a token requiring user to sign in
        /// </summary>
        private async void Authenticate(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingIndicator.IsActive = true;
                // Sign in user using MSAL and obtain an access token for Microsoft Graph
                graphClient = await SignInAndInitializeGraphServiceClient(scopes);

                // Call the /me endpoint of Graph
                user = await graphClient.Me.GetAsync();

                //Get user profile image.
                try
                {
                    var requPicture = await graphClient.Me.Photo.Content.GetAsync();

                    using (var memStream = new MemoryStream())
                    {
                        isPictureExist = true;
                        await requPicture.CopyToAsync(memStream);
                        memStream.Position = 0;

                        userPicture.SetSource(memStream.AsRandomAccessStream());
                    }
                }
                catch(Exception ex)
                {
                    isPictureExist = false;
                    Console.WriteLine(ex.ToString());
                }


                // Go back to the UI thread to make changes to the UI
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DisplayBasicTokenInfo(authResult);
                });

                LoadingIndicator.IsActive = false;
                this.Frame.Navigate(typeof(Navigation));
            }
            catch (MsalException msalEx)
            {
                await DisplayMessageAsync($"Error Acquiring Token:{System.Environment.NewLine}{msalEx}");
            }
            catch (Exception ex)
            {
                await DisplayMessageAsync($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
                return;
            }
        }
        /// <summary>
        /// Display basic information contained in the token. Needs to be called from the UI thread.
        /// </summary>
        private void DisplayBasicTokenInfo(AuthenticationResult authResult)
        {
            //TokenInfoText.Text = "";
            //if (authResult != null)
            //{
            //    TokenInfoText.Text += $"User Name: {authResult.Account.Username}" + Environment.NewLine;
            //    TokenInfoText.Text += $"Token Expires: {authResult.ExpiresOn.ToLocalTime()}" + Environment.NewLine;
            //}
        }
        /// <summary>
        /// Displays a message in the ResultText. Can be called from any thread.
        /// </summary>
        private async Task DisplayMessageAsync(string message)
        {
            ResultText.Visibility = Visibility.Visible;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    ResultText.Text = message;
                });
        }
        /// <summary>
        /// Signs in the user and obtains an access token for Microsoft Graph
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns> Access Token</returns>
        private static async Task<string> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            // Initialize the MSAL library by building a public client application
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority(Authority)
                .WithUseCorporateNetwork(false)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                 .WithLogging((level, message, containsPii) =>
                 {
                     Debug.WriteLine($"MSAL: {level} {message} ");
                 }, LogLevel.Warning, enablePiiLogging: false, enableDefaultPlatformLogging: true)
                .Build();

            // It's good practice to not do work on the UI thread, so use ConfigureAwait(false) whenever possible.
            IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync().ConfigureAwait(false);
            IAccount firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                                                  .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                authResult = await PublicClientApp.AcquireTokenInteractive(scopes)
                                                  .ExecuteAsync()
                                                  .ConfigureAwait(false);

            }
            return authResult.AccessToken;
        }

        /// <summary>
        /// Sign in user using MSAL and obtain a token for Microsoft Graph
        /// </summary>
        /// <returns>GraphServiceClient</returns>
        private async static Task<GraphServiceClient> SignInAndInitializeGraphServiceClient(string[] scopes)
        {
            var tokenProvider = new Helper.TokenProvider(SignInUserAndGetTokenUsingMSAL, scopes);
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
            var graphClient = new GraphServiceClient(authProvider, MSGraphURL);

            return await Task.FromResult(graphClient);
        }
        /// <summary>
        /// Sign out the current user
        /// </summary>
        public async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync().ConfigureAwait(false);
            IAccount firstAccount = accounts.FirstOrDefault();

            try
            {
                await PublicClientApp.RemoveAsync(firstAccount).ConfigureAwait(false);
                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                //{
                //ResultText.Text = "User has signed out";
                //this.CallGraphButton.Visibility = Visibility.Visible;
                //this.SignOutButton.Visibility = Visibility.Collapsed;
                //});
                //this.Frame.Navigate(typeof(MainPage));
            }
            catch (MsalException ex)
            {
                //ResultText.Text = $"Error signing out user: {ex.Message}";
            }
        }
    }
}
