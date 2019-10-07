using System;
using System.Linq;
namespace ParkingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (FileLoader.HasSave())
            {
                Console.ReadLine("kek");
            }
            ParkingManager app = new ParkingMaclassnager();

            /** Scenario 1 */
            Console.WriteLine("Scenario 1");
            ParkingSession ps = app.EnterParking("scenario1");
            ps.EntryDt = ps.EntryDt.AddHours(-1);
            Console.WriteLine("Remaining cost (must be 100): {0}", app.GetRemainingCost(ps.TicketNumber));
            app.PayForParking(ps.TicketNumber, 100);
            Console.WriteLine("Try to leave parking (must be True): {0}", app.TryLeaveParkingWithTicket(ps.TicketNumber, out ps));
            Console.WriteLine();
            
            /** Scenario 2 */
            Console.WriteLine("Scenario 2");
            ParkingSession ps2 = app.EnterParking("scenario2");
            ps2.EntryDt = ps2.EntryDt.AddMinutes(-10);
            Console.WriteLine("Remaining cost (must be 0): {0}", app.GetRemainingCost(ps2.TicketNumber));
            Console.WriteLine("Try to leave parking (must be True): {0}", app.TryLeaveParkingWithTicket(ps2.TicketNumber, out ps));
            Console.WriteLine();
            
            /** Scenario 3 */
            Console.WriteLine("Scenario 3");
            ParkingSession ps3 = app.EnterParking("scenario3");
            ps3.EntryDt = ps3.EntryDt.AddHours(-1);
            Console.WriteLine("Remaining cost (must be 100): {0}", app.GetRemainingCost(ps3.TicketNumber));
            app.PayForParking(ps3.TicketNumber, 100);
            DateTime newPaymentDt = (DateTime) ps3.PaymentDt;
            ps3.PaymentDt = newPaymentDt.AddMinutes(-30);
            Console.WriteLine("Try to leave parking (must be False): {0}", app.TryLeaveParkingWithTicket(ps3.TicketNumber, out ps));
            Console.WriteLine("Remaining cost (must be 50): {0}", app.GetRemainingCost(ps3.TicketNumber));
            app.PayForParking(ps3.TicketNumber, 50);
            Console.WriteLine("Try to leave parking (must be True): {0}", app.TryLeaveParkingWithTicket(ps3.TicketNumber, out ps));
            Console.WriteLine();
        }
    }
}