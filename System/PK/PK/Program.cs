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

            AuthorizationForm form = new AuthorizationForm();
            if (form.ShowDialog() == DialogResult.OK)
                Application.Run(new MainForm(form.UsersRole));
        }
    }
}
