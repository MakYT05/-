using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InsuranceApp.Models;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class UserAccountForm : Form
    {
        private TabControl tabControl = new();
        private string _userLogin;
        private string _userFIO;
        private string _userPhone;

        public UserAccountForm(string login, string fio, string phone)
        {
            _userLogin = login;
            _userFIO = fio;
            _userPhone = phone;

            Text = "Личный кабинет";
            Size = new Size(700, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        private void InitializeControls()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            var lblHeader = new Label
            {
                Text = $"Личный кабинет: {_userFIO}",
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            var lblPhone = new Label
            {
                Text = $"Телефон: {_userPhone}",
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 9),
                Location = new Point(350, 25),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblPhone);

            var btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(550, 15),
                Size = new Size(120, 30),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += (s, e) => RefreshAllData();
            headerPanel.Controls.Add(btnRefresh);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10),
                Padding = new Point(10, 10)
            };

            var tabSettings = new TabPage("Настройки аккаунта");
            tabSettings.Controls.Add(CreateSettingsPanel());
            tabControl.TabPages.Add(tabSettings);

            var tabRequests = new TabPage("Мои заявки");
            tabRequests.Controls.Add(CreateRequestsPanel());
            tabControl.TabPages.Add(tabRequests);

            var tabBookings = new TabPage("Мои бронирования");
            tabBookings.Controls.Add(CreateBookingsPanel());
            tabControl.TabPages.Add(tabBookings);

            var tabPolicies = new TabPage("Мои полисы");
            tabPolicies.Controls.Add(CreatePoliciesPanel());
            tabControl.TabPages.Add(tabPolicies);

            var tabNotifications = new TabPage("Уведомления");
            tabNotifications.Controls.Add(CreateNotificationsPanel());
            tabControl.TabPages.Add(tabNotifications);

            var btnClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(300, 480),
                Size = new Size(100, 30),
                BackColor = Color.LightCoral,
                DialogResult = DialogResult.OK
            };

            Controls.Add(btnClose);
            Controls.Add(tabControl);
            Controls.Add(headerPanel);
        }

        private void RefreshAllData()
        {
            try
            {
                _userPhone = GetUserPhone();
                
                var tabRequests = tabControl.TabPages["Мои заявки"];
                if (tabRequests != null)
                {
                    tabRequests.Controls.Clear();
                    tabRequests.Controls.Add(CreateRequestsPanel());
                }

                var tabBookings = tabControl.TabPages["Мои бронирования"];
                if (tabBookings != null)
                {
                    tabBookings.Controls.Clear();
                    tabBookings.Controls.Add(CreateBookingsPanel());
                }

                var tabPolicies = tabControl.TabPages["Мои полисы"];
                if (tabPolicies != null)
                {
                    tabPolicies.Controls.Clear();
                    tabPolicies.Controls.Add(CreatePoliciesPanel());
                }

                var tabNotifications = tabControl.TabPages["Уведомления"];
                if (tabNotifications != null)
                {
                    tabNotifications.Controls.Clear();
                    tabNotifications.Controls.Add(CreateNotificationsPanel());
                }

                MessageBox.Show("Данные обновлены!", "Обновление", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateSettingsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            int yPos = 30;

            var lblCurrentLogin = new Label
            {
                Text = $"Ваш логин: {_userLogin}",
                Location = new Point(30, yPos),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            panel.Controls.Add(lblCurrentLogin);

            yPos += 30;

            var lblCurrentPhone = new Label
            {
                Text = $"Ваш телефон: {_userPhone}",
                Location = new Point(30, yPos),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            panel.Controls.Add(lblCurrentPhone);

            yPos += 50;

            var lblOldPassword = new Label
            {
                Text = "Старый пароль:",
                Location = new Point(30, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            panel.Controls.Add(lblOldPassword);

            var tbOldPassword = new TextBox
            {
                Location = new Point(150, yPos - 3),
                Size = new Size(200, 20),
                PasswordChar = '*'
            };
            panel.Controls.Add(tbOldPassword);

            yPos += 40;

            var lblNewPassword = new Label
            {
                Text = "Новый пароль:",
                Location = new Point(30, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            panel.Controls.Add(lblNewPassword);

            var tbNewPassword = new TextBox
            {
                Location = new Point(150, yPos - 3),
                Size = new Size(200, 20),
                PasswordChar = '*'
            };
            panel.Controls.Add(tbNewPassword);

            yPos += 40;

            var lblConfirmPassword = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(30, yPos),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            panel.Controls.Add(lblConfirmPassword);

            var tbConfirmPassword = new TextBox
            {
                Location = new Point(170, yPos - 3),
                Size = new Size(180, 20),
                PasswordChar = '*'
            };
            panel.Controls.Add(tbConfirmPassword);

            yPos += 50;

            var btnChangePassword = new Button
            {
                Text = "Сменить пароль",
                Location = new Point(150, yPos),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            btnChangePassword.Click += (s, e) => ChangePassword(tbOldPassword.Text, tbNewPassword.Text, tbConfirmPassword.Text);
            panel.Controls.Add(btnChangePassword);

            return panel;
        }

        private Panel CreateRequestsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            try
            {
                var allRequests = DatabaseService.ПолучитьЗаявки();
                var userRequests = allRequests.Where(r => r.Телефон == _userPhone).ToList();

                var listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 10),
                    HorizontalScrollbar = true
                };

                if (userRequests.Count == 0)
                {
                    listBox.Items.Add("─── У вас нет заявок ───");
                    listBox.Items.Add("");
                    listBox.Items.Add("Чтобы подать заявку:");
                    listBox.Items.Add("1. Вернитесь в главное меню");
                    listBox.Items.Add("2. Нажмите 'Подать заявку на страхование'");
                    listBox.Items.Add("3. Заполните форму");
                }
                else
                {
                    listBox.Items.Add($"─── Найдено заявок: {userRequests.Count} ───");
                    listBox.Items.Add("");

                    foreach (var request in userRequests)
                    {
                        listBox.Items.Add("═══════════════════════════════════════");
                        listBox.Items.Add($"📋 Заявка №{request.Id}");
                        listBox.Items.Add($"   ФИО: {request.ФИО}");
                        listBox.Items.Add($"   Телефон: {request.Телефон}");
                        listBox.Items.Add($"   Объект: {request.ТипИмущества}");
                        listBox.Items.Add($"   Адрес: {request.Адрес}");
                        listBox.Items.Add($"   Стоимость: {request.Стоимость:C}");
                        listBox.Items.Add($"   Премия: {request.Премия:C}");
                        listBox.Items.Add($"   Срок: {request.Срок} дней");
                    }
                }

                panel.Controls.Add(listBox);
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Ошибка загрузки: {ex.Message}",
                    ForeColor = Color.Red,
                    AutoSize = true
                };
                panel.Controls.Add(lblError);
            }

            return panel;
        }

        private Panel CreateBookingsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            try
            {
                var allBookings = DatabaseService.ПолучитьБронирования();
                var userBookings = allBookings.Where(b => b.Телефон == _userPhone).ToList();

                var listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 10),
                    HorizontalScrollbar = true
                };

                if (userBookings.Count == 0)
                {
                    listBox.Items.Add("─── У вас нет бронирований ───");
                    listBox.Items.Add("");
                    listBox.Items.Add("Чтобы забронировать время:");
                    listBox.Items.Add("1. Вернитесь в главное меню");
                    listBox.Items.Add("2. Нажмите 'Забронировать время визита'");
                    listBox.Items.Add("3. Выберите дату и время");
                }
                else
                {
                    listBox.Items.Add($"─── Найдено бронирований: {userBookings.Count} ───");
                    listBox.Items.Add("");

                    foreach (var booking in userBookings)
                    {
                        listBox.Items.Add("═══════════════════════════════════════");
                        listBox.Items.Add($"📅 Бронирование №{booking.Id}");
                        listBox.Items.Add($"   ФИО: {booking.ФИО}");
                        listBox.Items.Add($"   Телефон: {booking.Телефон}");
                        listBox.Items.Add($"   Дата визита: {booking.Дата}");
                        listBox.Items.Add($"   Время: {booking.Время}");
                        listBox.Items.Add($"   Статус: {booking.Статус}");
                    }
                }

                panel.Controls.Add(listBox);
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Ошибка загрузки: {ex.Message}",
                    ForeColor = Color.Red,
                    AutoSize = true
                };
                panel.Controls.Add(lblError);
            }

            return panel;
        }

        private Panel CreatePoliciesPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            try
            {
                var полисыList = DatabaseService.GetPolicies();
                var клиентыList = DatabaseService.GetClients();
                var имуществоList = DatabaseService.GetProperties();

                var userPolicies = (from p in полисыList
                                   join c in клиентыList on p.КлиентId equals c.Id
                                   join i in имуществоList on p.ИмуществоId equals i.Id
                                   where c.Телефон == _userPhone
                                   select new
                                   {
                                       p.Id,
                                       c.ФИО,
                                       c.Телефон,
                                       i.Тип,
                                       i.Адрес,
                                       i.Стоимость,
                                       p.Премия,
                                       Начало = p.ДатаНачала,
                                       Окончание = p.ДатаОкончания
                                   }).ToList();

                var listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 10),
                    HorizontalScrollbar = true
                };

                if (userPolicies.Count == 0)
                {
                    listBox.Items.Add("─── У вас нет полисов ───");
                    listBox.Items.Add("");
                    listBox.Items.Add("Полисы появляются после того, как администратор");
                    listBox.Items.Add("примет вашу заявку на страхование.");
                }
                else
                {
                    int activeCount = userPolicies.Count(p => p.Окончание > DateTime.Now);
                    int expiredCount = userPolicies.Count - activeCount;

                    listBox.Items.Add($"─── Всего полисов: {userPolicies.Count} ───");
                    listBox.Items.Add($"   ✅ Действующих: {activeCount}");
                    listBox.Items.Add($"   ❌ Истекших: {expiredCount}");
                    listBox.Items.Add("");

                    foreach (var policy in userPolicies)
                    {
                        string status = policy.Окончание > DateTime.Now ? "✅ ДЕЙСТВУЕТ" : "❌ ИСТЕК";

                        listBox.Items.Add("═══════════════════════════════════════");
                        listBox.Items.Add($"📄 Полис №{policy.Id} - {status}");
                        listBox.Items.Add($"   Страхователь: {policy.ФИО}");
                        listBox.Items.Add($"   Объект: {policy.Тип}");
                        listBox.Items.Add($"   Адрес: {policy.Адрес}");
                        listBox.Items.Add($"   Страховая сумма: {policy.Стоимость:C}");
                        listBox.Items.Add($"   Страховая премия: {policy.Премия:C}");
                        listBox.Items.Add($"   Срок действия:");
                        listBox.Items.Add($"      с {policy.Начало:dd.MM.yyyy}");
                        listBox.Items.Add($"      по {policy.Окончание:dd.MM.yyyy}");

                        if (policy.Окончание > DateTime.Now)
                        {
                            int daysLeft = (policy.Окончание - DateTime.Now).Days;
                            listBox.Items.Add($"   Осталось дней: {daysLeft}");
                        }
                    }
                }

                panel.Controls.Add(listBox);
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Ошибка загрузки: {ex.Message}",
                    ForeColor = Color.Red,
                    AutoSize = true
                };
                panel.Controls.Add(lblError);
            }

            return panel;
        }

        private void ChangePassword(string oldPass, string newPass, string confirmPass)
        {
            if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Заполните все поля", "Ошибка");
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Новые пароли не совпадают", "Ошибка");
                return;
            }

            if (newPass.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать не менее 4 символов", "Ошибка");
                return;
            }

            var result = DatabaseService.СменитьПароль(_userLogin, oldPass, newPass);
            
            if (result)
            {
                MessageBox.Show("Пароль успешно изменен!", "Успех");
            }
            else
            {
                MessageBox.Show("Неверный старый пароль", "Ошибка");
            }
        }

        private string GetUserPhone()
        {
            try
            {
                if (!string.IsNullOrEmpty(_userPhone))
                {
                    return _userPhone;
                }

                var users = DatabaseService.GetUsers();
                var user = users.FirstOrDefault(u => u.Логин == _userLogin);
                return user?.Телефон ?? "";
            }
            catch
            {
                return "";
            }
        }

        private Panel CreateNotificationsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            try
            {
                int userId = DatabaseService.GetUserIdByLogin(_userLogin);
                var уведомления = DatabaseService.ПолучитьУведомленияПользователя(userId);

                var dataGrid = new DataGridView
                {
                    Location = new Point(10, 10),
                    Size = new Size(640, 280),
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false
                };
        
                var displayData = уведомления.Select(n => new
                {
                    n.Id,
                    Дата = n.ДатаСоздания.ToString("dd.MM.yyyy HH:mm"),
                    Тип = n.Тип,
                    Сообщение = n.Сообщение.Length > 50 ? n.Сообщение.Substring(0, 47) + "..." : n.Сообщение,
                    Статус = n.Прочитано ? "Прочитано" : "Новое",
                    ЕстьФайл = !string.IsNullOrEmpty(n.ПутьКФайлу) && File.Exists(n.ПутьКФайлу) ? "Да" : "Нет",
                    ПутьКФайлу = n.ПутьКФайлу
                }).ToList();

                dataGrid.DataSource = displayData;

                if (dataGrid.Columns.Count > 0)
                {
                    dataGrid.Columns["Id"].Visible = false;
                    dataGrid.Columns["ПутьКФайлу"].Visible = false;
                    dataGrid.Columns["Дата"].HeaderText = "Дата";
                    dataGrid.Columns["Тип"].HeaderText = "Тип";
                    dataGrid.Columns["Сообщение"].HeaderText = "Сообщение";
                    dataGrid.Columns["Статус"].HeaderText = "Статус";
                    dataGrid.Columns["ЕстьФайл"].HeaderText = "Документ";
                }

                var buttonPanel = new FlowLayoutPanel
                {
                    Location = new Point(10, 300),
                    Size = new Size(640, 40),
                    FlowDirection = FlowDirection.LeftToRight
                };

                Button btnDownload = new Button
                {
                    Text = "📥 Скачать договор",
                    Size = new Size(150, 30),
                    BackColor = Color.LightGreen,
                    FlatStyle = FlatStyle.Flat
                };
                btnDownload.Click += (s, e) => DownloadSelectedFile(dataGrid);

                Button btnMarkAsRead = new Button
                {
                    Text = "✓ Отметить как прочитанное",
                    Size = new Size(180, 30),
                    BackColor = Color.LightBlue,
                    FlatStyle = FlatStyle.Flat
                };
                btnMarkAsRead.Click += (s, e) => MarkAsRead(dataGrid);

                buttonPanel.Controls.Add(btnDownload);
                buttonPanel.Controls.Add(btnMarkAsRead);

                panel.Controls.Add(dataGrid);
                panel.Controls.Add(buttonPanel);
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Ошибка загрузки: {ex.Message}",
                    ForeColor = Color.Red,
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                panel.Controls.Add(lblError);
            }

            return panel;
        }

        private void DownloadSelectedFile(DataGridView grid)
        {
            try
            {
                if (grid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите уведомление с договором", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                }

                var selectedRow = grid.SelectedRows[0];
                string filePath = selectedRow.Cells["ПутьКФайлу"].Value?.ToString() ?? "";

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("Файл не найден или был удален", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.FileName = Path.GetFileName(filePath);
                    saveDialog.Filter = "PDF Files|*.pdf";
                    saveDialog.Title = "Сохранить договор";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(filePath, saveDialog.FileName, true);

                        int notificationId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                        DatabaseService.ОтметитьУведомлениеКакПрочитанное(notificationId);

                        MessageBox.Show($"Договор сохранен:\n{saveDialog.FileName}", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RefreshNotifications((Panel)grid.Parent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MarkAsRead(DataGridView grid)
        {
            try
            {
                if (grid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите уведомление", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedRow = grid.SelectedRows[0];
                int notificationId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                string status = selectedRow.Cells["Статус"].Value?.ToString() ?? "";

                if (status == "Прочитано")
                {
                    MessageBox.Show("Уведомление уже отмечено как прочитанное", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DatabaseService.ОтметитьУведомлениеКакПрочитанное(notificationId);
                MessageBox.Show("Уведомление отмечено как прочитанное", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshNotifications((Panel)grid.Parent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshNotifications(Panel panel)
        {
            try
            {
                panel.Controls.Clear();
                var newPanel = CreateNotificationsPanel();
                panel.Controls.Add(newPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }
    }
}