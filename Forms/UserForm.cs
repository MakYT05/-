using System;
using System.Drawing;
using System.Windows.Forms;
using InsuranceApp.Models;
using InsuranceApp.Services;

namespace InsuranceApp.Forms
{
    public class UserForm : Form
    {
        private TextBox tbFio = new();
        private TextBox tbTel = new();
        private TextBox tbTip = new();
        private TextBox tbStoim = new();
        private TextBox tbAdr = new();
        private TextBox tbPrem = new();
        private TextBox tbSrok = new();
        private Button btnSubmit = new();

        public UserForm()
        {
            Text = "Заявка на страхование";
            Size = new Size(400, 400);
            StartPosition = FormStartPosition.CenterParent;

            var labels = new[] { "ФИО", "Телефон", "Тип имущества", "Стоимость", "Адрес", "Премия", "Срок (дней)" };
            var boxes = new[] { tbFio, tbTel, tbTip, tbStoim, tbAdr, tbPrem, tbSrok };

            for (int i = 0; i < labels.Length; i++)
            {
                var lbl = new Label { Text = labels[i], Location = new Point(20, 20 + i * 40), AutoSize = true };
                var tb = boxes[i];
                tb.Location = new Point(150, 20 + i * 40);
                tb.Width = 200;
                Controls.Add(lbl);
                Controls.Add(tb);
            }

            btnSubmit.Text = "Подать заявку";
            btnSubmit.Location = new Point(120, 320);
            btnSubmit.Click += (s, e) =>
            {
                var z = new Заявка
                {
                    ФИО = tbFio.Text,
                    Телефон = tbTel.Text,
                    ТипИмущества = tbTip.Text,
                    Стоимость = decimal.Parse(tbStoim.Text),
                    Адрес = tbAdr.Text,
                    Премия = decimal.Parse(tbPrem.Text),
                    Срок = int.Parse(tbSrok.Text)
                };
                DatabaseService.ДобавитьЗаявку(z);
                MessageBox.Show("Заявка подана!");
                Close();
            };
            Controls.Add(btnSubmit);
        }
    }
}