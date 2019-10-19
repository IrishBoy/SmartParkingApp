using ParkingApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmartParkingApp
{
    [Serializable]
    public static class Files
    {
        private const string Tariffs_file_name = "..//..//..//tarrif.txt";
        private const string Loyalty_program = "..//..//..//loyality_program.txt";
        private const string Parking_sessions = "..//..//..//parking_sessions.txt";


        public static List<Tariff> Get_tariffs()
        {
            List<Tariff> Tariffs_info = new List<Tariff>();
            try
            {
                using (StreamReader tariffs = new StreamReader(Tariffs_file_name, System.Text.Encoding.Default))
                {
                    string row;
                    while ((row = tariffs.ReadLine()) != null)
                    {
                        string[] cells = row.Split(';');
                        Tariffs_info.Add(new Tariff(int.Parse(cells[0]), decimal.Parse(cells[1])));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Tariffs_info;
        }

        public static List<User> Get_loyalty_user()
        {
            List<User> loyalty_users = new List<User>();
            try
            {
                using (StreamReader users = new StreamReader(Loyalty_program, System.Text.Encoding.Default))
                {
                    string row;
                    while ((row = users.ReadLine()) != null)
                    {
                        string[] cells = row.Split(';');
                        loyalty_users.Add(new User()
                        {
                            Name = cells[0],
                            CarPlateNumber = cells[1],
                            Phone = cells[2]
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return loyalty_users;
        }

        public static bool Is_saved()
        {
            bool existance = File.Exists(Parking_sessions);
            return existance;
        }

        public static void Save_file(object saving_file)
        {
            try
            {
                using (Stream saving = File.Open(Parking_sessions, FileMode.Create))
                {

                    BinaryFormatter binary_file = new BinaryFormatter();
                    binary_file.Serialize(saving, saving_file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public static object Reload_session()
        {
            object info = null;
            try
            {
                using (Stream reading = File.Open((Parking_sessions), FileMode.Open))
                {
                    BinaryFormatter binary_file = new BinaryFormatter();
                    info = binary_file.Deserialize(reading);
                    return info;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return info;
        }
    }
}

