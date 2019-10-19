using SmartParkingApp;
using System;


namespace ParkingApp
{
    [Serializable]
    class Program
    {
        static void Main(string[] args)
        {

            ParkingManager Current_session = null;

            if (Files.Is_saved() == true)
            {
                Console.WriteLine(@"Previous session is stored,
                                    do you want to continue?
                                    Enter 'y' if yes");
                string continue_asnwer = Console.ReadLine();

                Current_session = continue_asnwer == "y" ? (ParkingManager)Files.Reload_session() : Current_session;
            }
            Current_session = Current_session == null ? new ParkingManager() : Current_session;

            Current_session = Current_session == null ? new ParkingManager() : Current_session;

            string Main_menu = (@"1 - Choose test scenario
                                  2 - Exit");
            while (true)
            {
                Console.WriteLine(Main_menu);
                string User_input = Console.ReadLine();
                switch (User_input)
                {
                    case "1":
                        string Scenario_menu = (@"Scenarios:
                                                  1 - Scenario 1 from task
                                                  2 - Scenario 2 from task
                                                  3 - Scenario 3 from task
                                                  4 - User leaves parking within 15 minutes
                                                  5 - No free places
                                                  6 - Same Plate Number");
                        string[] Cars =
                                 {
                               "car1", "car2", "car3", "car4", "car5", "car6",
                               "car7", "car8", "car9", "car10", "car11", "car12"
                                 };
                        int[] Add_hours =
                        {
                            -2, 0, -1, 0
                        };
                        int[] Add_mnutes =
                        {
                            0, -10, -30, -5
                        };
                        while (true)
                        {
                            Console.WriteLine(Scenario_menu);
                            string Scenario_choice = Console.ReadLine();
                            switch (Scenario_choice)
                            {
                                case "1":
                                    ParkingSession session_1 = Current_session.EnterParking(Cars[0]);
                                    session_1.EntryDt = session_1.EntryDt.AddHours(Add_hours[0]);
                                    Console.WriteLine("Scenario 1");
                                    Console.WriteLine($"Remaining cost : {Current_session.GetRemainingCost(session_1.TicketNumber)}");
                                    Current_session.PayForParking(session_1.TicketNumber, Current_session.GetRemainingCost(session_1.TicketNumber));
                                    Console.WriteLine($"Exiting: {Current_session.TryLeaveParkingWithTicket(session_1.TicketNumber, out session_1)}");
                                    Console.WriteLine("---End of the scenario 1---");
                                    break;
                                case "2":
                                    ParkingSession session_2 = Current_session.EnterParking(Cars[1]);
                                    session_2.EntryDt = session_2.EntryDt.AddMinutes(Add_mnutes[1]);
                                    Console.WriteLine("Scenario 2");
                                    Console.WriteLine($"Remaining cost: { Current_session.GetRemainingCost(session_2.TicketNumber)}");
                                    Console.WriteLine($"Exiting: {Current_session.TryLeaveParkingWithTicket(session_2.TicketNumber, out session_2)}");
                                    Console.WriteLine("---End of the scenario 2---");
                                    break;
                                case "3":
                                    ParkingSession session_3 = Current_session.EnterParking(Cars[2]);
                                    session_3.EntryDt = session_3.EntryDt.AddHours(Add_hours[2]);
                                    Console.WriteLine("Scenario 3");
                                    Console.WriteLine($"Remaining cost: {Current_session.GetRemainingCost(session_3.TicketNumber)}");
                                    Current_session.PayForParking(session_3.TicketNumber, Current_session.GetRemainingCost(session_3.TicketNumber));
                                    DateTime newPaymentDt = (DateTime)session_3.PaymentDt;
                                    session_3.PaymentDt = newPaymentDt.AddMinutes(Add_mnutes[2]);
                                    Console.WriteLine($"Exiting: {Current_session.TryLeaveParkingWithTicket(session_3.TicketNumber, out session_3)}");
                                    Console.WriteLine($"Remaining cost: {Current_session.GetRemainingCost(session_3.TicketNumber)}");
                                    Current_session.PayForParking(session_3.TicketNumber, Current_session.GetRemainingCost(session_3.TicketNumber));
                                    Console.WriteLine($"Exiting: {Current_session.TryLeaveParkingWithTicket(session_3.TicketNumber, out session_3)}");
                                    Console.WriteLine("---End of the scenario 3---");
                                    break;
                                case "4":
                                    ParkingSession session_4 = Current_session.EnterParking(Cars[3]);
                                    session_4.EntryDt = session_4.EntryDt.AddMinutes(Add_mnutes[3]);
                                    Console.WriteLine("Scenario 4");
                                    Console.WriteLine($"Exiting: {Current_session.TryLeaveParkingByCarPlateNumber(Cars[5], out session_4)}");
                                    Console.WriteLine("---End of the scenario 4---");
                                    break;
                                case "5":
                                    Console.WriteLine("Scenario 5");
                                    ParkingManager newParking = new ParkingManager();
                                    for (int i = 0; i < ParkingManager.Capacity; i++)
                                    {
                                        ParkingSession session = newParking.EnterParking(Cars[4] + i);
                                        if (session == null)
                                            Console.WriteLine($"Parking cannot accommodate {ParkingManager.Capacity} car");
                                    }
                                    string result_5 = (newParking.EnterParking(Cars[4]) == null ? "null" : "not null");
                                    Console.WriteLine($"{ParkingManager.Capacity + 1} car entering the parking: {result_5}");
                                    Console.WriteLine("---End of the scenario 5---");
                                    break;
                                case "6":
                                    Console.WriteLine("Scenario 6");
                                    Current_session.EnterParking(Cars[5]);
                                    string result_6 = Current_session.EnterParking(Cars[5]) == null ? "null" : "not null";
                                    Console.WriteLine($"Car with the same plate number can't access parking: {result_6}");
                                    Console.WriteLine();
                                    break;
                                default:
                                    Console.WriteLine("There is no such an option, try one more times");
                                    break;
                            }
                        }
                        break;
                    case "2":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("There is no such an option, try one more time");
                        break;
                }

            }
        }
    }
}

