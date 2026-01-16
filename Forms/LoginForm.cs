using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class LoginForm : Form
    {
        private TextBox tbLogin = new();
        private TextBox tbPassword = new();
        private Button btnLogin = new();
        private Button btnRegister = new();
        private Label lblMessage = new();

        public string РольПользователя { get; private set; } = "";
        public string ФИОПользователя { get; private set; } = "";

        public LoginForm()
        {
            Text = "Вход в систему страхования";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        private void InitializeControls()
        {
            var lblTitle = new Label
            {
                Text = "Система страхования имущества",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(80, 20)
            };

            var lblLogin = new Label
            {
                Text = "Логин:",
                Location = new Point(50, 80),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };

            tbLogin.Location = new Point(120, 80);
            tbLogin.Size = new Size(200, 20);
            tbLogin.Font = new Font("Arial", 10);

            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(50, 120),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };

            tbPassword.Location = new Point(120, 120);
            tbPassword.Size = new Size(200, 20);
            tbPassword.PasswordChar = '*';
            tbPassword.Font = new Font("Arial", 10);

            btnLogin.Text = "Войти";
            btnLogin.Location = new Point(80, 170);
            btnLogin.Size = new Size(100, 35);
            btnLogin.BackColor = Color.LightGreen;
            btnLogin.Font = new Font("Arial", 10);
            btnLogin.Click += (s, e) => Login();

            btnRegister.Text = "Регистрация";
            btnRegister.Location = new Point(200, 170);
            btnRegister.Size = new Size(100, 35);
            btnRegister.BackColor = Color.LightBlue;
            btnRegister.Font = new Font("Arial", 10);
            btnRegister.Click += (s, e) => ShowRegistrationForm();

            lblMessage.Location = new Point(50, 220);
            lblMessage.Size = new Size(300, 30);
            lblMessage.Font = new Font("Arial", 9);
            lblMessage.ForeColor = Color.Red;

            Controls.AddRange(new Control[] { 
                lblTitle, lblLogin, tbLogin, lblPassword, tbPassword, 
                btnLogin, btnRegister, lblMessage 
            });

            AcceptButton = btnLogin;
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(tbLogin.Text) || string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                lblMessage.Text = "Введите логин и пароль";
                return;
            }

            var result = DatabaseService.ПроверитьАвторизацию(tbLogin.Text.Trim(), tbPassword.Text);
            
            if (result.успех)
            {
                РольПользователя = result.роль;
                ФИОПользователя = result.фио;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblMessage.Text = "Неверный логин или пароль";
                tbPassword.Text = "";
                tbPassword.Focus();
            }
        }

        private void ShowRegistrationForm()
        {
            var registerForm = new RegisterForm();
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                lblMessage.Text = "Регистрация успешна! Теперь войдите в систему";
                lblMessage.ForeColor = Color.Green;
                tbLogin.Text = registerForm.Логин;
                tbPassword.Text = "";
                tbPassword.Focus();
            }
        }
    }
}