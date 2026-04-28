namespace InsuranceApp.Models
{
    public class Клиент
    {
        public int Id { get; set; }
        public string ФИО { get; set; } = "";
        public string Телефон { get; set; } = "";
        public string Паспорт { get; set; } = "";
        public string Адрес { get; set; } = "";
    }
}