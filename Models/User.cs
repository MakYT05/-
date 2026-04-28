namespace InsuranceApp.Models
{
    public class Пользователь
    {
        public int Id { get; set; }
        public string Логин { get; set; } = "";
        public string Пароль { get; set; } = "";
        public string Роль { get; set; } = "user";
        public string ФИО { get; set; } = "";
        public string Телефон { get; set; } = "";
        public string Паспорт { get; set; } = "";
        public string Адрес { get; set; } = "";
    }
}