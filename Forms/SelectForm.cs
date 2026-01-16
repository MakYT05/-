using System;
using System.Drawing;
using System.Windows.Forms;

namespace InsuranceApp.Forms
{
    public class SelectForm : Form
    {
        private ListBox listBox = new();
        private Button btnOk = new();
        private Button btnCancel = new();

        public int SelectedIndex => listBox.SelectedIndex;

        public SelectForm(string title, string[] items)
        {
            Text = title;
            Size = new Size(300, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var label = new Label
            {
                Text = title,
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            listBox.Location = new Point(10, 40);
            listBox.Size = new Size(260, 180);
            listBox.Items.AddRange(items);

            btnOk.Text = "Выбрать";
            btnOk.Location = new Point(60, 230);
            btnOk.Size = new Size(80, 30);
            btnOk.BackColor = Color.LightGreen;
            btnOk.Click += (s, e) =>
            {
                if (listBox.SelectedIndex >= 0)
                    DialogResult = DialogResult.OK;
                else
                    MessageBox.Show("Выберите элемент из списка");
            };

            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(150, 230);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { label, listBox, btnOk, btnCancel });

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}