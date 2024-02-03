using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace WebAppProjekty
{
    public class ProjectsXml: Projects
    {
        private Dictionary<string, PrjInfo> m_prjDict = new Dictionary<string, PrjInfo>(); //slovnik projektov, kde kluc je unikatne ID projektu
        private string m_fnXml = ""; //XML subor ako zdroj dat

        /// <summary>
        /// Pocet projektov.
        /// </summary>
        public override int Count { get { return m_prjDict.Count; } }

        /// <summary>
        /// Vrati unikatne ID vsetkych projektov.
        /// </summary>
        public override List<string> getIds()
        {
            List<string> ids = new List<string>();
            foreach (var entry in m_prjDict)
            {
                ids.Add(entry.Key);
            }
            return ids;
        }

        /// <summary>
        /// Na zaklade ID vrati ostatne informacie o projekte (PrjInfo alebo null).
        /// </summary>
        public override PrjInfo getPrjInfo(string id)
        {
            return m_prjDict.ContainsKey(id) ? m_prjDict[id] : null;
        }

        /// <summary>
        /// Prida novy projekt (ID + PrjInfo).
        /// </summary>
        public override bool addProject(string id, PrjInfo prjInfo)
        {
            if (m_prjDict.ContainsKey(id))
            {
                return false;
            }
            else
            {
                m_prjDict.Add(id, prjInfo);
                return true;
            }
        }

        /// <summary>
        /// Aktualizuje existujuci projekt (ID + PrjInfo).
        /// </summary>
        public override bool updatePrjInfo(string id, PrjInfo prjInfo)
        {
            if (m_prjDict.ContainsKey(id))
            {
                m_prjDict[id] = prjInfo;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Odstrani projekt na zaklade unikatneho ID.
        /// </summary>
        public override bool removeProject(string id)
        {
            return m_prjDict.Remove(id);
        }

        /// <summary>
        /// Zisti, ci je databaza otvorena (ci je XML subor nacitany v pamati).
        /// </summary>
        public override bool opened()
        {
            return m_fnXml != "";
        }
        /// <summary>
        /// Otvori databazu (nacita XML subor do pamati).
        /// </summary>
        public override bool open(string fname)
        {
            try
            {
                m_prjDict.Clear();
                loadFromXml(fname);
                m_fnXml = fname;
                return true;
            }
            catch (Exception e)
            {
                m_prjDict.Clear();
                m_fnXml = "";
                return false;
            }
        }
        
        /// <summary>
        /// Zabezpeci perzistenciu dat (ulozi projekty z pamati do XML suboru, ktory predtym zazalohuje).
        /// </summary>
        /// <returns></returns>
        public override bool flush()
        {
            if (!opened()) return false;

            try {
                string fnXmlBak = Path.GetDirectoryName(m_fnXml) + "\\" + Path.GetFileNameWithoutExtension(m_fnXml) + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + Path.GetExtension(m_fnXml) + ".bak";
                File.Move(m_fnXml, fnXmlBak); //zaloha povodneho XML subora (databazy)
                saveToXml(m_fnXml); //ulozenie projektov do XML subora (databazy)
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private void loadFromXml(string fnXml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fnXml);

            foreach (XmlNode projectNode in xmlDoc.SelectNodes("//project"))
            {
                // Ziskanie atributu 'id' z elementu 'project'
                string id = projectNode.Attributes["id"].Value;

                // a tiez InnerText z pod-elementov
                string name = projectNode.SelectSingleNode("name").InnerText;
                string abbreviation = projectNode.SelectSingleNode("abbreviation").InnerText;
                string customer = projectNode.SelectSingleNode("customer").InnerText;

                m_prjDict[id] = new PrjInfo(name, abbreviation, customer);
            }
        }

        private static void addNewElementInnerText(XmlDocument xmlDoc, XmlElement parent, string newElementName, string newElementInnerText)
        {
            XmlElement element = xmlDoc.CreateElement(newElementName);
            element.InnerText = newElementInnerText;
            parent.AppendChild(element);
        }

        private void saveToXml(string fnXml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "windows-1250", null);
            xmlDoc.AppendChild(xmlDeclaration);

            // Vytvorenie koreňového elementu 'projects'
            XmlElement projectsElement = xmlDoc.CreateElement("projects");
            xmlDoc.AppendChild(projectsElement);

            // Prechádzanie Dictionary a vytvorenie elementov pre každý projekt
            foreach (var prjEntry in m_prjDict)
            {
                // Vytvorenie elementu 'project' s atribútom 'id'
                XmlElement prjElement = xmlDoc.CreateElement("project");
                prjElement.SetAttribute("id", prjEntry.Key);

                // Pridanie pod-elementov s InnerText-ami
                addNewElementInnerText(xmlDoc, prjElement, "name"        , prjEntry.Value.Name);
                addNewElementInnerText(xmlDoc, prjElement, "abbreviation", prjEntry.Value.Abbreviation);
                addNewElementInnerText(xmlDoc, prjElement, "customer"    , prjEntry.Value.Customer);

                // Pripojenie elementu 'project' do elementu 'projects'
                projectsElement.AppendChild(prjElement);
            }

            // Ulozenie XmlDocument do suboru
            //xmlDoc.Save(fnXml);

            using (XmlTextWriter writer = new XmlTextWriter(fnXml, Encoding.GetEncoding("windows-1250")))
            {
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save(writer);
            }
        }

        public void print() //kontrolny vypis
        {
            int i = 0;

            foreach (var prjEntry in m_prjDict)
            {
                Console.WriteLine($"{++i})");
                Console.WriteLine($"* Project ID: {prjEntry.Key}");
                Console.WriteLine($"        Name: {prjEntry.Value.Name}");
                Console.WriteLine($"Abbreviation: {prjEntry.Value.Abbreviation}");
                Console.WriteLine($"    Customer: {prjEntry.Value.Customer}");
                Console.WriteLine();
            }
        }
    }
}
