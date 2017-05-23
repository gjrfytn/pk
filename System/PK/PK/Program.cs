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
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Forms.Authorization form = new Forms.Authorization();
            if (form.ShowDialog() == DialogResult.OK)
                Application.Run(new Forms.Main(form.UsersRole, form.UsersLogin));

            Application.ThreadException -= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Приложение продолжит работу. Сообщите администратору о возникновении ошибки.", "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt", true))
                writer.Write("\n\n" + DateTime.Now.ToString() + "\n" + e.Exception.ToString());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show("Приложение будет закрыто.", "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt", true))
                    writer.Write("\n\n" + DateTime.Now.ToString() + " CRITICAL ERROR \n" + e.ExceptionObject.ToString());
            }
        }
    }
}
