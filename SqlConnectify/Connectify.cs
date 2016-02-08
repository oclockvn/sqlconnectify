using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Data;

namespace SqlConnectify
{
    public interface IConnectify
    {
        List<T> Read<T>(string query);
    }

    public class Connectify : IConnectify
    {
        private ConnectifyManager _manager = new ConnectifyManager();

        private void Log(string format, params object[] args)
        {
            Logify.Log(format, args);
        }

        public void ConfigConnectionString(string connectionString)
        {
            _manager.ConfigConnectionString(connectionString);
        }

        public List<T> Read<T>(string query)
        {
            var table = _manager.Read(query);
            if (table == null || table.Rows.Count == 0) return null;

            var results = MappingList<T>(table);

            return results;
        }

        private List<T> MappingList<T>(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            List<T> results = new List<T>();
            var rows = table.Rows.Count;

            for (int i = 0; i < rows; i++)
            {
                var row = table.Rows[i];

                T obj = Mapping<T>(row);

                if (!EqualityComparer<T>.Default.Equals(obj, default(T)))
                {
                    results.Add(obj);
                }
            }

            return results;
        }

        private T Mapping<T>(DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }

            T obj = Activator.CreateInstance<T>();
            if (!typeof(T).GetProperties()
                .Any(p => p.GetCustomAttributes(typeof (ConnectifyMappingAttribute)).Any()))
            {
                return default(T);
            }

            var properties = typeof(T).GetProperties();

            if (properties == null || !properties.Any())
            {
                return default(T);
            }

            foreach (var prop in properties)
            {
                if (!prop.IsDefined(typeof(ConnectifyMappingAttribute)))
                    continue;

                var column = prop.GetCustomAttribute<ConnectifyMappingAttribute>().Column;
                if (string.IsNullOrEmpty(column) || string.IsNullOrWhiteSpace(column))
                    continue;

                if (!row.Table.Columns.Contains(column))
                    continue;

                var value = row[column];

                try
                {
                    prop.SetValue(obj, value);
                }
                catch (Exception)
                {

                }
            }

            return obj;
        }
    }
}
