///  ( proba * 60 ) Puis mélanger  
using System;
using System.Xml;
using System.Collections.Generic;

class LireXml {
    

public LireXml(string file)
{

    int idTu = 0, idTe=0, idSl = 0, i=0,j=0,k=0;
    List<Slot> slot = new List<Slot>(); //.ToArray()
            //slot.Add pour ajouter un elem
    int[][]lien= new int [12][];
        for (int n = 0; n < 12; n++)
        {
            lien[n] = new int[12];
        }
    List<int> tab = new List<int>();
    string nomTe="",tmp ="";

    using (XmlReader reader = XmlReader.Create(@file)){
        while(reader.Read()){
            if(reader.IsStartElement()){
                switch (reader.Name.ToString())
                {
                   case "terrain":
                        reader.ReadStartElement("idTe"); 
                        idTe = Int32.Parse(reader.ReadString());
                        reader.ReadStartElement("NomTe"); //depend de l'écriture dans le fichier xml
                        nomTe = reader.ReadString();
                        reader.ReadEndElement();
                        break;

                    case "tuile":

                        reader.ReadStartElement("idTu"); 
                        idTu = Int32.Parse(reader.ReadString());

                        reader.ReadStartElement("slot");
                            int nbAttr = reader.AttributeCount;
                            

                            for (int attInd = 0; attInd < nbAttr-1; attInd++)
                            {                             
                            //Récupérer le tableau des positions internes (slot[])
                            while(reader.MoveToNextAttribute())
                                {
                                    tmp = reader.ReadString();
                                    if (tmp == "terrain")
                                    {
                                        reader.ReadStartElement("terrain");
                                        idTe = Int32.Parse(reader.ReadString());
                                        slot.Add(new Slot(idTe));

                                    }
                                    if (tmp == "idSl")
                                    {
                                        idSl = Int32.Parse(reader.ReadString());//id du slot

                                    }
                                    else
                                    {
                                        tab.Add(Tuile.PointsCardPos[tmp]);
                                        j++;
                                    }
                                    reader.ReadEndElement();

                                }

                            }           
                            
   
                            lien[idSl] = tab.ToArray();      //Ajouter le tableau des liens sémantique
                            Tuile t= new Tuile(idTu,slot.ToArray(),lien);
                            Tuile.DicoTuiles.Add(idTu,t); //Ajouter la tuile à la Dico

                            ///tab 
                            j = 0;
                        break;
                }

            }
        }
    }
}


}

