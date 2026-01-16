namespace InsuranceApp.Models
{
    public class Заявка
    {
        public int Id { get; set; }
        public string ФИО { get; set; } = "";
        public string Телефон { get; set; } = "";
        public string ТипИмущества { get; set; } = "";
        public decimal Стоимость { get; set; }
        public string Адрес { get; set; } = "";
        public decimal Премия { get; set; }
        public int Срок { get; set; }
    }
}