using System;
using System.Runtime.CompilerServices;
using System.IO;

namespace WebAppProjekty
{
    /// <summary> 
    /// Trieda urcena na jednoduche logovanie do textoveho suboru. 
    /// </summary>
    public static class Logger
    {
        private static string m_fnLog;

        /// <summary> 
        /// Metoda na nastavenie mena logovacieho suboru. 
        /// </summary>
        public static void SetLogFilePath(string fnLog)
        {
            m_fnLog = fnLog;
        }

        /// <summary>
        /// Metoda na logovanie textu do suboru.
        /// </summary>
        public static void Log(string text, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) //[CallerFilePath] string filePath = "", 
        {
            if (m_fnLog == null || m_fnLog == "") return; //v takomto pripade sa neloguje

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //aktualny datum a cas
            string logLine = $"{timeStamp} - [{memberName}, {lineNumber}] - {text}"; //riadok na zapis

            try
            {
                using (StreamWriter writer = File.AppendText(m_fnLog)) //otvor textovy subor pre zapis (append)
                {
                    writer.WriteLine(logLine); //zapis logovaci zaznam
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Chyba pri logovani: {e.Message}");
            }
        }
    }
}
