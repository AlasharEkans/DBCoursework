using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChildrenGarden.Database;

namespace ChildrenGarden.Models
{
    public class Parent
    {
        public int Idparents { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // Метод додавання батька до бази даних
        public void Add()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO parents (name, surname, phone, email, address)
                VALUES (@name, @surname, @phone, @email, @address);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Name);
                    command.Parameters.AddWithValue("@surname", Surname);
                    command.Parameters.AddWithValue("@phone", Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@address", Address ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод отримання списку всіх батьків
        public static List<Parent> GetAll()
        {
            var parents = new List<Parent>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM parents;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var parent = new Parent
                            {
                                Idparents = Convert.ToInt32(reader["idparents"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Email = reader["email"].ToString(),
                                Address = reader["address"].ToString()
                            };

                            parents.Add(parent);
                        }
                    }
                }

                connection.Close();
            }

            return parents;
        }

        // Метод для отримання батька за ID
        public static Parent GetById(int id)
        {
            Parent parent = null;

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM parents WHERE idparents = @id;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            parent = new Parent
                            {
                                Idparents = Convert.ToInt32(reader["idparents"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Email = reader["email"].ToString(),
                                Address = reader["address"].ToString()
                            };
                        }
                    }
                }

                connection.Close();
            }

            return parent;
        }

        // Метод видалення батька/матері та пов'язаних записів
        public void Delete()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Отримуємо список дітей цього батька/матері
                        var children = Child.GetByParentId(Idparents);

                        foreach (var child in children)
                        {
                            child.Delete(connection, transaction);
                        }

                        // Видалення платежів
                        string deletePaymentsQuery = "DELETE FROM payments WHERE parent_id = @parentId;";
                        using (var command = new SQLiteCommand(deletePaymentsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@parentId", Idparents);
                            command.ExecuteNonQuery();
                        }

                        // Видаляємо батька/матір
                        string deleteParentQuery = "DELETE FROM parents WHERE idparents = @parentId;";
                        using (var command = new SQLiteCommand(deleteParentQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@parentId", Idparents);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Помилка при видаленні батька/матері: {ex.Message}");
                    }
                }

                connection.Close();
            }
        }

        // Перевизначення ToString() для зручного відображення
        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}