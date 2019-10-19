namespace SmartParkingApp

{
    public class Tariff
    {
        public int Minutes { get; set; }
        public decimal Rate { get; set; }

        public Tariff(int minutes, decimal rate)
        {
            this.Minutes = minutes;
            this.Rate = rate;
        }
    }
}
