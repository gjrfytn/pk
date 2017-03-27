using System;
using System.Windows.Forms;

namespace PK
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Forms.Authorization form = new Forms.Authorization();
            if (form.ShowDialog() == DialogResult.OK)
                Application.Run(new Forms.Main(form.UsersRole, form.UsersLogin));
        }
    }
}
