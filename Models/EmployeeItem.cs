namespace BreweryApp.Models
{
    public class EmployeeItem
    {
        public int КодСотрудника { get; set; }
        public string ФИО { get; set; } = string.Empty;
        public string Должность { get; set; } = string.Empty;

        public string Display
        {
            get
            {
                return $"{ФИО} ({Должность})";
            }
        }
    }
}