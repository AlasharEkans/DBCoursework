using System;
using System.Windows.Forms;
using ChildrenGarden.Models;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using ChildModel = ChildrenGarden.Models.Child;
using GroupModel = ChildrenGarden.Models.Group;
using System.Drawing.Drawing2D;

namespace ChildrenGarden.Forms
{
    public partial class AttendanceTab : UserControl
    {
        private DataGridView dataGridViewAttendance;
        private ComboBox comboBoxGroup; // Додано для вибору групи
        private ComboBox comboBoxChild;
        private DateTimePicker dateTimePickerDate;
        private ComboBox comboBoxStatus;
        private TextBox textBoxNotes;
        private Button buttonAdd;

        // Кнопки навігації між місяцями
        private Button buttonPreviousMonth;
        private Button buttonNextMonth;
        private Label labelCurrentMonth;

        private DateTime currentMonth;

        // Список дітей поточної групи
        private List<ChildModel> currentGroupChildren;

        public AttendanceTab()
        {
            currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            InitializeComponents();
            LoadGroups();
        }

        private void InitializeComponents()
        {
            dataGridViewAttendance = new DataGridView();
            comboBoxGroup = new ComboBox(); // Ініціалізація comboBoxGroup
            comboBoxChild = new ComboBox();
            dateTimePickerDate = new DateTimePicker();
            comboBoxStatus = new ComboBox();
            textBoxNotes = new TextBox();
            buttonAdd = new Button();

            buttonPreviousMonth = new Button();
            buttonNextMonth = new Button();
            labelCurrentMonth = new Label();

            int leftMargin = 10;
            int topMargin = 10;
            int labelWidth = 100;
            int controlWidth = 200;
            int controlHeight = 25;
            int spacing = 5;

            // Налаштування вибору групи
            var labelGroup = new Label();
            labelGroup.Text = "Група:";
            labelGroup.Location = new System.Drawing.Point(leftMargin, topMargin);
            labelGroup.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxGroup.Location = new System.Drawing.Point(leftMargin + labelWidth, topMargin);
            comboBoxGroup.Size = new System.Drawing.Size(controlWidth, controlHeight);
            comboBoxGroup.SelectedIndexChanged += ComboBoxGroup_SelectedIndexChanged;

            // Кнопки навігації по місяцях
            buttonPreviousMonth.Text = "<";
            buttonPreviousMonth.Location = new System.Drawing.Point(leftMargin + 390, topMargin);
            buttonPreviousMonth.Click += ButtonPreviousMonth_Click;

            buttonNextMonth.Text = ">";
            buttonNextMonth.Location = new System.Drawing.Point(leftMargin + 570, topMargin);
            buttonNextMonth.Click += ButtonNextMonth_Click;

            labelCurrentMonth.Text = currentMonth.ToString("MMMM yyyy");
            labelCurrentMonth.Location = new System.Drawing.Point(leftMargin + 470, topMargin);
            labelCurrentMonth.Size = new System.Drawing.Size(100, controlHeight);
            labelCurrentMonth.TextAlign = ContentAlignment.MiddleCenter;

            // Налаштування DataGridView
            dataGridViewAttendance.Location = new System.Drawing.Point(leftMargin, topMargin + controlHeight + spacing * 2);
            dataGridViewAttendance.Size = new System.Drawing.Size(800, 400);
            dataGridViewAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridViewAttendance.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewAttendance.AllowUserToResizeColumns = false;
            dataGridViewAttendance.AllowUserToResizeRows = false;

            dataGridViewAttendance.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewAttendance.ColumnHeadersHeight = 100; // Встановлюємо висоту для заголовків стовпців

            // Встановимо можливість редагування
            dataGridViewAttendance.ReadOnly = true;
            dataGridViewAttendance.AllowUserToAddRows = false;
            dataGridViewAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            int formLeft = leftMargin + 810;
            int currentTop = topMargin;

            var labelChild = new Label();
            labelChild.Text = "Дитина:";
            labelChild.Location = new System.Drawing.Point(formLeft, currentTop);
            labelChild.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxChild.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxChild.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelDate = new Label();
            labelDate.Text = "Дата:";
            labelDate.Location = new System.Drawing.Point(formLeft, currentTop);
            labelDate.Size = new System.Drawing.Size(labelWidth, controlHeight);
            dateTimePickerDate.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            dateTimePickerDate.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            var labelStatus = new Label();
            labelStatus.Text = "Статус:";
            labelStatus.Location = new System.Drawing.Point(formLeft, currentTop);
            labelStatus.Size = new System.Drawing.Size(labelWidth, controlHeight);
            comboBoxStatus.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            comboBoxStatus.Size = new System.Drawing.Size(controlWidth, controlHeight);
            comboBoxStatus.Items.AddRange(new string[] { "Присутній", "Відсутній" });
            currentTop += controlHeight + spacing;

            var labelNotes = new Label();
            labelNotes.Text = "Примітки:";
            labelNotes.Location = new System.Drawing.Point(formLeft, currentTop);
            labelNotes.Size = new System.Drawing.Size(labelWidth, controlHeight);
            textBoxNotes.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            textBoxNotes.Size = new System.Drawing.Size(controlWidth, controlHeight);
            currentTop += controlHeight + spacing;

            buttonAdd.Text = "Додати";
            buttonAdd.Location = new System.Drawing.Point(formLeft + labelWidth, currentTop);
            buttonAdd.Click += buttonAdd_Click;

            Controls.Add(labelGroup);
            Controls.Add(comboBoxGroup);
            Controls.Add(buttonPreviousMonth);
            Controls.Add(labelCurrentMonth);
            Controls.Add(buttonNextMonth);
            Controls.Add(dataGridViewAttendance);

            Controls.Add(labelChild);
            Controls.Add(comboBoxChild);
            Controls.Add(labelDate);
            Controls.Add(dateTimePickerDate);
            Controls.Add(labelStatus);
            Controls.Add(comboBoxStatus);
            Controls.Add(labelNotes);
            Controls.Add(textBoxNotes);
            Controls.Add(buttonAdd);
        }

        private void ComboBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChildren();
            LoadAttendance();
        }

        private void ButtonPreviousMonth_Click(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(-1);
            labelCurrentMonth.Text = currentMonth.ToString("MMMM yyyy");
            LoadAttendance();
        }

        private void ButtonNextMonth_Click(object sender, EventArgs e)
        {
            currentMonth = currentMonth.AddMonths(1);
            labelCurrentMonth.Text = currentMonth.ToString("MMMM yyyy");
            LoadAttendance();
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

        public void LoadChildren()
        {
            comboBoxChild.Items.Clear();
            currentGroupChildren = new List<ChildModel>();

            if (comboBoxGroup.SelectedItem != null)
            {
                var selectedGroup = (GroupModel)comboBoxGroup.SelectedItem;
                currentGroupChildren = ChildModel.GetByGroupId(selectedGroup.Idgroups);

                foreach (var child in currentGroupChildren)
                {
                    comboBoxChild.Items.Add(child);
                }
            }
        }

        public void LoadAttendance()
        {
            if (comboBoxGroup.SelectedItem == null)
            {
                dataGridViewAttendance.Columns.Clear();
                dataGridViewAttendance.Rows.Clear();
                return;
            }

            var selectedGroup = (GroupModel)comboBoxGroup.SelectedItem;
            currentGroupChildren = ChildModel.GetByGroupId(selectedGroup.Idgroups);

            // Визначаємо перший та останній день поточного місяця
            DateTime firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Отримуємо список дат в місяці
            var dates = Enumerable.Range(0, (lastDayOfMonth - firstDayOfMonth).Days + 1)
                                  .Select(offset => firstDayOfMonth.AddDays(offset))
                                  .ToList();

            // Очищаємо DataGridView
            dataGridViewAttendance.Columns.Clear();
            dataGridViewAttendance.Rows.Clear();

            // Додаємо перший стовпець з іменами дітей
            var nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "Дитина";
            nameColumn.Name = "ChildName";
            nameColumn.Width = 150;
            dataGridViewAttendance.Columns.Add(nameColumn);

            // Додаємо стовпці для кожної дати
            foreach (var date in dates)
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = date.ToString("dd.MM");
                column.Name = date.ToString("yyyyMMdd");
                column.Width = 30;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Повертаємо текст заголовку на 90 градусів
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                column.HeaderCell.Style.Font = new Font("Arial", 9, FontStyle.Regular);
                column.HeaderCell.Style.WrapMode = DataGridViewTriState.True;
                dataGridViewAttendance.Columns.Add(column);
            }

            // Повертаємо текст заголовків стовпців на 90 градусів
            dataGridViewAttendance.CellPainting += DataGridViewAttendance_CellPainting;
            dataGridViewAttendance.ColumnHeadersHeight = 100;
            dataGridViewAttendance.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Заповнюємо рядки
            foreach (var child in currentGroupChildren)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dataGridViewAttendance);
                row.Cells[0].Value = child.ToString();

                // Отримуємо відвідуваність для цієї дитини
                var attendanceList = Attendance.GetByChildId(child.ID);

                for (int i = 0; i < dates.Count; i++)
                {
                    var date = dates[i];
                    var attendance = attendanceList.FirstOrDefault(a => a.Date.Date == date);

                    if (attendance != null)
                    {
                        row.Cells[i + 1].Value = attendance.Status == "Присутній" ? "✔" : "✖";
                    }
                    else
                    {
                        row.Cells[i + 1].Value = "";
                    }
                }

                dataGridViewAttendance.Rows.Add(row);
            }
        }

        private void DataGridViewAttendance_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex > 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                GraphicsState state = e.Graphics.Save();
                e.Graphics.TranslateTransform(e.CellBounds.Left + e.CellBounds.Width / 2, e.CellBounds.Top + e.CellBounds.Height / 2);
                e.Graphics.RotateTransform(-90);
                string headerText = e.FormattedValue.ToString();
                SizeF textSize = e.Graphics.MeasureString(headerText, e.CellStyle.Font);
                e.Graphics.DrawString(headerText, e.CellStyle.Font, Brushes.Black, -textSize.Width / 2, -textSize.Height / 2);
                e.Graphics.Restore(state);
                e.Handled = true;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (comboBoxChild.SelectedItem == null || comboBoxStatus.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, заповніть всі поля.");
                return;
            }

            int selectedChildId = ((ChildModel)comboBoxChild.SelectedItem).ID;
            DateTime selectedDate = dateTimePickerDate.Value.Date;

            // Перевіряємо, чи існує запис
            var existingAttendance = Attendance.GetByChildIdAndDate(selectedChildId, selectedDate);

            if (existingAttendance != null)
            {
                if (existingAttendance.Status == comboBoxStatus.SelectedItem.ToString())
                {
                    MessageBox.Show("Статус вже встановлений на вибрану дату і не відрізняється.");
                    return;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Статус відвідуваності для цієї дитини на вибрану дату вже існує. Бажаєте його змінити?", "Підтвердження", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        // Оновлюємо статус
                        existingAttendance.Status = comboBoxStatus.SelectedItem.ToString();
                        existingAttendance.Notes = textBoxNotes.Text;
                        existingAttendance.Update();
                        LoadAttendance();
                        ClearForm();
                        MessageBox.Show("Статус відвідуваності було оновлено.");
                    }
                    else
                    {
                        return;
                    }

                    if (comboBoxChild.SelectedItem == null ||
                        comboBoxStatus.SelectedItem == null)
                    {
                        MessageBox.Show("Будь ласка, оберіть дитину та статус.");
                        return;
                    }
                }
            }
            else
            {
                // Додаємо новий запис
                var attendance = new Attendance
                {
                    ChildId = selectedChildId,
                    Date = selectedDate,
                    Status = comboBoxStatus.SelectedItem.ToString(),
                    Notes = textBoxNotes.Text
                };
                attendance.Add();
                LoadAttendance();
                ClearForm();
                MessageBox.Show("Запис відвідуваності було додано.");
            }
        }


        private void ClearForm()
        {
            comboBoxChild.SelectedIndex = -1;
            dateTimePickerDate.Value = DateTime.Now;
            comboBoxStatus.SelectedIndex = -1;
            textBoxNotes.Text = "";
        }
    }
}