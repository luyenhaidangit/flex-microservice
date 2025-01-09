using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows.Forms;

namespace Flex.LoginWinFormApp
{
    public partial class Form1 : Form
    {
        private HubConnection _connection;

        public Form1()
        {
            InitializeComponent();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:3001/notificationHub")
                .Build();

            _connection.On<string>("ReceiveNotification", (message) =>
            {
                Invoke((Action)(() =>
                {
                    MessageBox.Show($"Notification: {message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            });

            try
            {
                await _connection.StartAsync();
                MessageBox.Show("Connected to SignalR server!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var userId = "targetUserId";
                var message = "Hello from WinForms!";
                await _connection.InvokeAsync("SendNotificationToUser", userId, message);
            }
        }
    }
}
