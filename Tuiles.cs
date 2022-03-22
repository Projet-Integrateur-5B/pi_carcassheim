using UnityEngine;
using Mono.Data.Sqlite; 
using System.Data; 
using System;
using System.Collections.Generic;

public class Dbtest : MonoBehaviour
{
    public void Start(){
    Dictionary<int, int> dico_test = new Dictionary<int, int>();
    RemplirTuiles(dico_test);
    AfficherTuile(dico_test);
    
    }
   void RemplirTuiles (Dictionary <int, int> dico) {


         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "SELECT T.IDT,M.Proba FROM Tuile T,Modele M where T.IDM = M.IDM";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
       
            
            foreach (var item in dico)
            {
                reader.Read();
                dico.Key=reader.GetInt32(0);
                dico.Value=reader.GetInt32(1);
                
            }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
     }
     
    void AfficherTuile(Dictionary <int, int> dico)
    {
        foreach(var i in dico){
            Debug.Log(i.Key+i.Value);
        }
    }
     
     }

     
