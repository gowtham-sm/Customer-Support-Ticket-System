using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace SupportTicketClient;

public partial class MainWindow : Window
{
    // DTOs to map the JSON payload
    private record LoginRequest(string Username, string Password);
    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        txtError.Visibility = Visibility.Collapsed;
        btnLogin.IsEnabled = false;

        var request = new LoginRequest(txtUsername.Text, txtPassword.Password);

        try
        {
            // Call the Web API
            var response = await Session.ApiClient.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    // Store credentials in our global session
                    Session.Login(result.Token, result.Role);

                    new MessageDialog($"Logged in successfully as {Session.CurrentRole}", "Success").ShowDialog();
                    // Close the login window and open the dashboard
                    var dashboard = new DashboardWindow();
                    dashboard.Show();
                    this.Close();
                }
            }
            else
            {
                txtError.Text = "Invalid username or password.";
                txtError.Visibility = Visibility.Visible;
            }
        }
        catch (HttpRequestException)
        {
            txtError.Text = "Could not connect to the API. Is it running?";
            txtError.Visibility = Visibility.Visible;
        }
        finally
        {
            btnLogin.IsEnabled = true;
        }


    }

    private void BtnTheme_Click(object sender, RoutedEventArgs e)
    {
        //ThemeManager.ToggleTheme();
        //btnTheme.Content = ThemeManager.IsDarkTheme ? "☀️ Light" : "🌙 Dark";
    }

    private void DragWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton == System.Windows.Input.MouseButton.Left) this.DragMove();
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}