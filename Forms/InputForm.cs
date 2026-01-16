using System;
using System.Drawing;
using System.Windows.Forms;

namespace InsuranceApp.Forms
{
    public class InputForm : Form
    {
        private TextBox tb1 = new();
        private TextBox tb2 = new();
        private TextBox tb3 = new();
        private ComboBox cbType = new();
        private Button btnOk = new();
        private Button btnCancel = new();
        private Button btnCases = new();

        public string Input1 => tb1.Text;
        public string Input2 => tb2.Text;
        public string Input3 => tb3.Text;

        private string _label1, _label2, _label3;
        private bool _isPropertyForm;

        public InputForm(string label1, string label2, string label3 = null)
        {
            _label1 = label1;
            _label2 = label2;
            _label3 = label3;
            _isPropertyForm = label1.Contains("Тип") && label3 != null;

            Text = "Ввод данных";
            Size = new Size(400, _isPropertyForm ? 300 : 250);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeControls();
        }

        private void InitializeControls()
        {
            int yPos = 20;

            if (_isPropertyForm)
            {
                var lblType = new Label
                {
                    Text = "Что страхуем:",
                    Location = new Point(20, yPos),
                    AutoSize = true,
                    Font = new Font("Arial", 9)
                };
                cbType.Location = new Point(150, yPos);
                cbType.Size = new Size(200, 20);
                cbType.DropDownStyle = ComboBoxStyle.DropDownList;
                cbType.Items.AddRange(new string[] { "Квартира", "Дом", "Автомобиль", "Гараж", "Офисное помещение" });
                Controls.Add(lblType);
                Controls.Add(cbType);

                yPos += 40;

                var lblCost = new Label
                {
                    Text = "Стоимость:",
                    Location = new Point(20, yPos),
                    AutoSize = true,
                    Font = new Font("Arial", 9)
                };
                tb1.Location = new Point(150, yPos);
                tb1.Size = new Size(200, 20);
                tb1.TextChanged += NumericField_TextChanged;
                Controls.Add(lblCost);
                Controls.Add(tb1);

                yPos += 40;

                var lblAddress = new Label
                {
                    Text = "Адрес:",
                    Location = new Point(20, yPos),
                    AutoSize = true,
                    Font = new Font("Arial", 9)
                };
                tb2.Location = new Point(150, yPos);
                tb2.Size = new Size(200, 20);
                Controls.Add(lblAddress);
                Controls.Add(tb2);
            }
            else
            {
                var lbl1 = new Label
                {
                    Text = _label1,
                    Location = new Point(20, yPos),
                    AutoSize = true,
                    Font = new Font("Arial", 9)
                };
                tb1.Location = new Point(150, yPos);
                tb1.Size = new Size(200, 20);
                Controls.Add(lbl1);
                Controls.Add(tb1);

                yPos += 40;

                var lbl2 = new Label
                {
                    Text = _label2,
                    Location = new Point(20, yPos),
                    AutoSize = true,
                    Font = new Font("Arial", 9)
                };
                tb2.Location = new Point(150, yPos);
                tb2.Size = new Size(200, 20);
                
                if (_label2.Contains("Стоимость") || _label2.Contains("Премия") || _label2.Contains("Срок"))
                {
                    tb2.TextChanged += NumericField_TextChanged;
                }
                
                Controls.Add(lbl2);
                Controls.Add(tb2);
            }

            yPos += 50;

            btnOk.Text = "ОК";
            btnOk.Location = new Point(50, yPos);
            btnOk.Size = new Size(80, 30);
            btnOk.BackColor = Color.LightGreen;
            btnOk.Click += (s, e) => 
            {
                if (ValidateInput())
                    DialogResult = DialogResult.OK;
            };

            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(150, yPos);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            btnCases.Text = "Страховые случаи";
            btnCases.Location = new Point(250, yPos);
            btnCases.Size = new Size(120, 30);
            btnCases.BackColor = Color.LightBlue;
            btnCases.Click += (s, e) => new CasesForm().ShowDialog();

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(btnCases);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void NumericField_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (!string.IsNullOrEmpty(textBox.Text) && !IsNumeric(textBox.Text))
            {
                textBox.BackColor = Color.LightPink;
            }
            else
            {
                textBox.BackColor = Color.White;
            }
        }

        private bool IsNumeric(string text)
        {
            return decimal.TryParse(text, out _) || int.TryParse(text, out _);
        }

        private bool ValidateInput()
        {
            if (_isPropertyForm) {
                if (cbType.SelectedIndex < 0)
                {
                    MessageBox.Show("Выберите тип имущества из списка");
                    cbType.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(tb1.Text))
                {
                    MessageBox.Show("Введите стоимость имущества");
                    tb1.Focus();
                    return false;
                }
                if (!decimal.TryParse(tb1.Text, out decimal cost) || cost <= 0)
                {
                    MessageBox.Show("Введите корректную положительную стоимость\nПример: 1000 или 1500.50");
                    tb1.Focus();
                    tb1.SelectAll();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(tb2.Text))
                {
                    MessageBox.Show("Введите адрес имущества");
                    tb2.Focus();
                    return false;
                }
            }
            else if (_label1.Contains("Премия"))
            {
                if (string.IsNullOrWhiteSpace(tb1.Text))
                {
                    MessageBox.Show("Введите премию");
                    tb1.Focus();
                    return false;
                }
                if (!decimal.TryParse(tb1.Text, out decimal premium) || premium <= 0)
                {
                    MessageBox.Show("Введите корректную положительную премию\nПример: 1000 или 1500.50");
                    tb1.Focus();
                    tb1.SelectAll();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(tb2.Text))
                {
                    MessageBox.Show("Введите срок в днях");
                    tb2.Focus();
                    return false;
                }
                if (!int.TryParse(tb2.Text, out int term) || term <= 0)
                {
                    MessageBox.Show("Введите корректный положительный срок в днях\nПример: 365");
                    tb2.Focus();
                    tb2.SelectAll();
                    return false;
                }
            }
            else {
                if (string.IsNullOrWhiteSpace(tb1.Text))
                {
                    MessageBox.Show("Введите ФИО клиента");
                    tb1.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(tb2.Text))
                {
                    MessageBox.Show("Введите телефон клиента");
                    tb2.Focus();
                    return false;
                }
            }

            return true;
        }

        public string GetPropertyType()
        {
            return cbType.SelectedItem?.ToString() ?? "";
        }
    }
}