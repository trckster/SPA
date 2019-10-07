using System;
using System.Linq;
namespace ParkingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ParkingManager app = new ParkingManager();

            if (app.EnterParking("kekheh") == null)
                Console.WriteLine("fcuk");
            else
                Console.WriteLine("oh god");
            if (app.EnterParking("newkekheh") == null)
                Console.WriteLine("fcuk");
            else
                Console.WriteLine("oh god");
            if (app.EnterParking("kekheh") == null)
                Console.WriteLine("fcuk");
            else
                Console.WriteLine("oh god");
        }
    }
}