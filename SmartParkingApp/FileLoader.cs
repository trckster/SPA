using System;  
using System.IO;  
using System.Runtime.Serialization;  
using System.Runtime.Serialization.Formatters.Binary;  

namespace SmartParkingApp
{
    [Serializable]
    public class FileLoader
    {
        private const string FileName = "save.txt";

        public static void SaveObject(object objectToSave)
        {
            Stream s;
            BinaryFormatter bf = new BinaryFormatter();

            s = File.Open(FileName, FileMode.Create);
            bf.Serialize(s, objectToSave);
            s.Close();
        }

        public static object RestoreObject()
        {
            Stream s;
            object obj;
            BinaryFormatter bf = new BinaryFormatter();

            s = File.Open(FileName, FileMode.Open);
            obj = bf.Deserialize(s);
            s.Close();
            
            return obj;
        }

        public static bool HasSave()
        {
            return File.Exists(FileName);
        }
    }
}