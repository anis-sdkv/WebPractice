using HttpServer.Attributes;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;

namespace HttpServer.ORM
{
    public class MyORM
    {
        private string _connectionString;
        private string _tableName;

        public MyORM(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        //public MyORM AddParameter<T>(string name, T value)
        //{
        //    var parameter = new SqlParameter();
        //    parameter.ParameterName = name;
        //    parameter.Value = value;
        //    _command.Parameters.Add(parameter);
        //    return this;
        //}

        public int ExecuteNonQuerry(string querry, bool isStoredProc = false)
        {
            int noOfAffectedRows = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                if (isStoredProc)
                    command.CommandType = CommandType.StoredProcedure;

                command.CommandText = querry;
                connection.Open();
                noOfAffectedRows = command.ExecuteNonQuery();
            }
            return noOfAffectedRows;
        }

        public IEnumerable<T> ExecuteQuerry<T>(string querry, bool isStoredProc = false)
        {
            var result = new List<T>();
            var type = typeof(T);

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                if (isStoredProc)
                    command.CommandType = CommandType.StoredProcedure;

                command.CommandText = querry;
                connection.Open();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var instance = (T)Activator.CreateInstance(type);
                    type.GetProperties().ToList().ForEach(p =>
                    {
                        p.SetValue(instance, reader[p.Name]);
                    });
                    result.Add(instance);
                }
            }

            return result;
        }

        public T ExecuteScalar<T>(string querry, bool isStoredProc = false)
        {
            var result = default(T);

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                if (isStoredProc)
                    command.CommandType = CommandType.StoredProcedure;

                command.CommandText = querry;
                connection.Open();
                result = (T)command.ExecuteScalar();
            }

            return result;
        }


        public IEnumerable<T> Select<T>(string? selector = null)
        {
            var command = $"SELECT * FROM [dbo].[{_tableName}]";
            if (selector != null)
                command += $"WHERE {selector}";
            return ExecuteQuerry<T>(command);
        }

        public bool Insert<T>(T obj)
        {
            var properties = typeof(T)
                .GetProperties()
                .Where(p => p.IsDefined(typeof(DataField)))
                .ToDictionary(p => p.Name, p => $"'{p.GetValue(obj)}'");
            var command = $"INSERT INTO [dbo].[{_tableName}] " +
                $"({string.Join(',', properties.Keys)}) " +
                $"VALUES ({string.Join(',', properties.Values)});";
            return ExecuteNonQuerry(command) > 0 ? true : false;
        }

        public bool Update<T>(T obj, string selector)
        {
            var properties = typeof(T)
                .GetProperties()
                .Where(p => p.IsDefined(typeof(DataField)))
                .Select(p => $"{p.Name} = '{p.GetValue(obj)}'");
            var command = $"UPDATE [dbo].[{_tableName}]" +
                $"SET ({string.Join(',', properties)})" +
                $"WHERE {selector};";
            return ExecuteNonQuerry(command) == 1 ? true : false;
        }

        public int Delete(string selector)
        {
            var command = $"DELETE FROM [dbo].[{_tableName}]" +
                $"WHERE {selector}";
            return ExecuteNonQuerry(command);
        }
    }
}
