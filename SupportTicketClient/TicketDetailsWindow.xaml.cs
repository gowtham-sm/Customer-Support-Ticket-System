using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace SupportTicketClient;

public partial class TicketDetailsWindow : Window
{
    private readonly int _ticketId;

    // DTOs matching the nested API response
    public class TicketDetailsResponse
    {
        public TicketData Ticket { get; set; } = new();
        public List<CommentData> Comments { get; set; } = new();
        public List<HistoryData> History { get; set; } = new();
    }
    public class TicketData
    {
        public int TicketId { get; set; }
        public string Subject { get; set; } = "";
        public string Description { get; set; } = "";
        public string Priority { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; } 
    }
    public class CommentData
    {
        public string CommentText { get; set; } = "";
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }
        public Visibility IsInternalVisibility => IsInternal ? Visibility.Visible : Visibility.Collapsed;
    }
    public class HistoryData
    {
        public string OldStatus { get; set; } = "";
        public string NewStatus { get; set; } = "";
        public DateTime ChangedAt { get; set; }
        public int ChangedByUserId { get; set; }
    }

    public class AdminUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }

    public TicketDetailsWindow(int ticketId)
    {
        InitializeComponent();
        _ticketId = ticketId;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentRole == "Admin")
        {
            AdminPanel.Visibility = Visibility.Visible;
            chkIsInternal.Visibility = Visibility.Visible;

            await LoadAdmins(); 
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            var data = await Session.ApiClient.GetFromJsonAsync<TicketDetailsResponse>($"api/tickets/{_ticketId}");
            if (data != null)
            {
                // Bind UI Labels
                lblTicketId.Text = data.Ticket.TicketId.ToString();
                lblSubject.Text = data.Ticket.Subject;
                txtDescription.Text = data.Ticket.Description;
                lblPriority.Content = data.Ticket.Priority;
                lblStatus.Text = data.Ticket.Status;
                lblCreatedDate.Text = data.Ticket.CreatedDate.ToString("g");
                lblAssigned.Text = data.Ticket.AssignedToName ?? "Unassigned";

                // Bind Grids
                dgHistory.ItemsSource = data.History;
                lstComments.ItemsSource = data.Comments;
            }
        }
        catch (Exception ex)
        {
            new MessageDialog($"Error loading details: {ex.Message}", "Error").ShowDialog();
        }
    }

    private async void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
    {
        if (cmbStatus.SelectedItem == null) return;
        var newStatus = ((System.Windows.Controls.ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();

        var response = await Session.ApiClient.PutAsJsonAsync($"api/tickets/{_ticketId}/status", new { NewStatus = newStatus });
        if (response.IsSuccessStatusCode)
        {
            await LoadData(); 
        }
        else
        {
            // Parses the JSON object and targets the "message" key
            var errorJson = await response.Content.ReadFromJsonAsync<JsonElement>();
            var errorMessage = errorJson.GetProperty("message").GetString();

            new MessageDialog(errorMessage, "Error").ShowDialog();
        }
            //else
            //{
            //    var error = await response.Content.ReadAsStringAsync();
            //    new MessageDialog(error, "Error").ShowDialog();
            //}
    }

    private async void BtnAssign_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Ensure a user is actually selected
            if (cmbAssignAdmin.SelectedValue != null)
            {
                // Safely cast the selected Database ID
                int adminId = (int)cmbAssignAdmin.SelectedValue;

                // Send the exact property name 'AssignedToUserId' to match your API DTO
                var response = await Session.ApiClient.PutAsJsonAsync($"api/tickets/{_ticketId}/assign", new { AdminUserId = adminId });

                if (response.IsSuccessStatusCode)
                {
                    new MessageDialog("Ticket successfully assigned!", "Success").ShowDialog();
                    await LoadData(); 
                }
                else
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    new MessageDialog($"Failed to assign ticket: {errorString}", "API Error").ShowDialog();
                }
            }
            else
            {
                new MessageDialog("Please select an Admin from the dropdown to assign.", "Notice").ShowDialog();
            }
        }
        catch (Exception ex)
        {
            new MessageDialog($"A system error occurred: {ex.Message}", "System Error").ShowDialog();
        }
    }

    private async void BtnAddComment_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNewComment.Text)) return;

        var request = new
        {
            CommentText = txtNewComment.Text,
            IsInternal = chkIsInternal.IsChecked ?? false
        };

        var response = await Session.ApiClient.PostAsJsonAsync($"api/tickets/{_ticketId}/comments", request);

        if (response.IsSuccessStatusCode)
        {
            txtNewComment.Clear();
            chkIsInternal.IsChecked = false;
            await LoadData();
        }
            else
            {
                // Parses the JSON object and targets the "message" key
                var errorJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                var errorMessage = errorJson.GetProperty("message").GetString();

                new MessageDialog(errorMessage, "API Error").ShowDialog();
            }
        //else
        //{
        //    var error = await response.Content.ReadAsStringAsync();
        //    MessageBox.Show(error, "Error");
        //}

    }

    private void DragWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton == System.Windows.Input.MouseButton.Left) this.DragMove();
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async Task LoadAdmins()
    {
        try
        {
            var admins = await Session.ApiClient.GetFromJsonAsync<List<AdminUserDto>>("api/tickets/admins");
            if (admins != null)
            {
                cmbAssignAdmin.ItemsSource = admins;
            }
        }
        catch (Exception ex)
        {
            // THIS WILL POP UP AND TELL US EXACTLY WHAT IS WRONG
            new MessageDialog($"Debug Error loading admins: {ex.Message}", "Diagnostic").ShowDialog();
        }
    }

}