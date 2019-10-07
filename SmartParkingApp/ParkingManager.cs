using System;
using System.Collections.Generic;
using System.Linq;
using SmartParkingApp;

namespace ParkingApp
{
    [Serializable]
    class ParkingManager
    {
        private const int ParkingCapacity = 450;

        private List<ParkingSession> ActiveParkingSessions;
        private List<ParkingSession> CompletedParkingSessions;
        private List<Tariff> Tariff;

        private FileLoader fl;
        private int FreeLeavePeriod;
        

        public ParkingManager()
        {
            this.ActiveParkingSessions = new List<ParkingSession>();
            this.CompletedParkingSessions = new List<ParkingSession>();
            this.Tariff = new List<Tariff>();

            this.SetTariffData();
            
            this.FreeLeavePeriod = this.Tariff.First().Minutes;
            this.fl = new FileLoader();
        }
        
        public ParkingSession EnterParking(string carPlateNumber)
        {
            if (ParkingManager.ParkingCapacity <= this.ActiveParkingSessions.Count)
                return null;

            if (this.ActiveParkingSessions.Find(session => session.CarPlateNumber.Equals(carPlateNumber)) != null)
                return null;

            ParkingSession newSession = new ParkingSession();
            newSession.EntryDt = DateTime.Now;
            newSession.CarPlateNumber = carPlateNumber;
            newSession.TicketNumber = this.GetNextTicketNumber();

            this.ActiveParkingSessions.Add(newSession);

            this.Save();

            return newSession;
                 /**
                 * Advanced task:
                 * Link the new parking session to an existing user by car plate number (if such user exists)            
                 */
        }
        public bool TryLeaveParkingWithTicket(int ticketNumber, out ParkingSession session)
        {
            session = this.ActiveParkingSessions.Find(s => s.TicketNumber == ticketNumber);

            if (session == null)
                return false;

            DateTime startTimer;

            if (session.PaymentDt == null)
                startTimer = session.EntryDt;
            else
                startTimer = (DateTime) session.PaymentDt;

            if (startTimer.AddMinutes(this.FreeLeavePeriod).CompareTo(DateTime.Now) < 0)
            {
                session = null;
                
                return false;
            }

            session.ExitDt = DateTime.Now;
            
            this.ActiveParkingSessions.Remove(session);
            this.CompletedParkingSessions.Add(session);

            this.Save();

            return true;
        }        

        public decimal GetRemainingCost(int ticketNumber)
        {
            ParkingSession session = this.ActiveParkingSessions.Find(s => s.TicketNumber == ticketNumber);

            DateTime startTimerDt;

            if (session.PaymentDt != null)
                startTimerDt = (DateTime) session.PaymentDt;
            else
                startTimerDt = session.EntryDt;

            double spentTime = DateTime.Now.Subtract(startTimerDt).TotalMinutes;
            
            return this.GetPriceByMinutes(spentTime);
        }

        public void PayForParking(int ticketNumber, decimal amount)
        {
            ParkingSession session = this.ActiveParkingSessions.Find(s => s.TicketNumber == ticketNumber);
            
            session.PaymentDt = DateTime.Now;

            if (session.TotalPayment == null)
                session.TotalPayment = 0;

            session.TotalPayment += amount;

            this.Save();
        }

        /* ADDITIONAL TASK 2 */
        public bool TryLeaveParkingByCarPlateNumber(string carPlateNumber, out ParkingSession session)
        {
            /* There are 3 scenarios for this method:
            
            1. The user has not made any payments but leaves the parking within the free leave period
            from EntryDt:
               1.1 Complete the parking session by setting the ExitDt property
               1.2 Move the session from the list of active sessions to the list of past sessions             * 
               1.3 return true and the completed parking session object in the out parameter
            
            2. The user has already paid for the parking session (session.PaymentDt != null):
            Check that the current time is within the free leave period from session.PaymentDt
               2.1. If yes, complete the session in the same way as in the previous scenario
               2.2. If no, return false, session = null

            3. The user has not paid for the parking session:            
            3a) If the session has a connected user (see advanced task from the EnterParking method):
            ExitDt = PaymentDt = current date time; 
            TotalPayment according to the tariff table:            
            
            IMPORTANT: before calculating the parking charge, subtract FreeLeavePeriod 
            from the total number of minutes passed since entry
            i.e. if the registered visitor enters the parking at 10:05
            and attempts to leave at 10:25, no charge should be made, otherwise it would be unfair
            to loyal customers, because an ordinary printed ticket could be inserted in the payment
            kiosk at 10:15 (no charge) and another 15 free minutes would be given (up to 10:30)

            return the completed session in the out parameter and true in the main return value

            3b) If there is no connected user, set session = null, return false (the visitor
            has to insert the parking ticket and pay at the kiosk)
            */
            throw new NotImplementedException();
        }

        private int GetNextTicketNumber()
        {
            return this.ActiveParkingSessions.Count + this.CompletedParkingSessions.Count + 1;
        }

        private void SetTariffData()
        {
            this.Tariff.Add(new Tariff(15, 0));
            this.Tariff.Add(new Tariff(60, 50));
            this.Tariff.Add(new Tariff(120, 100));
            this.Tariff.Add(new Tariff(180, 200));
            this.Tariff.Add(new Tariff(240, 400));
            this.Tariff.Add(new Tariff(300, 800));
            this.Tariff.Add(new Tariff(450, 1600));
            this.Tariff.Add(new Tariff(600, 3200));
        }

        private decimal GetPriceByMinutes(double minutes)
        {
            Tariff result = null;

            foreach (var tariff in this.Tariff)
            {
                if (minutes <= tariff.Minutes)
                    if (result == null || tariff.Minutes < result.Minutes)
                        result = tariff;
            }

            if (result == null)
            {
                int maxMinutes = this.Tariff.Max(t => t.Minutes);
                
                result = this.Tariff.Find(t => t.Minutes == maxMinutes);
            }

            return result.Rate;
        }

        private void Save()
        {
            this.fl.SaveObject(this);
        }
    }
}
