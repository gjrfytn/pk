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

            Forms.AuthorizationForm form = new Forms.AuthorizationForm();
            if (form.ShowDialog() == DialogResult.OK)
                Application.Run(new Forms.MainForm(form.UsersRole));
        }
    }
}
