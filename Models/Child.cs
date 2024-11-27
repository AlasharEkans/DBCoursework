using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using ChildrenGarden.Database;

namespace ChildrenGarden.Models
{
    public class Child
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public int ParentId { get; set; }
        public int GroupId { get; set; }

        // Пов'язані об'єкти
        public Parent Parent { get; set; }
        public Group Group { get; set; }

        // Метод додавання дитини до бази даних
        public void Add()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO children (name, surname, date_of_birth, sex, parent_id, group_id)
                VALUES (@name, @surname, @date_of_birth, @sex, @parent_id, @group_id);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Name);
                    command.Parameters.AddWithValue("@surname", Surname);
                    command.Parameters.AddWithValue("@date_of_birth", DateOfBirth);
                    command.Parameters.AddWithValue("@sex", Sex);
                    command.Parameters.AddWithValue("@parent_id", ParentId);
                    command.Parameters.AddWithValue("@group_id", GroupId);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Метод отримання списку всіх дітей
        public static List<Child> GetAll()
        {
            var children = new List<Child>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM children;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var child = new Child
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                DateOfBirth = DateTime.Parse(reader["date_of_birth"].ToString()),
                                Sex = reader["sex"].ToString(),
                                ParentId = Convert.ToInt32(reader["parent_id"]),
                                GroupId = Convert.ToInt32(reader["group_id"])
                            };

                            // Завантажуємо пов'язані об'єкти Parent та Group
                            child.Parent = Parent.GetById(child.ParentId);
                            child.Group = Group.GetById(child.GroupId);

                            children.Add(child);
                        }
                    }
                }

                connection.Close();
            }

            return children;
        }

        // Метод для отримання дитини за ID
        public static Child GetById(int id)
        {
            Child child = null;

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM children WHERE ID = @id;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            child = new Child
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                DateOfBirth = DateTime.Parse(reader["date_of_birth"].ToString()),
                                Sex = reader["sex"].ToString(),
                                ParentId = Convert.ToInt32(reader["parent_id"]),
                                GroupId = Convert.ToInt32(reader["group_id"])
                            };

                            // Завантажуємо пов'язані об'єкти Parent та Group
                            child.Parent = Parent.GetById(child.ParentId);
                            child.Group = Group.GetById(child.GroupId);
                        }
                    }
                }

                connection.Close();
            }

            return child;
        }

        // Метод отримання дітей за ParentId
        public static List<Child> GetByParentId(int parentId)
        {
            var children = new List<Child>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM children WHERE parent_id = @parentId;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@parentId", parentId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var child = new Child
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                DateOfBirth = DateTime.Parse(reader["date_of_birth"].ToString()),
                                Sex = reader["sex"].ToString(),
                                ParentId = Convert.ToInt32(reader["parent_id"]),
                                GroupId = Convert.ToInt32(reader["group_id"])
                            };

                            // Завантажуємо пов'язані об'єкти Parent та Group
                            child.Parent = Parent.GetById(child.ParentId);
                            child.Group = Group.GetById(child.GroupId);

                            children.Add(child);
                        }
                    }
                }

                connection.Close();
            }

            return children;
        }

        // Метод отримання дітей за GroupId
        public static List<Child> GetByGroupId(int groupId)
        {
            var children = new List<Child>();

            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM children WHERE group_id = @groupId;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@groupId", groupId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var child = new Child
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                DateOfBirth = DateTime.Parse(reader["date_of_birth"].ToString()),
                                Sex = reader["sex"].ToString(),
                                ParentId = Convert.ToInt32(reader["parent_id"]),
                                GroupId = Convert.ToInt32(reader["group_id"])
                            };

                            // Завантажуємо пов'язані об'єкти Parent та Group
                            child.Parent = Parent.GetById(child.ParentId);
                            child.Group = Group.GetById(child.GroupId);

                            children.Add(child);
                        }
                    }
                }

                connection.Close();
            }

            return children;
        }

        public void Delete()
        {
            using (var connection = Database.Database.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Call the existing Delete method with connection and transaction
                        Delete(connection, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Помилка при видаленні дитини: {ex.Message}");
                    }
                }

                connection.Close();
            }
        }

        // Existing Delete method with parameters
        public void Delete(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            try
            {
                // Delete attendance records
                string deleteAttendanceQuery = "DELETE FROM attendance WHERE child_id = @childId;";
                using (var command = new SQLiteCommand(deleteAttendanceQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@childId", ID);
                    command.ExecuteNonQuery();
                }

                // Delete payments
                string deletePaymentsQuery = "DELETE FROM payments WHERE child_id = @childId;";
                using (var command = new SQLiteCommand(deletePaymentsQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@childId", ID);
                    command.ExecuteNonQuery();
                }

                // Delete the child
                string deleteChildQuery = "DELETE FROM children WHERE ID = @childId;";
                using (var command = new SQLiteCommand(deleteChildQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@childId", ID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при видаленні дитини {Name} {Surname}: {ex.Message}");
            }
        }

        // Перевизначення ToString() для зручного відображення
        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}