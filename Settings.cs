using System.Xml;

namespace WebAppProjekty
{
    /// <summary> 
    /// Trieda urcena na jednoduche citanie nastaveni z XML suboru. 
    /// </summary>
    public class Settings
    {
        private XmlDocument m_doc = new XmlDocument();

        /// <summary>
        /// Konstruktor precita nastavenia zo suboru.
        /// </summary>
        public Settings(string fnXml)
        {
            m_doc.Load(fnXml);
        }

        /// <summary>
        /// Indexer spristupni hodnotu konkretneho nastavenia.
        /// </summary>
        public string this[string key]
        {
            get
            {
                XmlNode node = m_doc.SelectSingleNode("/settings/" + key);
                if (node != null)
                {
                    XmlAttribute attr = node.Attributes["value"];
                    if (attr != null) return attr.Value;
                }
                return ""; //default
            }
        }
    }
}
