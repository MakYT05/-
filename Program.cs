using System;
using System.Linq;
using System.Windows.Forms;
using InsuranceApp.Forms;
using InsuranceApp.Services;

namespace InsuranceApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            DatabaseService.Initialize();
            DatabaseService.ОбновитьВсеТаблицы();
            DatabaseService.ОбновитьТаблицуПользователей();
            DatabaseService.ОбновитьДанныеКлиентов();
            CreateDefaultAdmin();
            FixUserPhones();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                if (loginForm.РольПользователя == "admin")
                {
                    Application.Run(new MainForm());
                }
                else
                {
                    Application.Run(new UserMenuForm(
                        loginForm.ЛогинПользователя,
                        loginForm.ФИОПользователя,
                        loginForm.РольПользователя,
                        loginForm.ТелефонПользователя
                    ));
                }
            }
        }

        static void CreateDefaultAdmin()
        {
            try
            {
                var users = DatabaseService.GetUsers();
                if (!users.Any(u => u.Логин == "admin"))
                {
                    DatabaseService.ЗарегистрироватьПользователя(
                        "admin", 
                        "admin123", 
                        "Администратор", 
                        "admin-phone", 
                        "",
                        "",
                        "admin"
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания админа: {ex.Message}");
            }
        }

        static void FixUserPhones()
        {
            try
            {
                var users = DatabaseService.GetUsers();
                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.Телефон))
                    {
                        DatabaseService.ОбновитьТелефонПользователя(user.Логин, "phone-" + user.Логин);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления телефонов: {ex.Message}");
            }
        }
    }
}