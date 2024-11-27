using System;
using System.Windows.Forms;
using ChildrenGarden.Forms;

namespace ChildrenGarden
{
    public partial class MainForm : Form
    {
        private TabControl tabControl;
        private ParentsTab parentsTab;
        private ChildrenTab childrenTab;
        private GroupsTab groupsTab;
        private StaffTab staffTab;
        private AttendanceTab attendanceTab;
        private PaymentsTab paymentsTab;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Width = 1296;
            this.Height = 768;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            var parentsTabPage = new TabPage("Батьки");
            var childrenTabPage = new TabPage("Діти");
            var groupsTabPage = new TabPage("Групи");
            var staffTabPage = new TabPage("Персонал");
            var attendanceTabPage = new TabPage("Відвідуваність");
            var paymentsTabPage = new TabPage("Платежі");

            parentsTab = new ParentsTab();
            parentsTab.Dock = DockStyle.Fill;
            parentsTab.ParentAdded += ParentsTab_ParentAdded;
            parentsTabPage.Controls.Add(parentsTab);

            childrenTab = new ChildrenTab();
            childrenTab.Dock = DockStyle.Fill;
            childrenTab.ChildAdded += ChildrenTab_ChildAdded;
            childrenTabPage.Controls.Add(childrenTab);

            groupsTab = new GroupsTab();
            groupsTab.Dock = DockStyle.Fill;
            groupsTab.GroupAdded += GroupsTab_GroupChanged; // Підписуємося на події
            groupsTab.GroupDeleted += GroupsTab_GroupChanged; // Підписуємося на події
            groupsTabPage.Controls.Add(groupsTab);

            staffTab = new StaffTab();
            staffTab.Dock = DockStyle.Fill;
            staffTab.StaffAdded += StaffTab_StaffAdded;
            staffTabPage.Controls.Add(staffTab);

            attendanceTab = new AttendanceTab();
            attendanceTab.Dock = DockStyle.Fill;
            attendanceTabPage.Controls.Add(attendanceTab);

            paymentsTab = new PaymentsTab();
            paymentsTab.Dock = DockStyle.Fill;
            paymentsTabPage.Controls.Add(paymentsTab);

            tabControl.TabPages.Add(parentsTabPage);
            tabControl.TabPages.Add(childrenTabPage);
            tabControl.TabPages.Add(groupsTabPage);
            tabControl.TabPages.Add(staffTabPage);
            tabControl.TabPages.Add(attendanceTabPage);
            tabControl.TabPages.Add(paymentsTabPage);

            Controls.Add(tabControl);

            Text = "Дитячий Садок - Управління";

            this.MaximizeBox = false;
        }

        private void GroupsTab_GroupChanged(object sender, EventArgs e)
        {
            // Оновлюємо список груп у вкладці ChildrenTab та AttendanceTab
            childrenTab.LoadGroups();
            attendanceTab.LoadGroups();
        }

        private void ParentsTab_ParentAdded(object sender, EventArgs e)
        {
            childrenTab.LoadParents();
            paymentsTab.LoadParents();
        }

        private void GroupsTab_GroupAdded(object sender, EventArgs e)
        {
            childrenTab.LoadGroups();
        }

        private void StaffTab_StaffAdded(object sender, EventArgs e)
        {
            groupsTab.LoadStaff();
        }

        private void ChildrenTab_ChildAdded(object sender, EventArgs e)
        {
            attendanceTab.LoadChildren();
            paymentsTab.LoadChildren();
        }
    }
}