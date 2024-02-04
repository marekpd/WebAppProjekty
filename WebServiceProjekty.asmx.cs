using System;
using System.Web.Services;
using System.IO;

namespace WebAppProjekty
{
    /// <summary>
    /// Summary description for WebServiceProjekty
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceProjekty : System.Web.Services.WebService
    {
        private static Projects m_projects = new ProjectsXml(); //samotne projekty

        /// <summary>
        /// Kde hladat "settings.xml".
        /// </summary>
        private static string getDataFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WebAppProjekty\"; //c:\ProgramData\WebAppProjekty\
        }
    
        /// <summary>
        /// Z relativnej cesty spravi absolutnu.
        /// </summary>
        private static string getFullPath(string fname, string currPath)
        {
            if (fname == null || fname == "") return "";
            return (Path.IsPathRooted(fname)) ? fname : Path.Combine(currPath, fname); //Path.GetFullPath()
        }

        /// <summary>
        /// Patri sa, aby sa klient na zaciatku pozdravil a povedal heslo (zaroven sa napr. nacita XML databaza ak este nie je).
        /// </summary>
        [WebMethod]
        public bool Hello(string password, out string errMsg)
        {
            try
            {
                string dataFolder = getDataFolder();
                if (!Directory.Exists(dataFolder)) throw new Exception("Data folder doesn't exist: '" + dataFolder + "'!");

                string fnSett = dataFolder + "settings.xml";
                Settings sett = new Settings(fnSett); //throw

                Logger.SetLogFilePath(getFullPath(sett["logging"], dataFolder));
                Logger.Log("Password=[" + password + "]");
                
                if (password != sett["password"]) throw new Exception("Incorrect password!");

                if (!m_projects.opened())
                {
                    string fnXml = getFullPath(sett["database"], dataFolder); //c:\ProgramData\WebAppProjekty\projects.xml 
                    if (!m_projects.open(fnXml)) throw new Exception("Can't open XML database: '" + fnXml + "'!"); //nacitanie projektov z XML subora (databazy)
                }

                Logger.Log("OK");
                errMsg = "";
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Error: " + e.Message);
                errMsg = e.Message;                
                return false;
            }
        }

        /// <summary>
        /// Patri sa, aby sa klient na konci tiez pozdravil (zaroven sa napr. ulozi XML databaza).
        /// </summary>
        [WebMethod]
        public bool Bye()
        {
            try
            {
                Logger.Log("Begin");

                if (m_projects.opened())
                {
                    if (!m_projects.flush()) throw new Exception("Can't backup and save XML file!"); //ulozenie projektov do XML subora (databazy)
                }

                Logger.Log("OK");
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Error: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Klientovi vrati pocet projektov (len pre informaciu).
        /// </summary>
        [WebMethod]
        public int GetProjectsCount()
        {
            return m_projects.Count;
        }

        /// <summary>
        /// Klientovi vrati ID vsetkych projektov.
        /// </summary>
        [WebMethod]
        public string[] GetProjectsIds()
        {
            Logger.Log(string.Format("Count={0}", m_projects.Count));
            return m_projects.getIds().ToArray();
        }

        /// <summary>
        /// Na zaklade ID vrati klientovi konkretny projekt.
        /// </summary>
        [WebMethod]
        public bool GetProject(string id, out string name, out string abbreviation, out string customer)
        {
            Logger.Log("Id=" + id);

            PrjInfo prjInfo = m_projects.getPrjInfo(id);
            if (prjInfo == null)
            {
                name = "";
                abbreviation = "";
                customer = "";

                Logger.Log("Not found");
                return false;
            }
            else
            {
                name = prjInfo.Name;
                abbreviation = prjInfo.Abbreviation;
                customer = prjInfo.Customer;

                Logger.Log("OK");
                return true;
            }
        }

        /// <summary>
        /// Prida novy projekt s unikatnym ID (od klienta).
        /// </summary>
        [WebMethod]
        public bool AddProject(string id, string name, string abbreviation, string customer)
        {
            Logger.Log("Id=" + id + ", Name=" + name + ", Abbreviation=" + abbreviation + ", Customer=" + customer);

            if (m_projects.addProject(id, new PrjInfo(name, abbreviation, customer)))
            {
                Logger.Log("OK");
                return true;
            }
            else
            {
                Logger.Log("Failed");
                return false;
            }
        }

        /// <summary>
        /// Aktualizuje existujuci projekt podla unikatneho ID (od klienta).
        /// </summary>
        [WebMethod]
        public bool UpdateProject(string id, string name, string abbreviation, string customer)
        {
            Logger.Log("Id=" + id + ", Name=" + name + ", Abbreviation=" + abbreviation + ", Customer=" + customer);
            if (m_projects.updatePrjInfo(id, new PrjInfo(name, abbreviation, customer)))
            {
                Logger.Log("OK");
                return true;
            }
            else
            {
                Logger.Log("Failed");
                return false;
            }
        }

        /// <summary>
        /// Odstrani existujuci projekt podla unikatneho ID (od klienta).
        /// </summary>
        [WebMethod]
        public bool RemoveProject(string id)
        {
            Logger.Log("Id=" + id);
            if (m_projects.removeProject(id))
            {
                Logger.Log("OK");
                return true;
            }
            else
            {
                Logger.Log("Failed");
                return false;
            }
        }

        /// <summary>
        /// Testovacia metoda c.1
        /// </summary>
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
            //return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }

        /// <summary>
        /// Testovacia metoda c.2
        /// </summary>
        [WebMethod]
        public int FindSubstringIndex(string str, string subs)
        {
            int index = str.IndexOf(subs, StringComparison.InvariantCultureIgnoreCase);
            return index;
        }

        /// <summary>
        /// Testovacia metoda c.3
        /// </summary>
        [WebMethod]
        public bool FindSubstringIndex2(string str, string subs, out int index)
        {
            index = str.IndexOf(subs, StringComparison.InvariantCultureIgnoreCase);
            return index >= 0;
        }
    }
}
