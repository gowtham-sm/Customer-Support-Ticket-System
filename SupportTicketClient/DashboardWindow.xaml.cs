using MaterialDesignThemes.Wpf;
using System.Net.Http.Json;
using System.Windows;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace SupportTicketClient;

public partial class DashboardWindow : Window
{
    // DTO mapping for the JSON response
    public class TicketListDto
    {
        public int TicketId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? AssignedToName { get; set; }
    }

    public DashboardWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Only standard users can create tickets 
        if (Session.CurrentRole == "Admin")
        {
            btnCreateTicket.Visibility = Visibility.Collapsed;
        }

        await LoadTickets();
    }

    private async Task LoadTickets()
    {
        try
        {
            // The API handles the role-based filtering securely on the backend
            var tickets = await Session.ApiClient.GetFromJsonAsync<List<TicketListDto>>("api/tickets");
            dgTickets.ItemsSource = tickets;
        }
        catch (Exception ex)
        {
            new MessageDialog("Failed to load tickets: " + ex.Message, "Error").ShowDialog();
        }
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadTickets();
    }

    private async void BtnCreateTicket_Click(object sender, RoutedEventArgs e)
    {
        var createWindow = new CreateTicketWindow
        {
            Owner = this 
        };

        if (createWindow.ShowDialog() == true)
        {
            await LoadTickets();
        }
    }

    private async void DgTickets_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (dgTickets.SelectedItem is TicketListDto selectedTicket)
        {
            var detailsWindow = new TicketDetailsWindow(selectedTicket.TicketId)
            {
                Owner = this
            };
            detailsWindow.ShowDialog();

            await LoadTickets();
        }
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Logout(); // Clears the JWT and Role

        var loginWindow = new MainWindow();
        loginWindow.Show();
        this.Close();
    }

    private readonly PaletteHelper _paletteHelper = new PaletteHelper();

    private void BtnTheme_Click(object sender, RoutedEventArgs e)
    {
        var theme = _paletteHelper.GetTheme();

        if (theme.GetBaseTheme() == BaseTheme.Dark)
        {
            theme.SetBaseTheme(BaseTheme.Light);
        }
        else
        {
            theme.SetBaseTheme(BaseTheme.Dark);
        }

        _paletteHelper.SetTheme(theme);
    }

    private void DragWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
        {
            this.DragMove();
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
}