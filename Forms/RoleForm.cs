using System;
using System.Drawing;
using System.Windows.Forms;

namespace InsuranceApp.Forms
{
    public class RoleForm : Form
    {
        private Button btnAdmin = new();
        private Button btnUser = new();

        public RoleForm()
        {
            Text = "Выбор роли";
            Size = new Size(300, 200);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var label = new Label
            {
                Text = "Выберите вашу роль:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(80, 30)
            };
            Controls.Add(label);

            btnAdmin.Text = "Администратор";
            btnAdmin.Location = new Point(80, 70);
            btnAdmin.Size = new Size(140, 30);
            btnAdmin.Click += (s, e) => CheckAdminPassword();
            Controls.Add(btnAdmin);

            btnUser.Text = "Пользователь";
            btnUser.Location = new Point(80, 110);
            btnUser.Size = new Size(140, 30);
            btnUser.Click += (s, e) =>
            {
                var userMenu = new UserMenuForm();
                userMenu.ShowDialog();
            };
            Controls.Add(btnUser);
        }

        private void CheckAdminPassword()
        {
            using (var passwordForm = new PasswordForm())
            {
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    var mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
            }
        }
    }

    public class PasswordForm : Form
    {
        private TextBox tbPassword = new();
        private Button btnOk = new();
        private Button btnCancel = new();

        public PasswordForm()
        {
            Text = "Ввод пароля";
            Size = new Size(300, 150);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var label = new Label
            {
                Text = "Введите пароль администратора:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(label);

            tbPassword.Location = new Point(20, 50);
            tbPassword.Size = new Size(240, 20);
            tbPassword.PasswordChar = '*';
            Controls.Add(tbPassword);

            btnOk.Text = "OK";
            btnOk.Location = new Point(60, 80);
            btnOk.Size = new Size(80, 25);
            btnOk.Click += (s, e) => ValidatePassword();
            Controls.Add(btnOk);

            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(150, 80);
            btnCancel.Size = new Size(80, 25);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(btnCancel);
        }

        private void ValidatePassword()
        {
            if (tbPassword.Text == "123")
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbPassword.Text = "";
                tbPassword.Focus();
            }
        }
    }
}