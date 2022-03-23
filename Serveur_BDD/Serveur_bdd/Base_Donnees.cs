using System;
using Mono.Data.Sqlite;

public class Base_Donnees
{
	private string dbName = @"URI=file:D:\Projet\pi_carcassonne\Serveur_BDD\Serveur_bdd\projet.db";
    public Base_Donnees()
	{

	}

	public SqliteConnection Connect()
    {
        try
        {
			SqliteConnection connection = new SqliteConnection(dbName);
			return connection;
		}
		catch(Exception ex)
        {
			Console.WriteLine("ERREUR: echec de l'ouverture : "+ex);
			
			return null;
		}
	}

    public string command(string s)
    {
        SqliteConnection connection  = this.Connect();
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
                Console.Write("Erreur : commande : "+s +" " +ex);
                return "";
            }
        }
        connection.Close();
    }

    public void Adduser(string Pseudo, string MDP, string Mail, string Photo, int XP, int Niveau, int Victoires, int Defaites, int Nbparties, string DateNaiss)
    {

        int age = GetAge(DateNaiss);

        if (age >= 13)
        {
            SqliteConnection connection = this.Connect();
            connection.Open();

            var cmd = connection.CreateCommand();
            string sqlQuery = "INSERT INTO Utilisateur (Pseudo,MDP,Mail,Photo,XP,Niveau,Victoires,Defaites,Nbparties,DateNaiss) VALUES('" + Pseudo + "','" + MDP + "','" + Mail + "','" + Photo + "','" + XP + "','" + Niveau + "','" + Victoires + "','" + Defaites + "','" + Nbparties + "','"+ DateNaiss+"');";
            cmd.CommandText = sqlQuery;
            var reader = cmd.ExecuteReader();
            reader.Close();

            connection.Close();
        }
        else
        {
            Console.Write("Erreur : Age : "+age);
        }
        Getuser();
    }

    public void Getuser()
    {
        string s = "select * from Utilisateur;";
        Console.Write("Resultat : "+s +" : " + command(s));
    }

    int GetAge(string DateNaiss)
    {
        DateTime toDate = DateTime.Parse(DateNaiss);
        int age = DateTime.Now.Subtract(toDate).Days;
        age = age / 365;
        return age;
    }
}
