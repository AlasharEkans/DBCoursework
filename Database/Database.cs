using System;
using System.Data.SQLite;
using System.IO;

namespace ChildrenGarden.Database
{
    /// <summary>
    /// Клас Database забезпечує взаємодію з базою даних SQLite.
    /// </summary>
    public static class Database
    {
        // Шлях до файлу бази даних
        private static readonly string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.sqlite");

        // Рядок підключення до бази даних
        private static readonly string connectionString = $"Data Source={dbFilePath};Version=3;";

        /// <summary>
        /// Ініціалізує базу даних. Якщо файл бази даних не існує, він буде створений, і всі таблиці будуть створені.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // Перевірка наявності файлу бази даних
                if (!File.Exists(dbFilePath))
                {
                    // Створення файлу бази даних
                    SQLiteConnection.CreateFile(dbFilePath);

                    // Створення таблиць
                    CreateTables();
                }
                else
                {
                    // Перевірка наявності необхідних таблиць
                    VerifyTables();
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок під час ініціалізації бази даних
                Console.WriteLine($"Помилка при ініціалізації бази даних: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Повертає новий об'єкт SQLiteConnection для взаємодії з базою даних.
        /// </summary>
        /// <returns>Об'єкт SQLiteConnection.</returns>
        public static SQLiteConnection GetConnection()
        {
            try
            {
                var connection = new SQLiteConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                // Обробка помилок під час створення підключення
                Console.WriteLine($"Помилка при створенні підключення до бази даних: {ex.Message}");
                throw;
            }
        }

        private static void CreateTables()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // SQL-запити для створення таблиць
                        string createParentsTable = @"
                        CREATE TABLE IF NOT EXISTS parents (
                            idparents INTEGER PRIMARY KEY AUTOINCREMENT,
                            name TEXT NOT NULL,
                            surname TEXT NOT NULL,
                            phone TEXT,
                            email TEXT,
                            address TEXT
                        );";

                        string createStaffTable = @"
                        CREATE TABLE IF NOT EXISTS staff (
                            idstaff INTEGER PRIMARY KEY AUTOINCREMENT,
                            name TEXT NOT NULL,
                            surname TEXT NOT NULL,
                            position TEXT,
                            phone TEXT,
                            email TEXT,
                            hire_date DATE
                        );";

                        string createGroupsTable = @"
                        CREATE TABLE IF NOT EXISTS groups (
                            idgroups INTEGER PRIMARY KEY AUTOINCREMENT,
                            name TEXT NOT NULL,
                            age TEXT,
                            teacher_id INTEGER,
                            schedule TEXT,
                            room TEXT,
                            FOREIGN KEY (teacher_id) REFERENCES staff(idstaff)
                        );";

                        string createChildrenTable = @"
                        CREATE TABLE IF NOT EXISTS children (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            name TEXT NOT NULL,
                            surname TEXT NOT NULL,
                            date_of_birth DATE,
                            sex TEXT,
                            parent_id INTEGER,
                            group_id INTEGER,
                            FOREIGN KEY (parent_id) REFERENCES parents(idparents),
                            FOREIGN KEY (group_id) REFERENCES groups(idgroups)
                        );";

                        string createAttendanceTable = @"
                        CREATE TABLE IF NOT EXISTS attendance (
                            idattendance INTEGER PRIMARY KEY AUTOINCREMENT,
                            child_id INTEGER,
                            date DATE,
                            status TEXT,
                            notes TEXT,
                            FOREIGN KEY (child_id) REFERENCES children(ID)
                        );";

                        string createPaymentsTable = @"
                        CREATE TABLE IF NOT EXISTS payments (
                            idpayments INTEGER PRIMARY KEY AUTOINCREMENT,
                            parent_id INTEGER,
                            child_id INTEGER, 
                            amount REAL,
                            date DATE,
                            way TEXT,
                            FOREIGN KEY (parent_id) REFERENCES parents(idparents),
                            FOREIGN KEY (child_id) REFERENCES children(ID)
                        );";

                        // Виконання запитів для створення таблиць
                        ExecuteNonQuery(connection, createParentsTable);
                        ExecuteNonQuery(connection, createStaffTable);
                        ExecuteNonQuery(connection, createGroupsTable);
                        ExecuteNonQuery(connection, createChildrenTable);
                        ExecuteNonQuery(connection, createAttendanceTable);
                        ExecuteNonQuery(connection, createPaymentsTable);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Обробка помилок під час створення таблиць
                        Console.WriteLine($"Помилка при створенні таблиць: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Перевіряє наявність необхідних таблиць у базі даних.
        /// </summary>
        private static void VerifyTables()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                try
                {
                    // Перевірка наявності необхідних таблиць
                    string[] tables = { "parents", "staff", "groups", "children", "attendance", "payments" };
                    foreach (string table in tables)
                    {
                        if (!TableExists(connection, table))
                        {
                            throw new Exception($"Таблиця '{table}' не існує в базі даних.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Обробка помилок перевірки
                    Console.WriteLine($"Помилка при перевірці таблиць: {ex.Message}");
                    throw;
                }

                connection.Close();
            }
        }

        /// <param name="connection">Підключення до бази даних.</param>
        /// <param name="tableName">Назва таблиці.</param>
        /// <returns>true, якщо таблиця існує; інакше false.</returns>
        private static bool TableExists(SQLiteConnection connection, string tableName)
        {
            string query = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }
        /// <param name="connection">Підключення до бази даних.</param>
        /// <param name="query">SQL-запит для виконання.</param>
        private static void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}