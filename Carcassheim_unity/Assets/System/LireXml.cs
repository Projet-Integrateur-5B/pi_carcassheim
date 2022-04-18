///  ( proba * 60 ) Puis mélanger  
using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
class LireXml : MonoBehaviour
{


    public LireXml(string file)
    {
        int idTu = 0, idTe = 0, idSl = 0, i = 0, j = 0, nbPos = 0;
        List<Slot> slot = new List<Slot>(); 
       
        int[][] lien = new int[12][];
        for (int n = 0; n < 12; n++)
        {
            lien[n] = new int[12];
        }
        List<int> tab = new List<int>();
        string nomTe = "", tmp = "";

        using (XmlReader reader = XmlReader.Create(@file))
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToString())
                    {
                        case "terrain":
                            reader.ReadToFollowing("idTe");
                            idTe = int.Parse(reader.ReadString());
                            reader.ReadToFollowing("nomTe"); //depend de l'écriture dans le fichier xml
                            nomTe = reader.ReadString();
                            Debug.Log("Terrain : ");
                            Debug.Log(idTe);
                            Debug.Log(nomTe);

                            break;

                        case "tuile":

                            reader.ReadToFollowing("idTu");
                            idTu = int.Parse(reader.ReadString());
                            
                            reader.ReadToFollowing("nbSlots");
                            j = Int32.Parse(reader.ReadString());

                            for (i = j; i > 0; i--)
                            {
                                reader.ReadToFollowing("slot");
                                reader.ReadToFollowing("idSl");
                                idSl = Int32.Parse(reader.ReadString());//id du slot

                                reader.ReadToFollowing("nbPositions");
                                nbPos = Int32.Parse(reader.ReadString());

                                //Récupérer le tableau des positions internes (slot[])
                                reader.ReadToFollowing("postionsSlot");
                                tmp = reader.ReadString();
                                string[] subs = tmp.Split(',', '.','\0');
                                foreach (string sub in subs)
                                {
                                   if (sub == "N" || sub == "NNO" || sub == "NNE" || sub == "NEE" || sub == "E" || sub == "SEE" || sub == "S" || sub == "SSO" || sub == "SOO" || sub == "O" || sub == "NOO")
                                    {
                                        tab.Add(Tuile.PointsCardPos[sub]);
                                    }
                                }
                                
                                reader.ReadToFollowing("terrain");
                                idTe = Int32.Parse(reader.ReadString());
                                slot.Add(new Slot(idTe));
                            }
                            lien[idSl] = tab.ToArray();      //Ajouter le tableau des liens sémantiques
                            Tuile t = new Tuile(idTu, slot.ToArray(), lien);
                            Tuile.DicoTuiles.Add(idTu, t); //Ajouter la tuile à la Dico
                            break;
                    }
                }
                //reader.ReadEndElement();
            }
        }
    }
   /*
    void Start()
    {
        String file = "Assets/system/infos.xml";
        LireXml l = new LireXml(file);
    }
   */
}

