namespace InsuranceApp.Models
{
    public class Имущество
    {
        public int Id { get; set; }
        public string Тип { get; set; } = "";
        public decimal Стоимость { get; set; }
        public string Адрес { get; set; } = "";
    }
}