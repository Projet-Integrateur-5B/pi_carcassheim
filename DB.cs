using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DB : MonoBehaviour
{
    private string dbName = "URI=file:projet.db";

    // Start is called before the first frame update
    void Start()
    {}
    // Update is called once per frame
    void Update()
    {}

    /// <summary>
    /// Fonction qui incrémente le nombre de parties jouées par un utilisateur
    /// appelée systematiquement à la fin de chaque partie pour tous les
    /// joueurs
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    public void IncrementeNbParties(int IDU)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open(); 
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "update Utilisateur set NbParties =  NbParties + 1 where IDU = " + IDU +";";
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Fonction qui incrémente le nombre de parties gagnées par un utilisateur le cas échaeant
    /// </summary>
    /// <param name="IDU">Identifiant du vainqueur</param>
    public void IncrementeVictoires(int IDU)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "update Utilisateur set victoires =  victoires + 1 where IDU = " + IDU + ";";
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Fonction qui incrémente le nombre de parties perdues par un utilisateur le cas échaeant
    /// </summary>
    /// <param name="IDU">Identifiant du vaincu</param>
    public void IncrementeDefaites(int IDU)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "update Utilisateur set defaites =  defaites + 1 where IDU = " + IDU + ";";
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Fonction renvoyant l'XP de'un utilisateur
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
    /// Fonction qui incrémente l'XP et le cas échéant le niveau d'un utilisateur qui arrive à XP
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    /// <param name="XP">Le nombre de points d'expérience à rajouter à l'utilisateur</param>
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
}