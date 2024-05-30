using System.Data.Odbc;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string connectionString = "DSN=PostgreSQL35W;Uid=postgres;Pwd=1;";

        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            connection.Open();

            ExecuteQuery(connection, "1. Отримати номери постачальників, які постачають вино W2",
                "SELECT id_prov FROM lab5.supply WHERE id_wine = 'W2';");

            ExecuteQuery(connection, "5. Отримати всі пари номерів постачальників, розташованих в одному місті",
                "SELECT p1.id_prov, p2.id_prov FROM lab5.provider p1, lab5.provider p2 WHERE p1.city_prov = p2.city_prov AND p1.id_prov < p2.id_prov;");

            ExecuteQuery(connection, "9. Отримати всі такі трійки “номер постачальника – номер вина – номер ресторану”, для яких постачальники, вина і ресторани розташовані у різних містах",
                "SELECT s.id_prov, s.id_wine, s.id_rest FROM lab5.supply s JOIN lab5.provider p ON s.id_prov = p.id_prov JOIN lab5.wine w ON s.id_wine = w.id_wine JOIN lab5.restaurant r ON s.id_rest = r.id_rest WHERE p.city_prov != w.city_wine AND w.city_wine != r.city_rest AND p.city_prov != r.city_rest;");

            ExecuteQuery(connection, "13. Отримати номери вин, що постачаються для усіх ресторанів постачальником із того ж міста, де розміщується ресторан",
                "SELECT DISTINCT s.id_wine FROM lab5.supply s JOIN lab5.provider p ON s.id_prov = p.id_prov JOIN lab5.restaurant r ON s.id_rest = r.id_rest WHERE p.city_prov = r.city_rest;");

            ExecuteQuery(connection, "17. Отримати загальну кількість вина W1, що постачається постачальником P1",
                "SELECT SUM(amount) AS total_amount FROM lab5.supply WHERE id_prov = 'P1' AND id_wine = 'W1';");

            ExecuteQuery(connection, "21. Отримати номери вин, що постачаються для будь-якого криворізького ресторану",
                "SELECT DISTINCT id_wine FROM lab5.supply s JOIN lab5.restaurant r ON s.id_rest = r.id_rest WHERE r.city_rest = 'Кривий Ріг';");

            ExecuteQuery(connection, "25. Отримати номери ресторанів, місто яких стоїть першим в алфавітному спискові міст",
                "SELECT id_rest FROM lab5.restaurant WHERE city_rest = (SELECT city_rest FROM lab5.restaurant ORDER BY city_rest LIMIT 1);");

            ExecuteQuery(connection, "29. Отримати назви ресторанів, які повністю забезпечуються постачальником P1",
                "SELECT r.name_rest FROM lab5.restaurant r WHERE NOT EXISTS (SELECT 1 FROM lab5.supply s WHERE s.id_rest = r.id_rest AND s.id_prov <> 'P1');");

            ExecuteQuery(connection, "33. Отримати всі міста, в яких розташовані хоча б один постачальник, одне вино чи один ресторан",
                "SELECT DISTINCT city_prov AS city FROM lab5.provider UNION SELECT DISTINCT city_wine AS city FROM lab5.wine UNION SELECT DISTINCT city_rest AS city FROM lab5.restaurant;");

            connection.Close();
        }
    }

    static void ExecuteQuery(OdbcConnection connection, string description, string query)
    {
        using (OdbcCommand command = new OdbcCommand(query, connection))
        {
            Console.WriteLine(description);
            using (OdbcDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i] + "\t");
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}
