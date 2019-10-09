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

        private List<ParkingSession> ActiveParkingSessions = new List<ParkingSession>();
        private List<ParkingSession> CompletedParkingSessions = new List<ParkingSession>();
        private List<Tariff> Tariffs = new List<Tariff>();
        private List<User> Users = new List<User>();

        private int FreeLeavePeriod;
        
        public ParkingManager()
        {
            this.SetTariffsData();
            this.LoadUsers();
            
            this.FreeLeavePeriod = this.Tariffs.Min(t => t.Minutes);
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
            newSession.User = this.Users.Find(user => user.CarPlateNumber == carPlateNumber);

            this.ActiveParkingSessions.Add(newSession);

            this.Save();

            return newSession;
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

            this.CompleteSession(session);

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
        
        public bool TryLeaveParkingByCarPlateNumber(string carPlateNumber, out ParkingSession session)
        {
            session = null;
            
            foreach (var activeSession in this.ActiveParkingSessions)
            {
                if (activeSession.User != null && ((User) activeSession.User).CarPlateNumber == carPlateNumber)
                {
                    session = activeSession;
                    break;
                }
            }

            if (session == null)
                return false;
            
            if (session.PaymentDt == null)
            {
                if (session.EntryDt.AddMinutes(this.FreeLeavePeriod).CompareTo(DateTime.Now) > 0)
                    session.ExitDt = DateTime.Now;
                else if (session.User != null)
                {
                    session.PaymentDt = DateTime.Now;
                    
                    session.TotalPayment += this.GetPriceByMinutes(DateTime.Now.Subtract(session.EntryDt).TotalMinutes - this.FreeLeavePeriod);
                }
                else
                    session = null;
            }
            else if (((DateTime)session.PaymentDt).AddMinutes(this.FreeLeavePeriod).CompareTo(DateTime.Now) < 0)
                session = null;

            if (session == null)
                return false;

            session.ExitDt = DateTime.Now;

            this.CompleteSession(session);

            return true;
        }

        private int GetNextTicketNumber()
        {
            return this.ActiveParkingSessions.Count + this.CompletedParkingSessions.Count + 1;
        }

        private void SetTariffsData()
        {
            this.Tariffs.Add(new Tariff(15, 0));
            this.Tariffs.Add(new Tariff(60, 50));
            this.Tariffs.Add(new Tariff(120, 100));
            this.Tariffs.Add(new Tariff(180, 200));
            this.Tariffs.Add(new Tariff(240, 400));
            this.Tariffs.Add(new Tariff(300, 800));
            this.Tariffs.Add(new Tariff(450, 1600));
            this.Tariffs.Add(new Tariff(600, 3200));
        }

        private decimal GetPriceByMinutes(double minutes)
        {
            Tariff result = null;

            foreach (var tariff in this.Tariffs)
            {
                if (minutes <= tariff.Minutes)
                    if (result == null || tariff.Minutes < result.Minutes)
                        result = tariff;
            }

            if (result == null)
            {
                int maxMinutes = this.Tariffs.Max(t => t.Minutes);
                
                result = this.Tariffs.Find(t => t.Minutes == maxMinutes);
            }

            return result.Rate;
        }

        private void Save()
        {
            FileLoader.SaveObject(this);
        }

        private void LoadUsers()
        {
            /** Change it suka */
            this.Users = FileLoader.FunnyName();
        }

        private void CompleteSession(ParkingSession session)
        {
            this.ActiveParkingSessions.Remove(session);
            this.CompletedParkingSessions.Add(session);
        }
    }
}
