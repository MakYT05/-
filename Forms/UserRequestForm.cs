using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Models;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class UserRequestForm : Form
    {
        private TextBox tbFio = new();
        private TextBox tbPhone = new();
        private ComboBox cbType = new();
        private TextBox tbAddress = new();
        private TextBox tbCost = new();
        private TextBox tbSrok = new();
        private Button btnCases = new();
        private Button btnConditions = new();
        private Button btnSubmit = new();
        private ErrorProvider errorProvider = new ErrorProvider();

        public UserRequestForm()
        {
            Text = "Заявка на страхование";
            Size = new Size(500, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        private void InitializeControls()
        {
            errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            errorProvider.ContainerControl = this;

            int y = 20;

            Controls.Add(MakeLabel("ФИО:", 20, y));
            tbFio.Location = new Point(160, y); tbFio.Width = 300;
            Controls.Add(tbFio);

            y += 40;
            Controls.Add(MakeLabel("Телефон:", 20, y));
            tbPhone.Location = new Point(160, y); tbPhone.Width = 300;
            Controls.Add(tbPhone);

            y += 40;
            Controls.Add(MakeLabel("Что страхуем:", 20, y));
            cbType.Location = new Point(160, y); cbType.Width = 300;
            cbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType.Items.AddRange(new string[] { "Квартира", "Дом", "Автомобиль", "Гараж", "Офисное помещение" });
            Controls.Add(cbType);

            y += 40;
            Controls.Add(MakeLabel("Адрес:", 20, y));
            tbAddress.Location = new Point(160, y); tbAddress.Width = 300;
            Controls.Add(tbAddress);

            y += 40;
            Controls.Add(MakeLabel("Стоимость:", 20, y));
            tbCost.Location = new Point(160, y); tbCost.Width = 300;
            Controls.Add(tbCost);

            y += 40;
            Controls.Add(MakeLabel("Срок (дней):", 20, y));
            tbSrok.Location = new Point(160, y); tbSrok.Width = 300;
            Controls.Add(tbSrok);

            y += 60;
            
            btnCases.Text = "Страховые случаи";
            btnCases.Location = new Point(20, y);
            btnCases.Size = new Size(140, 35);
            btnCases.BackColor = Color.LightBlue;
            btnCases.FlatStyle = FlatStyle.Flat;
            btnCases.Click += (s, e) => new CasesForm().ShowDialog();
            Controls.Add(btnCases);

            btnConditions.Text = "Условия страхования";
            btnConditions.Location = new Point(180, y);
            btnConditions.Size = new Size(140, 35);
            btnConditions.BackColor = Color.LightGoldenrodYellow;
            btnConditions.FlatStyle = FlatStyle.Flat;
            btnConditions.Click += (s, e) => new InsuranceConditionsForm().ShowDialog();
            Controls.Add(btnConditions);

            btnSubmit.Text = "Подать заявку";
            btnSubmit.Location = new Point(340, y);
            btnSubmit.Size = new Size(140, 35);
            btnSubmit.BackColor = Color.LightGreen;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Click += (s, e) => SubmitRequest();
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

        private void SubmitRequest()
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
            if (cbType.SelectedIndex < 0)
            {
                errorProvider.SetError(cbType, "Выберите объект страхования");
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                errorProvider.SetError(tbAddress, "Введите адрес");
                valid = false;
            }
            if (!decimal.TryParse(tbCost.Text, out decimal cost))
            {
                errorProvider.SetError(tbCost, "Введите корректную стоимость");
                valid = false;
            }
            if (!int.TryParse(tbSrok.Text, out int srok))
            {
                errorProvider.SetError(tbSrok, "Введите срок (число)");
                valid = false;
            }

            if (!valid) return;

            decimal премия = cost * 0.02m;

            var request = new Заявка
            {
                ФИО = tbFio.Text,
                Телефон = tbPhone.Text,
                ТипИмущества = cbType.SelectedItem.ToString(),
                Адрес = tbAddress.Text,
                Стоимость = cost,
                Премия = премия,
                Срок = srok
            };

            DatabaseService.ДобавитьЗаявку(request);
            MessageBox.Show($"Заявка успешно подана! Рассчитанная премия: {премия:C}");
            Close();
        }
    }
}