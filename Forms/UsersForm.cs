using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class UsersForm : Form
    {
        private DataGridView gridUsers = new();
        private ComboBox cbRole = new();
        private Button btnChangeRole = new();
        private Button btnDeleteUser = new();

        public UsersForm()
        {
            Text = "Управление пользователями";
            Size = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
            LoadData();
        }

        private void InitializeControls()
        {
            gridUsers.Location = new Point(10, 10);
            gridUsers.Size = new Size(580, 250);
            gridUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridUsers.ReadOnly = true;

            var lblRole = new Label
            {
                Text = "Изменить роль:",
                Location = new Point(10, 280),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };

            cbRole.Location = new Point(120, 280);
            cbRole.Size = new Size(100, 20);
            cbRole.Items.AddRange(new string[] { "user", "admin" });
            cbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            btnChangeRole.Text = "Изменить роль";
            btnChangeRole.Location = new Point(240, 280);
            btnChangeRole.Size = new Size(120, 25);
            btnChangeRole.BackColor = Color.LightYellow;
            btnChangeRole.Click += (s, e) => ChangeUserRole();

            btnDeleteUser.Text = "Удалить пользователя";
            btnDeleteUser.Location = new Point(370, 280);
            btnDeleteUser.Size = new Size(150, 25);
            btnDeleteUser.BackColor = Color.LightCoral;
            btnDeleteUser.Click += (s, e) => DeleteUser();

            Controls.AddRange(new Control[] { 
                gridUsers, lblRole, cbRole, btnChangeRole, btnDeleteUser 
            });
        }

        private void LoadData()
        {
            var users = DatabaseService.GetUsers();
            gridUsers.DataSource = users;
            
            if (gridUsers.Columns.Count > 0)
            {
                gridUsers.Columns["Id"].HeaderText = "ID";
                gridUsers.Columns["Логин"].HeaderText = "Логин";
                gridUsers.Columns["Пароль"].HeaderText = "Пароль";
                gridUsers.Columns["Роль"].HeaderText = "Роль";
                gridUsers.Columns["ФИО"].HeaderText = "ФИО";
            }
        }

        private void ChangeUserRole()
        {
            if (gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            if (cbRole.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите роль");
                return;
            }

            var id = (int)gridUsers.SelectedRows[0].Cells["Id"].Value;
            var логин = gridUsers.SelectedRows[0].Cells["Логин"].Value.ToString();
            var новаяРоль = cbRole.SelectedItem.ToString();

            if (MessageBox.Show($"Изменить роль пользователя {логин} на '{новаяРоль}'?", 
                "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseService.ОбновитьРольПользователя(id, новаяРоль);
                LoadData();
                MessageBox.Show("Роль изменена!");
            }
        }

        private void DeleteUser()
        {
            if (gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            var id = (int)gridUsers.SelectedRows[0].Cells["Id"].Value;
            var логин = gridUsers.SelectedRows[0].Cells["Логин"].Value.ToString();

            if (MessageBox.Show($"Удалить пользователя {логин}?", 
                "Подтверждение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseService.УдалитьПользователя(id);
                LoadData();
                MessageBox.Show("Пользователь удален!");
            }
        }
    }
}