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
            //try
            //{
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Forms.Authorization form = new Forms.Authorization();
            if (form.ShowDialog() == DialogResult.OK)
                Application.Run(new Forms.Main(form.UsersRole, form.UsersLogin));
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message + "\n\nПриложение будет закрыто.", "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt", true))
            //        writer.Write("\n\n" + DateTime.Now.ToString() + "\n" + e.ToString());
            //}
        }
    }
}
