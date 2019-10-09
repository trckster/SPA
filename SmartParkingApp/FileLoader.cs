using System;  
using System.IO;  
using System.Linq;
using ParkingApp;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmartParkingApp
{
    [Serializable]
    public static class FileLoader
    {
        private const string SessionStorageFileName = "save.txt";
        private const string UsersFileName = "users.txt";
        private const string TariffsFileName = "tariffs.txt";

        public static void SaveObject(object objectToSave)
        {
            Stream s;
            BinaryFormatter bf = new BinaryFormatter();

            s = File.Open(SessionStorageFileName, FileMode.Create);
            bf.Serialize(s, objectToSave);
            s.Close();
        }

        public static object RestoreObject()
        {
            Stream s;
            object obj;
            BinaryFormatter bf = new BinaryFormatter();

            s = File.Open(SessionStorageFileName, FileMode.Open);
            obj = bf.Deserialize(s);
            s.Close();
            
            return obj;
        }

        public static bool HasSave()
        {
            return File.Exists(SessionStorageFileName);
        }

        public static List<User> LoadUsers()
        {
            List<User> users = new List<User>();
            string[] lines = File.ReadAllLines(UsersFileName);
            
            foreach (string line in lines)
            {
                string[] fields = line.Split('|');
                users.Add(new User() {
                    Name = fields[0],
                    CarPlateNumber = fields[1],
                    Phone = fields[2]
                });
            }

            return users;
        }

        public static List<Tariff> LoadTariffs()
        {
            List<Tariff> tariffs = new List<Tariff>();
            string[] lines = File.ReadAllLines(TariffsFileName);
            
            foreach (string line in lines)
            {
                string[] fields = line.Split('|');
                tariffs.Add(new Tariff(int.Parse(fields[0]), decimal.Parse(fields[1])));
            }

            return tariffs;
        }
    }
}