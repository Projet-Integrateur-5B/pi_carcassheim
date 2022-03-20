using UnityEngine;
using Mono.Data.Sqlite; 
using System.Data; 
using System;

public class Dbtest : MonoBehaviour
{
   
   bool Identification (string login, string mdp) {

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "SELECT Pseudo,MDP FROM Utilisateur";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         while (reader.Read())
         {
             string Pseudo = reader.GetString(0);
             string Nom = reader.GetString(1);
            
            if(login == Pseudo && mdp == Nom){
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            return true;
            }
           
         }
         
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                return false;
     }
     

     
     }

     
