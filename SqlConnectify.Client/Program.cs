using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlConnectify;
using System.Data;
using System.Data.SqlClient;

namespace SqlConnectify.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectify = new Connectify();
            var items = connectify.Read<Item>("select * from categories");
            Console.WriteLine(items.Count);
            if (items != null && items.Any())
            {
                items.ForEach(i => Console.WriteLine("name: {0}, id: {1}, key: {2}", i.Name, i.Id, i.Description));
            }

            Console.ReadLine();
        }
    }

    public class Item
    {
        //[ConnectifyMapping(Column="Id")]
        public int Id { get; set; }

        [ConnectifyMapping(Column="NameE")]
        public string Name { get; set; }

        //[ConnectifyMapping(Column="Key")]
        public string Description { get; set; }
    }
}
