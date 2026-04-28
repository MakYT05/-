using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class RegisterForm : Form
    {
        private TextBox tbFio = new();
        private TextBox tbPhone = new();
        private TextBox tbLogin = new();
        private TextBox tbPassword = new();
        private TextBox tbConfirmPassword = new();
        private Button btnRegister = new();
        private Button btnCancel = new();
        private Label lblMessage = new();

        public string Логин => tbLogin.Text;

        public RegisterForm()
        {
            Text = "Регистрация нового пользователя";
            Size = new Size(400, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        private void InitializeControls()
        {
            int yPos = 20;

            var lblFio = new Label
            {
                Text = "ФИО:",
                Location = new Point(20, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            tbFio.Location = new Point(150, yPos);
            tbFio.Size = new Size(200, 20);
            Controls.Add(lblFio);
            Controls.Add(tbFio);

            yPos += 40;

            var lblPhone = new Label
            {
                Text = "Телефон:",
                Location = new Point(20, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            tbPhone.Location = new Point(150, yPos);
            tbPhone.Size = new Size(200, 20);
            Controls.Add(lblPhone);
            Controls.Add(tbPhone);

            yPos += 40;

            var lblLogin = new Label
            {
                Text = "Логин:",
                Location = new Point(20, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            tbLogin.Location = new Point(150, yPos);
            tbLogin.Size = new Size(200, 20);
            Controls.Add(lblLogin);
            Controls.Add(tbLogin);

            yPos += 40;

            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(20, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            tbPassword.Location = new Point(150, yPos);
            tbPassword.Size = new Size(200, 20);
            tbPassword.PasswordChar = '*';
            Controls.Add(lblPassword);
            Controls.Add(tbPassword);

            yPos += 40;

            var lblConfirmPassword = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(20, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            tbConfirmPassword.Location = new Point(150, yPos);
            tbConfirmPassword.Size = new Size(200, 20);
            tbConfirmPassword.PasswordChar = '*';
            Controls.Add(lblConfirmPassword);
            Controls.Add(tbConfirmPassword);

            yPos += 50;

            btnRegister.Text = "Зарегистрироваться";
            btnRegister.Location = new Point(80, yPos);
            btnRegister.Size = new Size(120, 30);
            btnRegister.BackColor = Color.LightGreen;
            btnRegister.Click += (s, e) => Register();

            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(220, yPos);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            lblMessage.Location = new Point(20, yPos + 40);
            lblMessage.Size = new Size(360, 30);
            lblMessage.Font = new Font("Arial", 9);
            lblMessage.ForeColor = Color.Red;

            Controls.AddRange(new Control[] { btnRegister, btnCancel, lblMessage });

            AcceptButton = btnRegister;
            CancelButton = btnCancel;
        }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(tbFio.Text))
            {
                lblMessage.Text = "Введите ФИО";
                return;
            }

            if (string.IsNullOrWhiteSpace(tbPhone.Text))
            {
                lblMessage.Text = "Введите телефон";
                return;
            }

            if (string.IsNullOrWhiteSpace(tbLogin.Text))
            {
                lblMessage.Text = "Введите логин";
                return;
            }

            if (string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                lblMessage.Text = "Введите пароль";
                return;
            }

            if (tbPassword.Text != tbConfirmPassword.Text)
            {
                lblMessage.Text = "Пароли не совпадают";
                return;
            }

            if (tbPassword.Text.Length < 4)
            {
                lblMessage.Text = "Пароль должен быть не менее 4 символов";
                return;
            }

            try
            {
                DatabaseService.ЗарегистрироватьПользователя(
                    tbLogin.Text.Trim(),
                    tbPassword.Text,
                    tbFio.Text.Trim(),
                    tbPhone.Text.Trim(),
                    "user"
                );
                
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти в систему.", 
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                lblMessage.Text = "Пользователь с таким логином уже существует";
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Ошибка регистрации: {ex.Message}";
            }
        }
    }
}