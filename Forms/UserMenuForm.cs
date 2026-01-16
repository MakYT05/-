using System;
using System.Drawing;
using System.Windows.Forms;

namespace InsuranceApp.Forms
{
    public class UserMenuForm : Form
    {
        private Button btnRequest = new();
        private Button btnBooking = new();

        public UserMenuForm()
        {
            Text = "Меню пользователя";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        private void InitializeControls()
        {
            var label = new Label
            {
                Text = "Выберите действие:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(100, 50)
            };
            Controls.Add(label);

            btnRequest.Text = "Подать заявку на страхование";
            btnRequest.Location = new Point(80, 120);
            btnRequest.Size = new Size(240, 40);
            btnRequest.BackColor = Color.LightGreen;
            btnRequest.FlatStyle = FlatStyle.Flat;
            btnRequest.Click += (s, e) =>
            {
                var requestForm = new UserRequestForm();
                requestForm.ShowDialog();
            };
            Controls.Add(btnRequest);

            btnBooking.Text = "Забронировать время визита";
            btnBooking.Location = new Point(80, 180);
            btnBooking.Size = new Size(240, 40);
            btnBooking.BackColor = Color.LightBlue;
            btnBooking.FlatStyle = FlatStyle.Flat;
            btnBooking.Click += (s, e) =>
            {
                var bookingForm = new BookingForm();
                bookingForm.ShowDialog();
            };
            Controls.Add(btnBooking);
        }
    }
}