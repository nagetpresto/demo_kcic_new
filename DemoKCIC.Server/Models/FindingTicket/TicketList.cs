namespace DemoKCIC.Server.Models.FindingTicket
{
    public class TicketList
    {
        public int ID { get; set; }
        public string TrainNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Class { get; set; }
        public decimal Price { get; set; }
    }

    public class MasterWilayah
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
