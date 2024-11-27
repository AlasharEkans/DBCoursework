using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ChildrenGarden.Models;

namespace ChildrenGarden.Forms
{
    public partial class GroupsTab : UserControl
    {
        private ListBox listBoxGroups;
        private TextBox textBoxName;
        private TextBox textBoxAge;
        private ComboBox comboBoxTeacher;
        private TextBox textBoxSchedule;
        private TextBox textBoxRoom;
        private Button buttonAdd;
        private Button buttonDelete;

        public GroupsTab()
        {
            InitializeComponents();
            LoadGroups();
            LoadStaff();
        }

        private void InitializeComponents()
        {
            listBoxGroups = new ListBox();
            textBoxName = new TextBox();
            textBoxAge = new TextBox();
            comboBoxTeacher = new ComboBox();
            textBoxSchedule = new TextBox();
            textBoxRoom = new TextBox();
            buttonAdd = new Button();
            buttonDelete = new Button(); // Ініціалізація кнопки видалення

            int leftMargin = 10;
            int topMargin = 10;
            int labelWidth = 120;
            int controlWidth = 200;
            int controlHeight = 25;
            int spacing = 5;

            listBoxGroups.Location = new System.Drawing.Point(leftMargin, topMargin);
            listBoxGroups.Size = new System.Drawing.Size(300, 400);
            listBoxGroups.SelectedIndexChanged += ListBoxGroups_SelectedIndexChanged;

            int formLeft = leftMargin + 310;
            int currentTop = topMargin;

            var labelName = new Label();
            labelName.Text = "Назва:";
            labelName.Location = new System.Drawing.Point(formLeft, currentTop);
            labelName.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxName.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxName.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelAge = new Label();
            labelAge.Text = "Вік:";
            labelAge.Location = new System.Drawing.Point(formLeft, currentTop);
            labelAge.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxAge.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxAge.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelTeacher = new Label();
            labelTeacher.Text = "Вчитель:";
            labelTeacher.Location = new System.Drawing.Point(formLeft, currentTop);
            labelTeacher.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxTeacher.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxTeacher.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelSchedule = new Label();
            labelSchedule.Text = "Розклад:";
            labelSchedule.Location = new System.Drawing.Point(formLeft, currentTop);
            labelSchedule.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxSchedule.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxSchedule.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelRoom = new Label();
            labelRoom.Text = "Кімната:";
            labelRoom.Location = new System.Drawing.Point(formLeft, currentTop);
            labelRoom.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxRoom.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxRoom.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            buttonAdd.Text = "Додати";
            buttonAdd.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Text = "Видалити";
            buttonDelete.Location = new System.Drawing.Point(formLeft + labelWidth + buttonAdd.Width + 10, currentTop);
            buttonDelete.Click += buttonDelete_Click;

            Controls.Add(listBoxGroups);
            Controls.Add(labelName);
            Controls.Add(textBoxName);
            Controls.Add(labelAge);
            Controls.Add(textBoxAge);
            Controls.Add(labelTeacher);
            Controls.Add(comboBoxTeacher);
            Controls.Add(labelSchedule);
            Controls.Add(textBoxSchedule);
            Controls.Add(labelRoom);
            Controls.Add(textBoxRoom);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);
        }

        private void LoadGroups()
        {
            listBoxGroups.Items.Clear();
            List<Group> groups = Group.GetAll();
            foreach (var group in groups)
            {
                listBoxGroups.Items.Add(group);
            }
        }

        public void LoadStaff()
        {
            comboBoxTeacher.Items.Clear();
            List<Staff> staffList = Staff.GetAll();
            foreach (var staff in staffList)
            {
                comboBoxTeacher.Items.Add(staff);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Перевірка на порожні поля
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                comboBoxTeacher.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля (Назва та Вчитель).");
                return;
            }

            var group = new Group
            {
                Name = textBoxName.Text.Trim(),
                Age = string.IsNullOrWhiteSpace(textBoxAge.Text) ? null : textBoxAge.Text.Trim(),
                TeacherId = ((Staff)comboBoxTeacher.SelectedItem).Idstaff,
                Schedule = string.IsNullOrWhiteSpace(textBoxSchedule.Text) ? null : textBoxSchedule.Text.Trim(),
                Room = string.IsNullOrWhiteSpace(textBoxRoom.Text) ? null : textBoxRoom.Text.Trim()
            };
            group.Add();
            LoadGroups();
            ClearForm();

            GroupAdded?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxGroups.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, оберіть групу для видалення.");
                return;
            }

            var selectedGroup = (Group)listBoxGroups.SelectedItem;

            // Перевірка наявності пов'язаних дітей
            var childrenInGroup = Child.GetByGroupId(selectedGroup.Idgroups);

            string message = "Ви впевнені, що хочете видалити цю групу?";
            if (childrenInGroup.Count > 0)
            {
                message += "\nДіти в цій групі будуть віднесені до групи без призначення.";
            }

            DialogResult result = MessageBox.Show(message, "Підтвердження видалення", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    selectedGroup.Delete();
                    LoadGroups();
                    ClearForm();
                    MessageBox.Show("Групу успішно видалено.");

                    // Викликаємо подію, якщо потрібна для оновлення в інших вкладках
                    GroupDeleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні групи: {ex.Message}");
                }
            }
        }

        private void ClearForm()
        {
            textBoxName.Text = "";
            textBoxAge.Text = "";
            comboBoxTeacher.SelectedIndex = -1;
            textBoxSchedule.Text = "";
            textBoxRoom.Text = "";
        }

        private void ListBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Можна додати заповнення полів при виборі групи зі списку
        }

        // Подія для повідомлення інших вкладок про видалення групи
        public event EventHandler GroupDeleted;

        // Подія для повідомлення інших вкладок про додавання групи
        public event EventHandler GroupAdded;
    }
}