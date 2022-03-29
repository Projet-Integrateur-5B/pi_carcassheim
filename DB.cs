using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

private string commandResult = "";

public class DB : MonoBehaviour
{
    private string dbName = "URI=file:Assets/projet.db";

    // Start is called before the first frame update
    void Start()
    {}
    // Update is called once per frame
    void Update()
    {}
  
/* -------------------------------- COMMAND --------------------------------- */
    public void command(string s)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = s;
                cmd.ExecuteNonQuery();
                try { commandResult = cmd.ExecuteScalar().ToString(); }
                catch {Console.Write("Erreur :" + commandResult);} 
            }
            connection.Close();
        }
    }

    public string getCommandResult()
    {
        return commandResult;
    }
/* -------------------------------------------------------------------------- */

    /// <summary>
    /// Fonction qui incremente le nombre de parties jouees par un utilisateur
    /// appelee systematiquement e la fin de chaque partie pour tous les
    /// joueurs
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    public void IncrementeNbParties(int IDU)
    {
        string s = "update Utilisateur set NbParties =  NbParties + 1 where IDU = " + IDU +";";
        command(s);
    }

    /// <summary>
    /// Fonction qui incremente le nombre de parties gagnees par un utilisateur le cas echaeant
    /// </summary>
    /// <param name="IDU">Identifiant du vainqueur</param>
    public void IncrementeVictoires(int IDU)
    {
        string s = "update Utilisateur set victoires =  victoires + 1 where IDU = " + IDU + ";";
        command(s);
    }

    /// <summary>
    /// Fonction qui incremente le nombre de parties perdues par un utilisateur le cas echaeant
    /// </summary>
    /// <param name="IDU">Identifiant du vaincu</param>
    public void IncrementeDefaites(int IDU)
    {
        string s = "update Utilisateur set defaites =  defaites + 1 where IDU = " + IDU + ";";
        command(s);
    }

    /// <summary>
    /// Fonction renvoyant l'XP de'un utilisateur
    /// Elle est essentiellement une fonction intermediaire pour
    /// incrementation de l'XP d'un utilisateur pour ne pas
    /// executer deux commandes SQLite en meme temps
    /// et bloquer la BDD
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    /// <returns></returns>
    public int GetXP(int IDU)
    {
        var connection = new SqliteConnection(dbName);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "select XP from Utilisateur where IDU = " + IDU + ";";
        int XP = Convert.ToInt32(cmd.ExecuteScalar());
        connection.Close();
        return XP;
    }

    /// <summary>
    /// Fonction qui incremente l'XP et le cas echeant le niveau d'un utilisateur qui arrive e XP
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    /// <param name="XP">Le nombre de points d'experience e rajouter e l'utilisateur</param>
    public void AddXP(int IDU, int XP)
    {
        int CurExp = GetXP(IDU);
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                int lvl = (CurExp + XP) / 100;
                print(lvl);
                CurExp = (CurExp + XP) % 100;
                print(CurExp);
                cmd.CommandText = "update Utilisateur set XP = " + CurExp + ", Niveau = Niveau + " + lvl + " where IDU = " + IDU + ";";
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    int GetAge(string DateNaiss)
    {
        DateTime toDate = DateTime.Parse(DateNaiss);
        int age = DateTime.Now.Subtract(toDate).Days;
        age = age / 365;
        return age;
    }

    //  Renvoie une ID libre pour générer une nouvelle partie
    public int idPartieLibre()
    {
        string s = "select count(*) from table Partie;";
        command(s);
        return Int32.Parse(getCommandResult());
    }


/* ------------------------------ RECUPERATION INFO COMPTE --------------------------- */
    //Pseudo
    public string Pseudo(int IDU)
        {
            string s = "select Pseudo from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return Parse(getCommandResult());
        }
    //Photo
    public string Photo(int IDU)
        {
            string s = "select Photo from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return Parse(getCommandResult());
        }
    //Victoires
    public int Victoires(int IDU)
        {
            string s = "select Victoires from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return Int32.Parse(getCommandResult());
        }
    //Niveau
    public int Niveau(int IDU)
        {
            string s = "select Niveau from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return Int32.Parse(getCommandResult());
        }
    //Defaites
    public int Defaites(int IDU)
        {
            string s = "select Defaites from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return Int32.Parse(getCommandResult());
        }
    //NbParties
    public int NbParties(int IDU)
        {
            string s = "select NbParties from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return Int32.Parse(getCommandResult());
        }

/* -------------------------------------------------------------------------- */



/* ---------------------------------- DROP ---------------------------------- */
    public void Drop(string tableName)
    {
        string s = "DROP Table '" + tableName + "';";
        command(s);
    }

    public void DropPartie()
    {
        Drop("PartieExt");
        Drop("Partie");
    }
/* -------------------------------------------------------------------------- */
}