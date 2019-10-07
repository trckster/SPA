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

        public void SaveObject(object objectToSave)
        {
            Stream s;
            BinaryFormatter bf = new BinaryFormatter();

            s = File.Open(FileLoader.FileName, FileMode.Create);
            bf.Serialize(s, objectToSave);
            s.Close();
        }

        public object RestoreObject()
        {
            Stream s;
            object obj;
            BinaryFormatter bf = new BinaryFormatter();

            s = File.Open(FileLoader.FileName, FileMode.Open);
            obj = bf.Deserialize(s);
            s.Close();
            
            return obj;
        }
    }
}