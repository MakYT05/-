using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class BookingsForm : Form
    {
        private DataGridView gridBookings = new();
        private Button btnDelete = new();

        public BookingsForm()
        {
            Text = "Бронирования времени";
            Size = new Size(700, 400);
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
            LoadData();
        }

        private void InitializeControls()
        {
            gridBookings.Location = new Point(10, 10);
            gridBookings.Size = new Size(660, 300);
            gridBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBookings.ReadOnly = true;

            btnDelete.Text = "Удалить бронь";
            btnDelete.Location = new Point(10, 320);
            btnDelete.Size = new Size(120, 30);
            btnDelete.BackColor = Color.LightCoral;
            btnDelete.Click += (s, e) => DeleteBooking();

            Controls.Add(gridBookings);
            Controls.Add(btnDelete);
        }

        private void LoadData()
        {
            var bookings = DatabaseService.ПолучитьБронирования();
            gridBookings.DataSource = bookings;
            
            if (gridBookings.Columns.Count > 0)
            {
                gridBookings.Columns["Id"].HeaderText = "ID";
                gridBookings.Columns["ФИО"].HeaderText = "ФИО клиента";
                gridBookings.Columns["Телефон"].HeaderText = "Телефон";
                gridBookings.Columns["Дата"].HeaderText = "Дата визита";
                gridBookings.Columns["Время"].HeaderText = "Время визита";
                gridBookings.Columns["Статус"].HeaderText = "Статус";
            }
        }

        private void DeleteBooking()
        {
            if (gridBookings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите бронирование для удаления");
                return;
            }

            var id = (int)gridBookings.SelectedRows[0].Cells["Id"].Value;
            var фио = gridBookings.SelectedRows[0].Cells["ФИО"].Value.ToString();
            var дата = gridBookings.SelectedRows[0].Cells["Дата"].Value.ToString();
            var время = gridBookings.SelectedRows[0].Cells["Время"].Value.ToString();

            if (MessageBox.Show($"Удалить бронирование?\n\nКлиент: {фио}\nДата: {дата}\nВремя: {время}", 
                "Подтверждение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseService.УдалитьБронирование(id);
                LoadData();
                MessageBox.Show("Бронирование удалено!");
            }
        }
    }
}