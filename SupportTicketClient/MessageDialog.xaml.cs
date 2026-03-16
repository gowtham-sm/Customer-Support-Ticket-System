using System.Windows;
using System.Windows.Input;

namespace SupportTicketClient
{
    public partial class MessageDialog : Window
    {
        public MessageDialog(string message, string title = "Notice")
        {
            InitializeComponent();

            txtMessage.Text = message;
            lblTitle.Text = title;
        }

        private void DragWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}