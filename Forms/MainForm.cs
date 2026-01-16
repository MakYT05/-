using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class MainForm : Form
    {
        private DataGridView gridКлиенты = new();
        private DataGridView gridИмущество = new();
        private DataGridView gridПолисы = new();

        private Button btnAddClient = new();
        private Button btnDeleteClient = new();
        private Button btnAddProperty = new();
        private Button btnDeleteProperty = new();
        private Button btnAddPolicy = new();
        private Button btnDeletePolicy = new();
        private Button btnRequests = new();
        private Button btnViewBookings = new();
        private Button btnManageUsers = new();

        public MainForm()
        {
            Text = "Система страхования имущества";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;

            BackgroundImage = CreateBackgroundImage();
            BackgroundImageLayout = ImageLayout.Stretch;

            InitializeControls();
            LoadData();
            SetupGridStyles();
        }

        private Bitmap CreateBackgroundImage()
        {
            var bmp = new Bitmap(Width, Height);
            using (var g = Graphics.FromImage(bmp))
            using (var brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(Width, Height),
                Color.Lavender,
                Color.AliceBlue))
            {
                g.FillRectangle(brush, 0, 0, Width, Height);

                using (var texture = new HatchBrush(
                    HatchStyle.LightDownwardDiagonal,
                    Color.FromArgb(30, Color.LightSteelBlue),
                    Color.Transparent))
                {
                    g.FillRectangle(texture, 0, 0, Width, Height);
                }
            }
            return bmp;
        }

        private void InitializeControls()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            var panelClients = new Panel { Dock = DockStyle.Fill };
            var lblClients = new Label
            {
                Text = "Клиенты",
                Dock = DockStyle.Top,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            gridКлиенты.Dock = DockStyle.Fill;
            var panelClientBtns = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
            btnAddClient.Text = "Добавить"; 
            btnAddClient.BackColor = Color.LightGreen;
            btnAddClient.Size = new Size(80, 30);
            btnDeleteClient.Text = "Удалить"; 
            btnDeleteClient.BackColor = Color.LightCoral;
            btnDeleteClient.Size = new Size(80, 30);
            btnAddClient.Click += (s, e) => AddClient();
            btnDeleteClient.Click += (s, e) => DeleteClient();
            panelClientBtns.Controls.AddRange(new Control[] { btnAddClient, btnDeleteClient });
            panelClients.Controls.AddRange(new Control[] { gridКлиенты, lblClients, panelClientBtns });

            var panelProps = new Panel { Dock = DockStyle.Fill };
            var lblProps = new Label
            {
                Text = "Имущество",
                Dock = DockStyle.Top,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            gridИмущество.Dock = DockStyle.Fill;
            var panelPropBtns = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
            btnAddProperty.Text = "Добавить"; 
            btnAddProperty.BackColor = Color.LightGreen;
            btnAddProperty.Size = new Size(80, 30);
            btnDeleteProperty.Text = "Удалить"; 
            btnDeleteProperty.BackColor = Color.LightCoral;
            btnDeleteProperty.Size = new Size(80, 30);
            btnAddProperty.Click += (s, e) => AddProperty();
            btnDeleteProperty.Click += (s, e) => DeleteProperty();
            panelPropBtns.Controls.AddRange(new Control[] { btnAddProperty, btnDeleteProperty });
            panelProps.Controls.AddRange(new Control[] { gridИмущество, lblProps, panelPropBtns });

            var panelPolicies = new Panel { Dock = DockStyle.Fill };
            var lblPolicies = new Label
            {
                Text = "Полисы страхования",
                Dock = DockStyle.Top,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            gridПолисы.Dock = DockStyle.Fill;

            var panelPolicyBtns = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
            btnAddPolicy.Text = "Добавить полис"; 
            btnAddPolicy.BackColor = Color.LightGreen;
            btnAddPolicy.Size = new Size(100, 30);
            btnDeletePolicy.Text = "Удалить полис"; 
            btnDeletePolicy.BackColor = Color.LightCoral;
            btnDeletePolicy.Size = new Size(100, 30);
            btnRequests.Text = "Посмотреть заявки"; 
            btnRequests.BackColor = Color.LightBlue;
            btnRequests.Size = new Size(120, 30);

            btnAddPolicy.Click += (s, e) => AddPolicy();
            btnDeletePolicy.Click += (s, e) => DeletePolicy();
            btnRequests.Click += (s, e) =>
            {
                var reqForm = new RequestsForm();
                reqForm.ЗаявкаПринята += () => LoadData();
                reqForm.ShowDialog();
            };
            panelPolicyBtns.Controls.AddRange(new Control[] { btnAddPolicy, btnDeletePolicy, btnRequests });

            var panelBookingBtns = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
            btnViewBookings.Text = "Посмотреть бронь"; 
            btnViewBookings.BackColor = Color.LightYellow;
            btnViewBookings.Size = new Size(120, 30);
            btnViewBookings.Click += (s, e) => ViewBookings();
            panelBookingBtns.Controls.Add(btnViewBookings);

            var policiesContainer = new Panel { Dock = DockStyle.Fill };
            policiesContainer.Controls.Add(gridПолисы);
            policiesContainer.Controls.Add(panelPolicyBtns);
            policiesContainer.Controls.Add(panelBookingBtns);

            policiesContainer.Controls.SetChildIndex(panelBookingBtns, 0);
            policiesContainer.Controls.SetChildIndex(panelPolicyBtns, 1);
            policiesContainer.Controls.SetChildIndex(gridПолисы, 2);

            panelPolicies.Controls.AddRange(new Control[] { lblPolicies, policiesContainer });

            layout.Controls.Add(panelClients, 0, 0);
            layout.Controls.Add(panelProps, 0, 1);
            layout.Controls.Add(panelPolicies, 1, 0);
            layout.SetRowSpan(panelPolicies, 2);

            btnManageUsers.Text = "Управление пользователями";
            btnManageUsers.BackColor = Color.LightGoldenrodYellow;
            btnManageUsers.Size = new Size(150, 30);
            btnManageUsers.Click += (s, e) => ManageUsers();
            panelPolicyBtns.Controls.Add(btnManageUsers);

            Controls.Add(layout);
        }

        private void SetupGridStyles()
        {
            foreach (var grid in new[] { gridКлиенты, gridИмущество, gridПолисы })
            {
                grid.BorderStyle = BorderStyle.FixedSingle;
                grid.BackgroundColor = Color.White;
                grid.DefaultCellStyle.Font = new Font("Arial", 9);
                grid.DefaultCellStyle.BackColor = Color.White;
                grid.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                grid.DefaultCellStyle.SelectionForeColor = Color.Black;

                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightGray;

                grid.EnableHeadersVisualStyles = false;
                grid.RowHeadersVisible = false;
                grid.AllowUserToAddRows = false;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
            }
        }

        private void LoadData()
        {
            gridКлиенты.DataSource = DatabaseService.ПолучитьКлиентов();
            gridИмущество.DataSource = DatabaseService.ПолучитьИмущество();

            var клиенты = DatabaseService.ПолучитьКлиентов();
            var имущество = DatabaseService.ПолучитьИмущество();
            var полисы = DatabaseService.ПолучитьПолисы();

            var data = полисы.Select(p => new
            {
                p.Id,
                Клиент = клиенты.FirstOrDefault(c => c.Id == p.КлиентId)?.ФИО,
                Телефон = клиенты.FirstOrDefault(c => c.Id == p.КлиентId)?.Телефон,
                ЧтоСтрахуем = имущество.FirstOrDefault(i => i.Id == p.ИмуществоId)?.Тип,
                Адрес = имущество.FirstOrDefault(i => i.Id == p.ИмуществоId)?.Адрес,
                p.Премия,
                Начало = p.ДатаНачала.ToShortDateString(),
                Конец = p.ДатаОкончания.ToShortDateString()
            }).ToList();

            gridПолисы.DataSource = data;
        }

        private void AddClient()
        {
            var form = new InputForm("ФИО клиента:", "Телефон:");
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DatabaseService.ДобавитьКлиента(form.Input1.Trim(), form.Input2.Trim());
                    LoadData();
                    MessageBox.Show("Клиент успешно добавлен!", "Успех", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteClient()
        {
            if (gridКлиенты.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите клиента для удаления - щелкните по строке в таблице");
                return;
            }

            var id = (int)gridКлиенты.SelectedRows[0].Cells["Id"].Value;
            var фио = gridКлиенты.SelectedRows[0].Cells["ФИО"].Value.ToString();
            
            if (MessageBox.Show($"Удалить клиента: {фио}?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseService.УдалитьКлиента(id);
                LoadData();
                MessageBox.Show("Клиент удален!");
            }
        }

        private void AddProperty()
        {
            var form = new InputForm("Тип имущества:", "Стоимость:", "Адрес:");
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string тип = form.GetPropertyType();
                    decimal стоимость = decimal.Parse(form.Input1);
                    string адрес = form.Input2;

                    DatabaseService.ДобавитьИмущество(тип, стоимость, адрес);
                    LoadData();
                    MessageBox.Show("Имущество успешно добавлено!", "Успех", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении имущества: {ex.Message}", 
                                  "Ошибка", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteProperty()
        {
            if (gridИмущество.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите имущество для удаления - щелкните по строке в таблице");
                return;
            }

            var id = (int)gridИмущество.SelectedRows[0].Cells["Id"].Value;
            var тип = gridИмущество.SelectedRows[0].Cells["Тип"].Value.ToString();
            var адрес = gridИмущество.SelectedRows[0].Cells["Адрес"].Value.ToString();
            
            if (MessageBox.Show($"Удалить имущество: {тип} ({адрес})?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseService.УдалитьИмущество(id);
                LoadData();
                MessageBox.Show("Имущество удалено!");
            }
        }

        private void AddPolicy()
        {
            var клиенты = DatabaseService.ПолучитьКлиентов();
            var имущество = DatabaseService.ПолучитьИмущество();
            
            if (!клиенты.Any())
            {
                MessageBox.Show("Сначала добавьте клиента!");
                return;
            }
            if (!имущество.Any())
            {
                MessageBox.Show("Сначала добавьте имущество!");
                return;
            }
        
            try
            {
                var clientForm = new SelectForm("Выберите клиента:", клиенты.Select(c => c.ФИО).ToArray());
                if (clientForm.ShowDialog() != DialogResult.OK) return;
                var клиентId = клиенты[clientForm.SelectedIndex].Id;
        
                var propertyForm = new SelectForm("Выберите имущество:", 
                    имущество.Select(p => $"{p.Тип} ({p.Адрес}) - {p.Стоимость:C}").ToArray());
                if (propertyForm.ShowDialog() != DialogResult.OK) return;
                var имуществоId = имущество[propertyForm.SelectedIndex].Id;
        
                var form = new InputForm("Премия:", "Срок (дней):");
                if (form.ShowDialog() == DialogResult.OK)
                {
                    decimal премия = decimal.Parse(form.Input1);
                    int срок = int.Parse(form.Input2);
        
                    DatabaseService.ДобавитьПолис(клиентId, имуществоId, премия, DateTime.Today, DateTime.Today.AddDays(срок));
                    LoadData();
                    MessageBox.Show("Полис успешно добавлен!", "Успех");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении полиса: {ex.Message}", "Ошибка");
            }
        }

        private void DeletePolicy()
        {
            if (gridПолисы.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите полис для удаления - щелкните по строке в таблице");
                return;
            }

            var id = (int)gridПолисы.SelectedRows[0].Cells["Id"].Value;
            var клиент = gridПолисы.SelectedRows[0].Cells["Клиент"].Value.ToString();
            var имущество = gridПолисы.SelectedRows[0].Cells["ЧтоСтрахуем"].Value.ToString();
            
            if (MessageBox.Show($"Удалить полис: {клиент} - {имущество}?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseService.УдалитьПолис(id);
                LoadData();
                MessageBox.Show("Полис удален!");
            }
        }

        private void ViewBookings()
        {
            var bookingsForm = new BookingsForm();
            bookingsForm.ShowDialog();
        }

        private void ManageUsers()
        {
            var usersForm = new UsersForm();
            usersForm.ShowDialog();
        }
    }
}