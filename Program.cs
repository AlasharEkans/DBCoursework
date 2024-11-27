using System;
using System.Windows.Forms;
using ChildrenGarden.Database;
using ChildrenGarden.Forms;

namespace ChildrenGarden
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Ініціалізація бази даних
            Database.Database.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}