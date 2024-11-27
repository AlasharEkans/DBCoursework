using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ChildModel = ChildrenGarden.Models.Child;
using ParentModel = ChildrenGarden.Models.Parent;
using GroupModel = ChildrenGarden.Models.Group;
using ChildrenGarden.Models;

namespace ChildrenGarden.Forms
{
    public partial class ChildrenTab : UserControl
    {
        private ListBox listBoxChildren;
        private TextBox textBoxName;
        private TextBox textBoxSurname;
        private DateTimePicker dateTimePickerDOB;
        private ComboBox comboBoxSex;
        private ComboBox comboBoxParent;
        private ComboBox comboBoxGroup;
        private Button buttonAdd;
        private Button buttonDelete; // Додано кнопку видалення

        public ChildrenTab()
        {
            InitializeComponents();
            LoadChildren();
            LoadParents();
            LoadGroups();
        }

        private void InitializeComponents()
        {
            listBoxChildren = new ListBox();
            textBoxName = new TextBox();
            textBoxSurname = new TextBox();
            dateTimePickerDOB = new DateTimePicker();
            comboBoxSex = new ComboBox();
            comboBoxParent = new ComboBox();
            comboBoxGroup = new ComboBox();
            buttonAdd = new Button();
            buttonDelete = new Button(); // Ініціалізація кнопки видалення

            int leftMargin = 10;
            int topMargin = 10;
            int labelWidth = 120;
            int controlWidth = 200;
            int controlHeight = 25;
            int spacing = 5;

            listBoxChildren.Location = new System.Drawing.Point(leftMargin, topMargin);
            listBoxChildren.Size = new System.Drawing.Size(300, 400);
            listBoxChildren.SelectedIndexChanged += ListBoxChildren_SelectedIndexChanged;

            int formLeft = leftMargin + 310;
            int currentTop = topMargin;

            var labelName = new Label();
            labelName.Text = "Ім'я:";
            labelName.Location = new System.Drawing.Point(formLeft, currentTop);
            labelName.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxName.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxName.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelSurname = new Label();
            labelSurname.Text = "Прізвище:";
            labelSurname.Location = new System.Drawing.Point(formLeft, currentTop);
            labelSurname.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxSurname.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxSurname.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelDOB = new Label();
            labelDOB.Text = "Дата народження:";
            labelDOB.Location = new System.Drawing.Point(formLeft, currentTop);
            labelDOB.Size = new System.Drawing.Size(labelWidth, controlHeight);
            dateTimePickerDOB.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            dateTimePickerDOB.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelSex = new Label();
            labelSex.Text = "Стать:";
            labelSex.Location = new System.Drawing.Point(formLeft, currentTop);
            labelSex.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxSex.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxSex.Size = new System.Drawing.Size(controlWidth, controlHeight);
            comboBoxSex.Items.AddRange(new string[] { "Чоловіча", "Жіноча" });
            currentTop += controlHeight + spacing;

            var labelParent = new Label();
            labelParent.Text = "Батько/Мати:";
            labelParent.Location = new System.Drawing.Point(formLeft, currentTop);
            labelParent.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxParent.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxParent.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelGroup = new Label();
            labelGroup.Text = "Група:";
            labelGroup.Location = new System.Drawing.Point(formLeft, currentTop);
            labelGroup.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxGroup.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxGroup.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            buttonAdd.Text = "Додати";
            buttonAdd.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Text = "Видалити";
            buttonDelete.Location = new System.Drawing.Point(formLeft + labelWidth + buttonAdd.Width + 10, currentTop);
            buttonDelete.Click += buttonDelete_Click;

            Controls.Add(listBoxChildren);
            Controls.Add(labelName);
            Controls.Add(textBoxName);
            Controls.Add(labelSurname);
            Controls.Add(textBoxSurname);
            Controls.Add(labelDOB);
            Controls.Add(dateTimePickerDOB);
            Controls.Add(labelSex);
            Controls.Add(comboBoxSex);
            Controls.Add(labelParent);
            Controls.Add(comboBoxParent);
            Controls.Add(labelGroup);
            Controls.Add(comboBoxGroup);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);
        }

        private void LoadChildren()
        {
            listBoxChildren.Items.Clear();
            List<ChildModel> children = ChildModel.GetAll();
            foreach (var child in children)
            {
                listBoxChildren.Items.Add(child);
            }
        }

        public void LoadParents()
        {
            comboBoxParent.Items.Clear();
            List<ParentModel> parents = ParentModel.GetAll();
            foreach (var parent in parents)
            {
                comboBoxParent.Items.Add(parent);
            }
        }

        public void LoadGroups()
        {
            comboBoxGroup.Items.Clear();
            List<GroupModel> groups = GroupModel.GetAll();
            foreach (var group in groups)
            {
                comboBoxGroup.Items.Add(group);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Перевірка на порожні поля
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxSurname.Text) ||
                comboBoxSex.SelectedItem == null ||
                comboBoxParent.SelectedItem == null ||
                comboBoxGroup.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, заповніть всі поля.");
                return;
            }

            var child = new ChildModel
            {
                Name = textBoxName.Text.Trim(),
                Surname = textBoxSurname.Text.Trim(),
                DateOfBirth = dateTimePickerDOB.Value,
                Sex = comboBoxSex.SelectedItem.ToString(),
                ParentId = ((ParentModel)comboBoxParent.SelectedItem).Idparents,
                GroupId = ((GroupModel)comboBoxGroup.SelectedItem).Idgroups
            };
            child.Add();
            LoadChildren();
            ClearForm();

            // Викликаємо подію для оновлення в інших вкладках
            ChildAdded?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxChildren.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, оберіть дитину для видалення.");
                return;
            }

            var selectedChild = (ChildModel)listBoxChildren.SelectedItem;

            // Перевірка наявності пов'язаних записів
            var attendanceRecords = Attendance.GetByChildId(selectedChild.ID);
            var paymentRecords = Payment.GetByChildId(selectedChild.ID);

            string message = "Ви впевнені, що хочете видалити цю дитину?";
            if (attendanceRecords.Count > 0 || paymentRecords.Count > 0)
            {
                message += "\nЦе також видалить всі записи відвідуваності та платежі цієї дитини.";
            }

            DialogResult result = MessageBox.Show(message, "Підтвердження видалення", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    selectedChild.Delete();
                    LoadChildren();
                    ClearForm();
                    MessageBox.Show("Дитину та пов'язані записи успішно видалено.");

                    // Викликаємо подію, якщо потрібна для оновлення в інших вкладках
                    ChildDeleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні дитини: {ex.Message}");
                }
            }
        }

        private void ClearForm()
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            dateTimePickerDOB.Value = DateTime.Now;
            comboBoxSex.SelectedIndex = -1;
            comboBoxParent.SelectedIndex = -1;
            comboBoxGroup.SelectedIndex = -1;
        }

        private void ListBoxChildren_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public event EventHandler ChildDeleted;

        public event EventHandler ChildAdded;
    }
}