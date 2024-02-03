using System.Collections.Generic;

namespace WebAppProjekty
{
    //Priklad struktury XML databazy:
    //<? xml version="1.0" encoding="windows-1250"?>
    //<projects>
    //    <project id = "prj1" >
    //    <name>Informačný systém firmy ABC</name>
    //    <abbreviation>IS-ABC</abbreviation>
    //    <customer>ABC, s.r.o.</customer>
    //</project>

    public class PrjInfo
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Customer { get; set; }
        public PrjInfo(string name, string abbreviation, string customer)
        {
            Name = name;
            Abbreviation = abbreviation;
            Customer = customer;
        }
    }

    public abstract class Projects
    {
        public abstract int Count { get; } //Pocet projektov.
        public abstract List<string> getIds(); //Vrati unikatne ID vsetkych projektov.
        public abstract PrjInfo getPrjInfo(string id); //Na zaklade ID vrati ostatne informacie o projekte (PrjInfo alebo null).
        public abstract bool addProject(string id, PrjInfo prjInfo); //Prida novy projekt (ID + PrjInfo).
        public abstract bool updatePrjInfo(string id, PrjInfo prjInfo); //Aktualizuje existujuci projekt(ID + PrjInfo).
        public abstract bool removeProject(string id); //Odstrani projekt na zaklade unikatneho ID.
        public abstract bool opened(); //Zisti, ci je databaza otvorena.
        public abstract bool open(string fname); //Otvori databazu.
        public abstract bool flush(); //Dodatocne zabezpeci perzistenciu dat, ak treba.
    }
}
