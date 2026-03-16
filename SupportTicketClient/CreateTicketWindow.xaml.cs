using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace SupportTicketClient;

public partial class CreateTicketWindow : Window
{
    // DTO matching the API's expected payload
    private record CreateTicketRequest(string Subject, string Description, string Priority);

    public CreateTicketWindow()
    {
        InitializeComponent();
    }

    private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        // 1. Basic UI Validation 
        if (string.IsNullOrWhiteSpace(txtSubject.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
        {
            new MessageDialog("Please fill in all required fields.", "Validation Error").ShowDialog();
            return;
        }

        btnSubmit.IsEnabled = false;
        var priority = ((ComboBoxItem)cmbPriority.SelectedItem).Content.ToString()!;

        var request = new CreateTicketRequest(txtSubject.Text, txtDescription.Text, priority);

        try
        {
            // 2. Send POST request to API
            var response = await Session.ApiClient.PostAsJsonAsync("api/tickets", request);

            if (response.IsSuccessStatusCode)
            {
                new MessageDialog("Ticket submitted successfully!", "Success").ShowDialog();
                this.DialogResult = true; 
                this.Close();
            }
            else
            {
                var errorJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                var errorMessage = errorJson.GetProperty("message").GetString();

                new MessageDialog(errorMessage, "API Error").ShowDialog();
            }
            //else
            //{
            //    var error = await response.Content.ReadAsStringAsync();
            //    new MessageDialog($"Failed to create ticket. Server responded: {error}", "API Error").ShowDialog();
            //    btnSubmit.IsEnabled = true;
            //}
        }
        catch (HttpRequestException ex)
        {
            new MessageDialog($"Connection error: {ex.Message}", "Network Error").ShowDialog();
            btnSubmit.IsEnabled = true;
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }

    private void DragWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton == System.Windows.Input.MouseButton.Left) this.DragMove();
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
