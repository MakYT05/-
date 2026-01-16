namespace InsuranceApp.Models
{
    public class Бронирование
    {
        public int Id { get; set; }
        public string ФИО { get; set; } = "";
        public string Телефон { get; set; } = "";
        public string Дата { get; set; } = "";
        public string Время { get; set; } = "";
        public string Статус { get; set; } = "активна";
    }
}