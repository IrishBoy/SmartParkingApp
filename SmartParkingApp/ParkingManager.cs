using SmartParkingApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingApp
{
    class ParkingManager
    {

        /* BASIC PART */
        /* ALL CUSTOM METHODS AND VARIABLES ARE ABOVE GIVEN*/
        public const int Capacity = 150;


        private List<ParkingSession> ActiveSessions = new List<ParkingSession>();
        private List<ParkingSession> CompletedSessions = new List<ParkingSession>();
        private List<Tariff> Tariffs = new List<Tariff>();
        private List<User> Users = new List<User>();


        private int FreePeriod;


        public ParkingManager()
        {
            this.Tariffs = Files.Get_tariffs();
            this.Users = Files.Get_loyalty_user();

            this.FreePeriod = this.Tariffs.Min(tariff => tariff.Minutes);
        }


        private void CompleteSession(ParkingSession session)
        {
            this.ActiveSessions.Remove(session);
            this.CompletedSessions.Add(session);
        }

        public ParkingSession Get_session_by_ticket(int ticketNumber)
        {
            ParkingSession exist_cur_session = this.ActiveSessions.Find(cur_ses => cur_ses.TicketNumber == ticketNumber);

            return exist_cur_session;
        }

        public object Get_cur_session(string carPlateNumber)
        {
            object exist_session = ActiveSessions.Find(Currentsession => Currentsession.CarPlateNumber == carPlateNumber);
            return exist_session;
        }

        public User Get_cur_user(string carPlateNumber)
        {
            User Cur_user = Users.Find(Current_user => Current_user.CarPlateNumber == carPlateNumber);
            return Cur_user;
        }

        public ParkingSession Get_session_by_Plate_number(string Number)
        {
            ParkingSession Req_session = this.ActiveSessions.Find(s => s.CarPlateNumber == Number);
            return Req_session;

        }
        private int Get_new_ticket()
        {
            int New_ticket = ActiveSessions.Count + CompletedSessions.Count + 1;
            return New_ticket;
        }

        public Tariff Get_tariff(int Minutes)
        {
            Tariff Cur_tariff = this.Tariffs.Find(t => t.Minutes == Minutes);
            return Cur_tariff;
        }
        public ParkingSession EnterParking(string carPlateNumber)
        {

            int cur_sessinon_length = this.ActiveSessions.Count;
            object cur_ses_exist = Get_cur_session(carPlateNumber);
            DateTime Cur_date_entr = DateTime.Now;
            User Current_user = Get_cur_user(carPlateNumber);
            /* Check that there is a free parking place (by comparing the parking capacity 
             * with the number of active parking sessions). If there are no free places, return null
                */

            if (cur_sessinon_length >= ParkingManager.Capacity)
                return null;
            /* Also check that there are no existing active sessions with the same car plate number,
            * if such session exists, also return null
            */

            if (cur_ses_exist != null)
                return null;
            /* Otherwise:
            * Create a new Parking session, fill the following properties:
            * EntryDt = current date time
            * CarPlateNumber = carPlateNumber (from parameter)
            * TicketNumber = unused parking ticket number = generate this programmatically
            */
            ParkingSession New_session = new ParkingSession();

            New_session.EntryDt = Cur_date_entr;
            New_session.CarPlateNumber = carPlateNumber;
            New_session.TicketNumber = this.Get_new_ticket();
            New_session.User = Current_user;
            /* Add the newly created session to the list of active sessions
            */
            this.ActiveSessions.Add(New_session);
            /* Advanced task:
            * Link the new parking session to an existing user by car plate number (if such user exists)            
            */
            Files.Save_file(this);

            return New_session;


            throw new NotImplementedException();
        }



        public bool TryLeaveParkingWithTicket(int ticketNumber, out ParkingSession session)
        {
            /*
             * Check that the car leaves parking within the free leave period
             * from the PaymentDt (or if there was no payment made, from the EntryDt)
             * 1. If yes:
             *   1.1 Complete the parking session by setting the ExitDt property
             *   1.2 Move the session from the list of active sessions to the list of past sessions             * 
             *   1.3 return true and the completed parking session object in the out parameter
             * 
             * 2. Otherwise, return false, session = null
             */
            session = Get_session_by_ticket(ticketNumber);
            DateTime startTimer;


            if (session is null)
                return false;


            startTimer = session.PaymentDt == null ? session.EntryDt : (DateTime)session.PaymentDt;


            if (startTimer.AddMinutes(this.FreePeriod) < (DateTime.Now))
            {
                session = null;

                return false;
            }
            DateTime Exit_time = DateTime.Now;
            session.ExitDt = Exit_time;

            this.CompleteSession(session);

            Files.Save_file(this);

            return true;

            throw new NotImplementedException();
        }


        public decimal GetRemainingCost(int ticketNumber)
        {
            /* Return the amount to be paid for the parking
             * If a payment had already been made but additional charge was then given
             * because of a late exit, this method should return the amount 
             * that is yet to be paid (not the total charge)
             */
            ParkingSession session = Get_session_by_ticket(ticketNumber);

            DateTime Timer_start;


            Timer_start = session.PaymentDt == null ? session.EntryDt : (DateTime)session.PaymentDt;
            double Time_park = DateTime.Now.Subtract(Timer_start).TotalMinutes;

            return this.Get_price(Time_park);



            throw new NotImplementedException();
        }

        public void PayForParking(int ticketNumber, decimal amount)
        {
            /*
             * Save the payment details in the corresponding parking session
             * Set PaymentDt to current date and time
             * 
             * For simplicity we won't make any additional validation here and always
             * assume that the parking charge is paid in full
             */
            ParkingSession session = Get_session_by_ticket(ticketNumber);

            session.PaymentDt = DateTime.Now;

            session.TotalPayment = session.TotalPayment == null ? 0 : session.TotalPayment;

            session.TotalPayment += amount;

            Files.Save_file(this);
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

            session = Get_session_by_Plate_number(carPlateNumber);

            if (session is null)
                return false;

            if (session.PaymentDt is null)
            {
                if (session.User != null)
                {
                    session.TotalPayment = session.TotalPayment == null ? 0 : session.TotalPayment;
                    session.PaymentDt = DateTime.Now;

                    double Park_time = DateTime.Now.Subtract(session.EntryDt).TotalMinutes - this.FreePeriod;

                    decimal Add_payment = this.Get_price(Park_time);

                    session.TotalPayment += Add_payment;
                }
                else if (session.EntryDt.AddMinutes(this.FreePeriod) <= (DateTime.Now))
                {
                    session = null;
                }
                else
                    session.ExitDt = DateTime.Now;
            }
            else if (((DateTime)session.PaymentDt).AddMinutes(this.FreePeriod) < (DateTime.Now))
                session = null;

            if (session is null)
                return false;

            session.ExitDt = DateTime.Now;

            this.CompleteSession(session);

            return true;

            throw new NotImplementedException();
        }
        public decimal Get_price(double minutes)
        {
            Tariff result = null;

            foreach (var tariff in this.Tariffs)
            {
                result = ((minutes <= tariff.Minutes) && (result == null || tariff.Minutes < result.Minutes)) ? tariff : result;
            }

            if (result is null)
            {
                int maxMinutes = this.Tariffs.Max(t => t.Minutes);

                result = Get_tariff(maxMinutes);
            }

            return result.Rate;
        }


    }
}
