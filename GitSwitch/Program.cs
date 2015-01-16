using System;
using System.Windows.Forms;

namespace GitSwitch
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            CustomApplicationContext context = new CustomApplicationContext();
            Application.Run(context);
            
        }
    }
}
