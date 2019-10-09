using System;
using System.Linq;
using SmartParkingApp;

namespace ParkingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ParkingManager app = null;
            
            if (FileLoader.HasSave())
            {
                Console.WriteLine("Save file found.");
                Console.WriteLine("Continue previous session? (enter 'no' for refusal)");
                string answer = Console.ReadLine();
                
                if (answer != "no")
                    app = (ParkingManager) FileLoader.RestoreObject();
            }

            Console.WriteLine();
            Console.WriteLine("--------  Test zone --------");
            Console.WriteLine();

            if (app == null)
                app = new ParkingManager();

            string[] carPlates =
            {
                "scenario1", "scenario2", "scenario3", "scenario4", "scenario5", "scenario6",
                "scenario7", "scenario8", "scenario9", "scenario10", "scenario11", "scenario12",
            };

            /** Scenario 1 */
            Console.WriteLine("Scenario 1");
            ParkingSession ps = app.EnterParking(carPlates[0]);
            ps.EntryDt = ps.EntryDt.AddHours(-1);
            Console.WriteLine("Remaining cost (must be 100): {0}", app.GetRemainingCost(ps.TicketNumber));
            app.PayForParking(ps.TicketNumber, app.GetRemainingCost(ps.TicketNumber));
            Console.WriteLine("Try to leave parking (must be True): {0}", app.TryLeaveParkingWithTicket(ps.TicketNumber, out ps));
            Console.WriteLine();
            
            /** Scenario 2 */
            Console.WriteLine("Scenario 2");
            ParkingSession ps2 = app.EnterParking(carPlates[1]);
            ps2.EntryDt = ps2.EntryDt.AddMinutes(-10);
            Console.WriteLine("Remaining cost (must be 0): {0}", app.GetRemainingCost(ps2.TicketNumber));
            Console.WriteLine("Try to leave parking (must be True): {0}", app.TryLeaveParkingWithTicket(ps2.TicketNumber, out ps));
            Console.WriteLine();
            
            /** Scenario 3 */
            Console.WriteLine("Scenario 3");
            ParkingSession ps3 = app.EnterParking(carPlates[2]);
            ps3.EntryDt = ps3.EntryDt.AddHours(-1);
            Console.WriteLine("Remaining cost (must be 100): {0}", app.GetRemainingCost(ps3.TicketNumber));
            app.PayForParking(ps3.TicketNumber, app.GetRemainingCost(ps3.TicketNumber));
            DateTime newPaymentDt = (DateTime) ps3.PaymentDt;
            ps3.PaymentDt = newPaymentDt.AddMinutes(-30);
            Console.WriteLine("Try to leave parking (must be False): {0}", app.TryLeaveParkingWithTicket(ps3.TicketNumber, out ps));
            Console.WriteLine("Remaining cost (must be 50): {0}", app.GetRemainingCost(ps3.TicketNumber));
            app.PayForParking(ps3.TicketNumber, app.GetRemainingCost(ps3.TicketNumber));
            Console.WriteLine("Try to leave parking (must be True): {0}", app.TryLeaveParkingWithTicket(ps3.TicketNumber, out ps));
            Console.WriteLine();
            
            /**
             * Scenario 4
             * 
             * Check that parking can't be overflowed.
             */
            Console.WriteLine("Scenario 4");
            ParkingManager newParking = new ParkingManager();
            for (int i = 0; i < 450; i++)
            {
                ParkingSession session = newParking.EnterParking(carPlates[3] + i);
                if (session == null)
                    Console.WriteLine("450 cars can't fit in parking. Error.");
            }
            Console.WriteLine("451's car tries to enter the parking (parking session must be null): {0}",
                newParking.EnterParking(carPlates[3]) == null ? "null" : "not null");
            Console.WriteLine();
            
            /**
             * Scenario 5
             *
             * Check that car with repeating plate number cant't access parking
             */
            Console.WriteLine("Scenario 5");
            app.EnterParking("scenario5");
            Console.WriteLine("Car with the same plate number can't access parking (parking session must be null): {0}",
                app.EnterParking(carPlates[4]) == null ? "null" : "not null");
            Console.WriteLine();
            
            /**
             * Scenario 6
             *
             * TryLeaveParkingByCarPlateNumber 1
             * (The user has not made any payments and leaves parking)
             */
            Console.WriteLine("Scenario 6");
            ParkingSession ps6 = app.EnterParking(carPlates[5]);
            ps6.EntryDt = ps6.EntryDt.AddMinutes(-5);
            Console.WriteLine("Try to leave parking by plate (must be True): {0}", 
                app.TryLeaveParkingByCarPlateNumber(carPlates[5], out ps6));
            Console.WriteLine();
            
            /**
             * Scenario 7
             *
             * TryLeaveParkingByCarPlateNumber 2.1
             * (The user has already paid for the parking session and leaves within time)
             */
            Console.WriteLine("Scenario 7");
            ParkingSession ps7 = app.EnterParking(carPlates[6]);
            ps7.EntryDt = ps7.EntryDt.AddHours(-1);
            app.PayForParking(ps7.TicketNumber, app.GetRemainingCost(ps7.TicketNumber));
            ps7.PaymentDt = ((DateTime) ps7.PaymentDt).AddMinutes(-12);
            Console.WriteLine("Try to leave parking by plate number after payment (must be True): {0}", 
                app.TryLeaveParkingByCarPlateNumber(carPlates[6], out ps7));
            Console.WriteLine();
            
            /**
             * Scenario 8
             *
             * TryLeaveParkingByCarPlateNumber 2.2
             * (The user has already paid for the parking session and leaves after time's up)
            */
            Console.WriteLine("Scenario 8");
            ParkingSession ps8 = app.EnterParking(carPlates[7]);
            ps8.EntryDt = ps8.EntryDt.AddHours(-1);
            app.PayForParking(ps8.TicketNumber, app.GetRemainingCost(ps8.TicketNumber));
            ps8.PaymentDt = ((DateTime) ps8.PaymentDt).AddMinutes(-17);
            Console.WriteLine("Try to leave parking by plate number after payment (must be False): {0}", 
                app.TryLeaveParkingByCarPlateNumber(carPlates[7], out ps8));
            Console.WriteLine();
            
            /**
             * Scenario 9
             *
             * TryLeaveParkingByCarPlateNumber 3a
             * (The user has not made any payments and leaves parking)
             */
            Console.WriteLine("Scenario 9");
            ParkingSession ps9 = app.EnterParking(carPlates[8]);
            Console.WriteLine("Parking session must be linked with User (must be not null): {0}",
                ps9.User == null ? "null" : "not null");
            ps9.EntryDt = ps9.EntryDt.AddMinutes(-199);
            Console.WriteLine("Can leave parking, payment debited itself (must be True): {0}",
                app.TryLeaveParkingByCarPlateNumber(carPlates[8], out ps9));
            Console.WriteLine("Parking session total payment (must be 400): {0}", ps9.TotalPayment);
            Console.WriteLine();
            
            /**
             * Scenario 10
             *
             * TryLeaveParkingByCarPlateNumber 3a IMPORTANT
             * (The user has not made any payments and leaves parking x2)
             */
            Console.WriteLine("Scenario 10");
            ParkingSession ps10 = app.EnterParking(carPlates[9]);
            Console.WriteLine("Parking session must be linked with User (must be not null): {0}",
                ps10.User == null ? "null" : "not null");
            ps10.EntryDt = ps10.EntryDt.AddMinutes(-65);
            Console.WriteLine("Can leave parking, payment debited itself (must be True): {0}",
                app.TryLeaveParkingByCarPlateNumber(carPlates[9], out ps10));
            Console.WriteLine("Parking session total payment (must be 50): {0}", ps10.TotalPayment);
            Console.WriteLine();
            
            /**
             * Scenario 11
             *
             * TryLeaveParkingByCarPlateNumber 3b
             * (No connected user, no payments, time's up)
             */
            Console.WriteLine("Scenario 11");
            ParkingSession ps11 = app.EnterParking(carPlates[10]);
            Console.WriteLine("Parking session must not be linked with User (must be null): {0}",
                ps11.User == null ? "null" : "not null");
            ps11.EntryDt = ps11.EntryDt.AddMinutes(-199);
            Console.WriteLine("Can't leave parking (must be False): {0}",
                app.TryLeaveParkingByCarPlateNumber(carPlates[10], out ps11));
            Console.WriteLine();

        }
    }
}