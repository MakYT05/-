using System;
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
            CreateDefaultAdmin();

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
                    Application.Run(new UserMenuForm());
                }
            }
        }

        static void CreateDefaultAdmin()
        {
            try
            {
                DatabaseService.ЗарегистрироватьПользователя("admin", "admin123", "Администратор", "admin");
            }
            catch{}
        }
    }
}