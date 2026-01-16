using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class RequestsForm : Form
    {
        private DataGridView gridЗаявки = new();
        private Button btnAccept = new();
        private Button btnReject = new();

        public event Action ЗаявкаПринята;

        public RequestsForm()
        {
            Text = "Заявки";
            Size = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
            LoadData();
        }

        private void InitializeControls()
        {
            gridЗаявки.Location = new Point(10, 10);
            gridЗаявки.Size = new Size(560, 280);
            gridЗаявки.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridЗаявки.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridЗаявки.ReadOnly = true;

            btnAccept.Text = "Принять";
            btnAccept.Location = new Point(10, 310);
            btnAccept.Size = new Size(100, 30);
            btnAccept.BackColor = Color.LightGreen;
            btnAccept.Click += (s, e) => AcceptRequest();

            btnReject.Text = "Отклонить";
            btnReject.Location = new Point(120, 310);
            btnReject.Size = new Size(100, 30);
            btnReject.BackColor = Color.LightCoral;
            btnReject.Click += (s, e) => RejectRequest();

            Controls.Add(gridЗаявки);
            Controls.Add(btnAccept);
            Controls.Add(btnReject);
        }

        private void LoadData()
        {
            gridЗаявки.DataSource = DatabaseService.ПолучитьЗаявки();
        }

        private void AcceptRequest()
        {
            if (gridЗаявки.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заявку для принятия");
                return;
            }

            var id = (int)gridЗаявки.SelectedRows[0].Cells["Id"].Value;

            DatabaseService.ПринятьЗаявку(id);

            MessageBox.Show("Заявка принята!");

            ЗаявкаПринята?.Invoke();
            LoadData();
        }

        private void RejectRequest()
        {
            if (gridЗаявки.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заявку для отклонения");
                return;
            }

            var id = (int)gridЗаявки.SelectedRows[0].Cells["Id"].Value;

            DatabaseService.ОтклонитьЗаявку(id);

            MessageBox.Show("Заявка отклонена!");
            LoadData();
        }
    }
}