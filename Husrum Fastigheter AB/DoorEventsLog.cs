using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husrum_Fastigheter_AB
{
    public class DoorEventsLog
    {
        int Max_Enteries = 20;
        Database DataBase = new Database();

        public void Start()
        {
            int input = 0;
            while (true)
            {
                Console.WriteLine("\n(1) Search by door");
                Console.WriteLine("(2) Search by Event");
                Console.WriteLine("(3) Search by Location");
                Console.WriteLine("(4) Search by Tenant");
                Console.WriteLine("(5) Search by Tag");
                Console.WriteLine("(6) Find tenants by apartment number");
                Console.WriteLine("(7) Create database");

                while (true)
                {
                    try
                    {
                        input = Convert.ToInt32(Console.ReadLine());
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input");
                    }
                }

                switch (input)
                {
                    case 1:
                        FindEntriesByDoor();
                        break;
                    case 2:
                        FindEntriesByEvent();
                        break;
                    case 3:
                        FindEntriesByLocation();
                        break;
                    case 4:
                        FindEntriesByTenant();
                        break;
                    case 5:
                        FindEntriesByTag();
                        break;
                    case 6:
                        ListTenantAt();
                        break;
                    case 7:
                        DataBase.Create_Database();
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }
        }

        public string Input_Reader()
        {
            string input = "";
            while (true)
            {
                try
                {
                    input = Console.ReadLine();
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input");
                }
            }
            return input;
        }

        public void Print_Result(DataTable result)
        {
            foreach (DataRow x in result.Rows)
            {
                var time = x["Time"].ToString();
                var date = x["Date"].ToString();
                var location = x["Location"].ToString();
                var tenant = x["Name"].ToString();
                var @event = x["Event"].ToString();
                var tag = x["Tag"].ToString();

                switch (@event)
                {
                    case "DÖIN":
                        Console.WriteLine(date + "\t" + time + "\t" + location + "\t" + @event + "\t" + tag + "\t" + tenant + " öppnade dörren till {0} inifrån", location);
                        break;
                    case "DÖUT":
                        Console.WriteLine(date + "\t" + time + "\t" + location + "\t" + @event + "\t" + tag + "\t" + tenant + " öppnade dörren till {0} utifrån", location);
                        break;
                    case "FDIN":
                        Console.WriteLine(date + "\t" + time + "\t" + location + "\t" + @event + "\t" + tag + "\t" + tenant + " försökte öppna dörren till {0} inifrån", location);
                        break;
                    default:
                        Console.WriteLine(date + "\t" + time + "\t" + location + "\t" + @event + "\t" + tag + "\t" + tenant + " försökte öppna dörren till {0} utifrån", location);
                        break;
                }

            }
        }

        public void FindEntriesByDoor ()
        {
            Console.WriteLine("Enter the door code: ");
            string input = Input_Reader();
            DataTable result = DataBase.Data_Fetcher(@"SELECT Logs.Date, Logs.Time, Locations.Location, Events.Event, Tag.Tag, Tenants.name
                                                       FROM Logs 
                                                       JOIN Locations on Logs.Location = Locations.ID
                                                       JOIN Events on Logs.Event = Events.ID
                                                       JOIN Tenants on Logs.Tenant = Tenants.ID
                                                       LEFT JOIN Tenants as Tag ON Logs.Tag = Tag.ID 
                                                       WHERE Locations.Location = @Location
                                                       ORDER BY Logs.time DESC
                                                       LIMIT " + Max_Enteries, new string[] { "@Location", input });
            if (result.Rows.Count == 0)
                Console.WriteLine("No results found");
            else
                Print_Result(result);
        }

        public void FindEntriesByEvent()
        {
            Console.WriteLine("Enter the event code: ");
            string input = Input_Reader();
            DataTable result = DataBase.Data_Fetcher(@"SELECT Logs.Date, Logs.Time, Locations.Location, Events.Event, Tag.Tag, Tenants.name
                                                       FROM Logs 
                                                       JOIN Locations on Logs.Location = Locations.ID
                                                       JOIN Events on Logs.Event = Events.ID
                                                       JOIN Tenants on Logs.Tenant = Tenants.ID
                                                       LEFT JOIN Tenants as Tag ON Logs.Tag = Tag.ID 
                                                       WHERE Events.Event = @Event
                                                       ORDER BY Logs.time DESC
                                                       LIMIT " + Max_Enteries, new string[] { "@Event", input });
            if (result.Rows.Count == 0)
                Console.WriteLine("No results found");
            else
                Print_Result(result);
        }

        public void FindEntriesByLocation()
        {
            Console.WriteLine("Enter the location code: ");
            string input = Input_Reader();
            input = "%" + input + "%";
            DataTable result = DataBase.Data_Fetcher(@"SELECT Logs.Date, Logs.Time, Locations.Location, Events.Event, Tag.Tag, Tenants.name
                                                       FROM Logs 
                                                       JOIN Locations on Logs.Location = Locations.ID
                                                       JOIN Events on Logs.Event = Events.ID
                                                       JOIN Tenants on Logs.Tenant = Tenants.ID
                                                       LEFT JOIN Tenants as Tag ON Logs.Tag = Tag.ID 
                                                       WHERE Locations.Location LIKE @Location
                                                       ORDER BY Logs.time DESC
                                                       LIMIT " + Max_Enteries, new string[] { "@Location", input });
            if (result.Rows.Count == 0)
                Console.WriteLine("No results found");
            else
                Print_Result(result);
        }

        public void FindEntriesByTag()
        {
            Console.WriteLine("Enter the tag code: ");
            string input = Input_Reader();
            DataTable result = DataBase.Data_Fetcher(@"SELECT Logs.Date, Logs.Time, Locations.Location, Events.Event, Tag.Tag, Tenants.name
                                                       FROM Logs 
                                                       JOIN Locations on Logs.Location = Locations.ID
                                                       JOIN Events on Logs.Event = Events.ID
                                                       JOIN Tenants on Logs.Tenant = Tenants.ID
                                                       LEFT JOIN Tenants as Tag ON Logs.Tag = Tag.ID 
                                                       WHERE Tenants.Tag = @Tag
                                                       ORDER BY Logs.time DESC
                                                       LIMIT " + Max_Enteries, new string[] { "@Tag", input });
            if (result.Rows.Count == 0)
                Console.WriteLine("No results found");
            else
                Print_Result(result);
        }

        public void FindEntriesByTenant()
        {
            Console.WriteLine("Enter the tenant's name: ");
            string input = Input_Reader();
            DataTable result = DataBase.Data_Fetcher(@"SELECT Logs.Date, Logs.Time, Locations.Location, Events.Event, Tag.Tag, Tenants.name
                                                       FROM Logs 
                                                       JOIN Locations on Logs.Location = Locations.ID
                                                       JOIN Events on Logs.Event = Events.ID
                                                       JOIN Tenants on Logs.Tenant = Tenants.ID
                                                       LEFT JOIN Tenants as Tag ON Logs.Tag = Tag.ID 
                                                       WHERE Tenants.Name = @Name
                                                       ORDER BY Logs.time DESC
                                                       LIMIT " + Max_Enteries, new string[] { "@Name", input });
            if (result.Rows.Count == 0)
                Console.WriteLine("No results found");
            else
                Print_Result(result);
        }

        public void ListTenantAt()
        {
            Console.WriteLine("Enter the apartment's number: ");
            string input = Input_Reader();
            DataTable result = DataBase.Data_Fetcher(@"SELECT * FROM Tenants 
                                                     WHERE Tenants.Apartment = @Apartment", new string[] {"@Apartment" , input});

            foreach (DataRow x in result.Rows)
            {
                Console.WriteLine(x["Name"] + "\t" + x["Apartment"] + "\t" + x["Tag"]);
            }
        }

        public void LogEntry()
        {
            Console.WriteLine("");
        }


    }
}
