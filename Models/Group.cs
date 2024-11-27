using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChildrenGarden.Database;

namespace ChildrenGarden.Models
{
    public class Group
    {
        public int Idgroups { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public int TeacherId { get; set; }
        public string Schedule { get; set; }
        public string Room { get; set; }

        public Staff Teacher { get; set; }

        public void Add()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO groups (name, age, teacher_id, schedule, room)
                VALUES (@name, @age, @teacher_id, @schedule, @room);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Name);
                    command.Parameters.AddWithValue("@age", Age);
                    command.Parameters.AddWithValue("@teacher_id", TeacherId);
                    command.Parameters.AddWithValue("@schedule", Schedule);
                    command.Parameters.AddWithValue("@room", Room);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод отримання списку всіх груп
        public static List<Group> GetAll()
        {
            var groups = new List<Group>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM groups;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var group = new Group
                            {
                                Idgroups = Convert.ToInt32(reader["idgroups"]),
                                Name = reader["name"].ToString(),
                                Age = reader["age"].ToString(),
                                TeacherId = Convert.ToInt32(reader["teacher_id"]),
                                Schedule = reader["schedule"].ToString(),
                                Room = reader["room"].ToString()
                            };

                            // Завантажуємо об'єкт Teacher
                            group.Teacher = Staff.GetById(group.TeacherId);

                            groups.Add(group);
                        }
                    }
                }

                connection.Close();
            }

            return groups;
        }

        // Метод для отримання групи за ID
        public static Group GetById(int id)
        {
            Group group = null;

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM groups WHERE idgroups = @id;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            group = new Group
                            {
                                Idgroups = Convert.ToInt32(reader["idgroups"]),
                                Name = reader["name"].ToString(),
                                Age = reader["age"].ToString(),
                                TeacherId = Convert.ToInt32(reader["teacher_id"]),
                                Schedule = reader["schedule"].ToString(),
                                Room = reader["room"].ToString()
                            };

                            // Завантажуємо об'єкт Teacher
                            group.Teacher = Staff.GetById(group.TeacherId);
                        }
                    }
                }

                connection.Close();
            }

            return group;
        }

        // Метод видалення групи
        public void Delete()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Отримуємо список дітей в цій групі
                        var childrenInGroup = Child.GetByGroupId(Idgroups);

                        if (childrenInGroup.Count > 0)
                        {
                            // Можна або видалити дітей, або перевести їх в іншу групу
                            // У цьому прикладі встановимо group_id = NULL
                            string updateChildrenQuery = "UPDATE children SET group_id = NULL WHERE group_id = @groupId;";
                            using (var command = new SQLiteCommand(updateChildrenQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@groupId", Idgroups);
                                command.ExecuteNonQuery();
                            }
                        }

                        // Видаляємо групу
                        string deleteGroupQuery = "DELETE FROM groups WHERE idgroups = @groupId;";
                        using (var command = new SQLiteCommand(deleteGroupQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@groupId", Idgroups);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Помилка при видаленні групи: {ex.Message}");
                    }
                }

                connection.Close();
            }
        }

        // Перевизначення ToString() для зручного відображення
        public override string ToString()
        {
            return $"{Name} (Вік: {Age})";
        }
    }
}