using HttpServer.Models;

namespace HttpServer.ORM.DAO
{
    class AccountDAO : IDAO<Account, int>
    {
        private MyORM orm;

        public AccountDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Accounts");
            if (!TableExist())
            {
                Console.WriteLine("Table not found. Creating...");
                CreateTable();
            }
        }

        public int Delete(int id) =>
            orm.Delete($"Id = {id}");

        public List<Account> GetAll() =>
            orm.Select<Account>().ToList();

        public Account? GetEntityByKey(int id) =>
            orm.Select<Account>($"Id = {id}").FirstOrDefault();

        public bool Create(Account entity) =>
            orm.Insert(entity);

        public bool Update(Account entity) =>
            orm.Update(entity, $"Id = {entity.Id}");

        public Account? GetEntityByLogin(string login) =>
            orm.Select<Account>($"Login = '{login}'").FirstOrDefault();

        private bool TableExist()
        {
            var checkCmd = "SELECT COUNT(*) FROM [information_schema].tables " +
                "WHERE table_name = 'Accounts';";
            return orm.ExecuteScalar<int>(checkCmd) > 0 ? true : false;
        }

        public bool CreateTable()
        {
            if (TableExist()) return false;
            var createCmd = "CREATE TABLE Accounts(" +
                "Id INT PRIMARY KEY NOT NULL IDENTITY(1, 1)," +
                "Login VARCHAR(50) NOT NULL," +
                "Password VARCHAR(50) NOT NULL);";
            orm.ExecuteNonQuerry(createCmd);
            return true;
        }
    }
}