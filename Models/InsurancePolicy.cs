namespace InsuranceApp.Models
{
    public class Полис
    {
        public int Id { get; set; }
        public Клиент Клиент { get; set; } = null!;
        public Имущество Имущество { get; set; } = null!;
        public DateTime ДатаНачала { get; set; }
        public DateTime ДатаОкончания { get; set; }
        public decimal Премия { get; set; }
        
        public int КлиентId { get; internal set; }
        public int ИмуществоId { get; internal set; }
    }
}