using System;

namespace InsuranceApp.Models
{
    public class Уведомление
    {
        public int Id { get; set; }
        public int ПользовательId { get; set; }
        public int ПолисId { get; set; }
        public string Тип { get; set; } = "";
        public string Сообщение { get; set; } = "";
        public DateTime ДатаСоздания { get; set; }
        public bool Прочитано { get; set; }
        public string ПутьКФайлу { get; set; } = "";
    }
}