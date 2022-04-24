using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using UnityEngine;

namespace Assets.system
{
    internal class LireXML2
    {
        static Dictionary<string, int> Positions;
        public static Dictionary<int, TypeTerrain> IdVersTerrain;
        static Dictionary<string, TypeTerrain> DictionaireTemp;
        static LireXML2()
        {
            Positions = new Dictionary<string, int>
            {
                {"nno", 0 },
                {"n", 1 },
                {"nne", 2 },
                {"nee", 3 },
                {"e", 4 },
                {"see", 5 },
                {"sse", 6 },
                {"s", 7 },
                {"sso", 8 },
                {"soo", 9 },
                {"o", 10 },
                {"noo", 11 }
            };

            IdVersTerrain = new Dictionary<int, TypeTerrain>();

            DictionaireTemp = new Dictionary<string, TypeTerrain>()
            {
                { "abbaye", TypeTerrain.Abbaye },
                { "ville_simple", TypeTerrain.Ville },
                { "ville_blason", TypeTerrain.VilleBlason },
                { "champs", TypeTerrain.Pre },
                { "chemin", TypeTerrain.Route },
                { "riviere", TypeTerrain.Riviere }
            };
        }
        public static Dictionary<int, Tuile> Read(string file)
        {
            var result = new Dictionary<int, Tuile>();

            using (XmlReader reader = XmlReader.Create(Application.streamingAssetsPath + "/" + @file))
            {
                ReadTerrain(reader);
                while (reader.Read())
                {
                    bool readingId = false;
                    int currentId = 0;
                    List<List<int>> lien = new List<List<int>>();
                    List<Slot> slots = new List<Slot>();
                    Tuile current;
                    int lienPtr = 0;
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "tuile":
                                lien = new List<List<int>>();
                                slots = new List<Slot>();
                                lienPtr = 0;

                                
                                break;
                            case "id":
                                    readingId = true;
                                break;
                            case "slot":
                                Slot tempSlot;
                                var tempLien = ReadSlot(reader, out tempSlot);
                                lien.Add(tempLien);
                                slots.Add(tempSlot);
                                lienPtr++;
                                break;
                            default:
                                break;
                        }
                            break;
                        case XmlNodeType.Text:
                            if (readingId)
                                currentId = int.Parse(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "tuile")
                            {
                                var temp = new List<int[]>();
                                foreach (var item in lien)
                                {
                                    temp.Add(item.ToArray());
                                }
                                current = new Tuile((ulong)currentId, slots.ToArray(), temp.ToArray());
                                result.Add(currentId, current);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private static void ReadTerrain(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                bool readingId = false;
                bool readingNom = false;
                int currentId = 0;
                string currentNom = "";
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xmlReader.Name)
                        {
                            case "id":
                                readingId = true;
                                break;
                            case "nom":
                                readingNom = true;
                                break;
                            default:
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        readingId = false;
                        readingNom = false;
                        if (xmlReader.Name == "terrain")
                            IdVersTerrain.Add(currentId, DictionaireTemp[currentNom]);
                        break;
                    case XmlNodeType.Text:
                        if (readingId)
                            currentId = int.Parse(xmlReader.Value);
                        if (readingNom)
                            currentNom = xmlReader.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        private static List<int> ReadSlot(XmlReader xmlReader, out Slot slot)
        {
            var result = new List<int>();
            bool stop = false;
            int idTerrain = 0;
            string nodeName = "";
            bool goNext = false;
            while (!stop && xmlReader.Read())
            {
                if (goNext)
                {
                    goNext = false;
                    continue;
                }
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xmlReader.Name)
                        {
                            case "id":
                                nodeName = "id";
                                break;
                            case "slot":
                                stop = true;
                                break;
                            case "terrain":
                                nodeName = "terrain";
                                break;
                            default:
                                result.Add(Positions[xmlReader.Name]);
                                goNext = true;
                                break;
                        }
                        break;
                    case XmlNodeType.Text:
                        switch (nodeName)
                        {
                            case "id":
                                break;
                            case "terrain":
                                idTerrain = int.Parse(xmlReader.Value);
                                break;
                            default:
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (xmlReader.Name == "slot")
                            stop = true;
                        break;
                    default:
                        break;
                }
            }

            slot = new Slot((ulong)idTerrain);

            return result;
        }
    }
}
