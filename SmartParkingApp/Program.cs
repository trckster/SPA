using System;
using System.Linq;
namespace ParkingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ParkingManager app = new ParkingManager();

            ParkingSession ps = app.EnterParking("Simple car");
            ps.EntryDt = ps.EntryDt.AddMinutes(-439);
            Console.WriteLine("Payment: {0}", app.GetRemainingCost(ps.TicketNumber));
            app.PayForParking(ps.TicketNumber, 10);
            ParkingSession ps2;
            Console.WriteLine("Hello blyab {0}", ps.TotalPayment);
            Console.WriteLine("Wanna go {0}", app.TryLeaveParkingWithTicket(ps.TicketNumber, out ps2));
        }
    }
}