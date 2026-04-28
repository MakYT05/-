using System;
using System.Drawing;
using System.Windows.Forms;

namespace InsuranceApp.Forms
{
    public class UserMenuForm : Form
    {
        private Button btnRequest = new();
        private Button btnBooking = new();
        private Button btnAccount = new();
        private Panel headerPanel = new();

        private string _userLogin;
        private string _userFIO;
        private string _userRole;
        private string _userPhone;
        public UserMenuForm(string login, string fio, string role, string phone)
        {
            _userLogin = login;
            _userFIO = fio;
            _userRole = role;
            _userPhone = phone;
        
            Text = "Меню пользователя";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        
            InitializeControls();
        }
        private void InitializeControls()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            var lblWelcome = new Label
            {
                Text = $"Добро пожаловать, {_userFIO}!",
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 20),
                AutoSize = true
            };

            btnAccount.Text = "👤";
            btnAccount.Font = new Font("Arial", 16);
            btnAccount.Size = new Size(40, 40);
            btnAccount.Location = new Point(this.Width - 60, 10);
            btnAccount.FlatStyle = FlatStyle.Flat;
            btnAccount.FlatAppearance.BorderSize = 0;
            btnAccount.BackColor = Color.FromArgb(41, 128, 185);
            btnAccount.ForeColor = Color.White;
            btnAccount.Click += (s, e) => OpenAccountForm();

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(btnAccount);

            var panelMenu = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(50)
            };

            var lblMenu = new Label
            {
                Text = "Выберите действие:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(100, 30)
            };

            btnRequest.Text = "Подать заявку на страхование";
            btnRequest.Location = new Point(80, 80);
            btnRequest.Size = new Size(280, 50);
            btnRequest.BackColor = Color.FromArgb(46, 204, 113);
            btnRequest.ForeColor = Color.White;
            btnRequest.Font = new Font("Arial", 10);
            btnRequest.FlatStyle = FlatStyle.Flat;
            btnRequest.Click += (s, e) =>
            {
                var requestForm = new UserRequestForm();
                requestForm.ShowDialog();
            };

            btnBooking.Text = "Забронировать время визита";
            btnBooking.Location = new Point(80, 150);
            btnBooking.Size = new Size(280, 50);
            btnBooking.BackColor = Color.FromArgb(52, 152, 219);
            btnBooking.ForeColor = Color.White;
            btnBooking.Font = new Font("Arial", 10);
            btnBooking.FlatStyle = FlatStyle.Flat;
            btnBooking.Click += (s, e) =>
            {
                var bookingForm = new BookingForm(_userLogin, _userFIO);
                bookingForm.ShowDialog();
            };

            panelMenu.Controls.Add(lblMenu);
            panelMenu.Controls.Add(btnRequest);
            panelMenu.Controls.Add(btnBooking);

            Controls.Add(panelMenu);
            Controls.Add(headerPanel);
        }

        private void OpenAccountForm()
        {
            var accountForm = new UserAccountForm(_userLogin, _userFIO, _userPhone);
            accountForm.ShowDialog();
        }
    }
}