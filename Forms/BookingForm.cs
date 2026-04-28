using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class BookingForm : Form
    {
        private DateTimePicker datePicker = new();
        private ComboBox cbTime = new();
        private TextBox tbFio = new();
        private TextBox tbPhone = new();
        private Button btnSubmit = new();
        private ErrorProvider errorProvider = new ErrorProvider();
        
        private string _userLogin;
        private string _userFIO;

        public BookingForm()
        {
            Text = "Бронирование времени визита";
            Size = new Size(400, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        public BookingForm(string userLogin, string userFIO)
        {
            _userLogin = userLogin;
            _userFIO = userFIO;

            Text = "Бронирование времени визита";
            Size = new Size(400, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
            
            tbFio.Text = _userFIO;
            tbFio.ReadOnly = true;
            tbFio.BackColor = Color.LightGray;
            
            LoadUserPhone();
        }

        private void LoadUserPhone()
        {
            try
            {
                var пользователи = DatabaseService.GetUsers();
                var пользователь = пользователи.Find(u => u.Логин == _userLogin);
                if (пользователь != null)
                {
                    tbPhone.Text = пользователь.Телефон;
                    tbPhone.ReadOnly = true;
                    tbPhone.BackColor = Color.LightGray;
                }
            }
            catch{}
        }

        private void InitializeControls()
        {
            errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            errorProvider.ContainerControl = this;

            int y = 20;

            Controls.Add(MakeLabel("ФИО:", 20, y));
            tbFio.Location = new Point(120, y); 
            tbFio.Width = 240;
            Controls.Add(tbFio);

            y += 40;
            Controls.Add(MakeLabel("Телефон:", 20, y));
            tbPhone.Location = new Point(120, y); 
            tbPhone.Width = 240;
            Controls.Add(tbPhone);

            y += 40;
            Controls.Add(MakeLabel("Дата визита:", 20, y));
            datePicker.Location = new Point(120, y); 
            datePicker.Width = 240;
            datePicker.MinDate = DateTime.Today;
            datePicker.MaxDate = DateTime.Today.AddMonths(1);
            Controls.Add(datePicker);

            y += 40;
            Controls.Add(MakeLabel("Время:", 20, y));
            cbTime.Location = new Point(120, y); 
            cbTime.Width = 240;
            cbTime.DropDownStyle = ComboBoxStyle.DropDownList;
            
            for (int hour = 9; hour <= 18; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    if (hour == 18 && minute > 0) break;
                    cbTime.Items.Add($"{hour:D2}:{minute:D2}");
                }
            }
            Controls.Add(cbTime);

            y += 60;
            btnSubmit.Text = "Забронировать время";
            btnSubmit.Location = new Point(100, y);
            btnSubmit.Size = new Size(200, 35);
            btnSubmit.BackColor = Color.LightGreen;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Click += (s, e) => SubmitBooking();
            Controls.Add(btnSubmit);
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y + 5),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
        }

        private void SubmitBooking()
        {
            errorProvider.Clear();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(tbFio.Text))
            {
                errorProvider.SetError(tbFio, "Введите ФИО");
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(tbPhone.Text))
            {
                errorProvider.SetError(tbPhone, "Введите телефон");
                valid = false;
            }
            if (cbTime.SelectedIndex < 0)
            {
                errorProvider.SetError(cbTime, "Выберите время");
                valid = false;
            }

            if (!valid) return;

            string selectedDate = datePicker.Value.ToString("yyyy-MM-dd");
            string selectedTime = cbTime.SelectedItem.ToString();
            
            try
            {
                DatabaseService.ДобавитьБронирование(tbFio.Text, tbPhone.Text, selectedDate, selectedTime);
                
                MessageBox.Show($"Время успешно забронировано!\n\n" +
                              $"ФИО: {tbFio.Text}\n" +
                              $"Телефон: {tbPhone.Text}\n" +
                              $"Дата: {selectedDate}\n" +
                              $"Время: {selectedTime}", 
                              "Бронирование подтверждено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при бронировании: {ex.Message}", "Ошибка");
            }

            Close();
        }
    }
}