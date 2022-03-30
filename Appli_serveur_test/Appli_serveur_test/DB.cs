namespace Server;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class DB
{
    private string dbName;
    private string commandResult = "No Result";
    public DB()
    {
        string path = System.Reflection.Assembly.GetCallingAssembly().Location;
        dbName = Path.GetDirectoryName(path);
        int size = dbName.Length;
        dbName = @"URI=file:" + dbName.Substring(0, size - 17) + @"\projet.db";
        Console.WriteLine(dbName);
    }

    public DB(string path)
    {
        dbName = path;
    }

    public SqliteConnection Connect()
    {
        try
        {
            SqliteConnection connection = new SqliteConnection(dbName);
            return connection;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERREUR: echec de l'ouverture : " + ex);

            return null;
        }
    }

    /* -------------------------------- COMMAND --------------------------------- */
    public string command(string s)
    {
        SqliteConnection connection = this.Connect();
        connection.Open();

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = s;
            cmd.ExecuteNonQuery();
            try
            {
                return cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                Console.Write("Erreur : commande : " + s + " " + ex);
                return "";
            }
        }
        connection.Close();
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
                CurExp = (CurExp + XP) % 100;
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
            return getCommandResult();
        }
    //Photo
    public string Photo(int IDU)
        {
            string s = "select Photo from table Utilisateur where IDU = " + IDU +";";
            command(s);
            return getCommandResult();
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


    bool Identification(string login, string mdp)
    {
        bool res = false;

        SqliteConnection connection = this.Connect();
        connection.Open();
        
        IDbCommand cmd = connection.CreateCommand();
        string sqlQuery = "SELECT Pseudo,MDP FROM Utilisateur";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string Pseudo = reader.GetString(0);
            string Nom = reader.GetString(1);

            if (login == Pseudo && mdp == Nom)
            {
                res = true;
                break;
            }

        }
        reader.Close();
        cmd.Dispose();
        connection.Close();
        return res;
    }

    public bool Adduser(string Pseudo, string MDP, string Mail, string Photo, int XP, int Niveau, int Victoires, int Defaites, int Nbparties, string DateNaiss)
    {
        bool res = false;
        int age = GetAge(DateNaiss);

        if (age >= 13)
        {
            SqliteConnection connection = this.Connect();
            connection.Open();

            var cmd = connection.CreateCommand();
            string sqlQuery = "INSERT INTO Utilisateur (Pseudo,MDP,Mail,Photo,XP,Niveau,Victoires,Defaites,Nbparties,DateNaiss) VALUES('" + Pseudo + "','" + MDP + "','" + Mail + "','" + Photo + "','" + XP + "','" + Niveau + "','" + Victoires + "','" + Defaites + "','" + Nbparties + "','" + DateNaiss + "');";
            cmd.CommandText = sqlQuery;
            var reader = cmd.ExecuteReader();

            reader.Close();
            connection.Close();

            //Insertion ok 
            res = true;
        }
        else
        {
            Console.Write("Erreur : Age : " + age);
        }
        return res;
    }

    public void AddExtension(string Nom)
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();
        string sqlQuery = "INSERT INTO Extension (Nom) VALUES('" + Nom + "')";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();


        reader.Close();
        cmd.Dispose();
        connection.Close();
    }
    // fonction d'ajout de modèle
    public void AddModele(string Nom, int IDE, int Proba)
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();

        string sqlQuery = "INSERT INTO Extension (Proba,Nom,IDE) VALUES('" + Proba + "','" + Nom + "','" + IDE + "')";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();


        reader.Close();
        cmd.Dispose();
        connection.Close();
    }

    // focntion d'ajout de la tuile  
    public void AddTuile(int IDM, string Image)
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();

        string sqlQuery = "INSERT INTO Tuile (IDM,Image) VALUES('" + IDM + "','" + Image + "')";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();


        reader.Close();
        cmd.Dispose();
        connection.Close();
    }

    // focntion d'ajout de la partie 
    public void AddPartie(int Moderateur, string Statut, int NbMaxJ, string Prive, int Timer, int TMaxJ, int Meeples)
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();

        string sqlQuery = "INSERT INTO Partie (Moderateur,Statut,NbMaxJ,Prive,Timer,TMaxJ,Meeples) VALUES('" + Moderateur + "','" + Statut + "','" + NbMaxJ + "','" + Prive + "','" + Timer + "','" + TMaxJ + "','" + Meeples + "')";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();


        reader.Close();
        cmd.Dispose();
        connection.Close();
    }

    // focntion d'ajout de la fonction addtext
    public void AddExt(int IDE)
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();

        string sqlQuery = "INSERT INTO Extension (IDE) VALUES('" + IDE + "')";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();


        reader.Close();
        cmd.Dispose();
        connection.Close();
    }

    public void ReadDatabase()
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();

        string sqlQuery = "SELECT IDE,Nom FROM Extension";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.Write("IDE = " + reader.GetInt32(0));
            Console.Write("Nom = " + reader.GetString(1));
        }

        reader.Close();
        cmd.Dispose();
        connection.Close();
    }

    void RemplirTuiles(Dictionary<int, int> dico)
    {

        SqliteConnection connection = this.Connect();
        connection.Open();

        var cmd = connection.CreateCommand();

        string sqlQuery = "SELECT T.IDT,M.Proba FROM Tuile T,Modele M where T.IDM = M.IDM";
        cmd.CommandText = sqlQuery;
        IDataReader reader = cmd.ExecuteReader();
         
        while(reader.Read())
        {
            dico.Add(reader.GetInt32(0), reader.GetInt32(1));  
        }

        reader.Close();
        cmd.Dispose();
        connection.Close();
    }
}