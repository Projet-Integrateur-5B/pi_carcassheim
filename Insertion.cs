using UnityEngine;
using Mono.Data.Sqlite; 
using System.Data; 
using System;

public class Dbtest : MonoBehaviour
{
   
   private void Start(){
       //mettre les fonctions ici 
       
   }
   int GetAge(string DateNaiss)
    {
        DateTime toDate = DateTime.Parse(DateNaiss);
        int age = DateTime.Now.Subtract(toDate).Days;
        age = age / 365;
        return age;
    }

   void Adduser(string Pseudo, string MDP, string Mail, string Photo, int XP, int Niveau, int Victoires, int Defaites, int Nbparties, string DateNaiss)
  {
     
        int age = GetAge(DateNaiss);
        if(age >= 13)
        {
         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "INSERT INTO Utilisateur (Pseudo,MDP,Mail,Photo,XP,Niveau,Victoires,Defaites,Nbparties,DateNaiss) VALUES('"+Pseudo+"','"+MDP+"','"+Mail+"','"+Photo+"','"+XP+"','"+Niveau+"','"+Victoires+"','"+Defaites+"','"+Nbparties+"','"++"')";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         
         
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
        }
        else
        {
            // à remplir
        }
     }
// focntion d'ajout de l'extension
     void AddExtension(string Nom)
  {
     

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "INSERT INTO Extension (Nom) VALUES('"+Nom+"')";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         
         
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
     }
    // fonction d'ajout de modèle
     void AddModele(string Nom, int IDE,int Proba)
  {
     

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "INSERT INTO Extension (Proba,Nom,IDE) VALUES('"+Proba+"','"+Nom+"','"+IDE+"')";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         
         
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
     }

     // focntion d'ajout de la tuile  
     void AddTuile( int IDM ,  string Image)
  {
     

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "INSERT INTO Tuile (IDM,Image) VALUES('"+IDM+"','"+Image+"')";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         
         
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
     }
     
      // focntion d'ajout de la partie 
     void AddPartie(int Moderateur ,  string Statut,  int NbMaxJ ,  string Prive,  int Timer, int TMaxJ, int Meeples )
  {
     

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection 
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "INSERT INTO Partie (Moderateur,Statut,NbMaxJ,Prive,Timer,TMaxJ,Meeples) VALUES('"+Moderateur+"','"+Statut+"','"+NbMaxJ+"','"+Prive+"','"+Timer+"','"+TMaxJ+"','"+Meeples+"')";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         
         
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
     }
     
       // focntion d'ajout de la fonction addtext
     void AddExt( int IDE)
  {
     

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "INSERT INTO Extension (IDE) VALUES('"+IDE+"')";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         
         
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
     }
     
    void ReadDatabase () {

         string conn = "URI=file:" + Application.dataPath + "/projet.db"; //Path to database.
         IDbConnection dbconn;
         dbconn = (IDbConnection) new SqliteConnection(conn);
         dbconn.Open(); //Open connection to the database.
         IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "SELECT IDE,Nom FROM Extension";
         dbcmd.CommandText = sqlQuery;
         IDataReader reader = dbcmd.ExecuteReader();
         while (reader.Read())
         {
             int IDE = reader.GetInt32(0);
             string Nom = reader.GetString(1);
            
            
             Debug.Log( "IDE: " +IDE + " Nom:  " +Nom);
         }
         reader.Close();
         reader = null;
         dbcmd.Dispose();
         dbcmd = null;
         dbconn.Close();
         dbconn = null;
     }
     
     
     }

     
