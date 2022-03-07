using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.IO;
using System;

using UnityEngine;
using Mono.Data.Sqlite;

public class DB : MonoBehaviour
{
    private string dbName = "URI=file:projet.db";
    // Start is called before the first frame update
    void Start()
    {
        IncrementeNbParties(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Incrémente le nombre de parties jouées par l'utilisateur IDU.
    /// Appelé à la fin de chaque partie
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    void IncrementeNbParties(int IDU)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Utilisateur SET NbParties = NbParties + 1 WHERE IDU = " + IDU + ";";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
    /// <summary>
    /// Incrémente le nombre de parties perdues par l'utilisateur IDU.
    /// Appelé potentiellement à la fin de chaque partie
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    void IncrementePertes(int IDU)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Utilisateur SET Defaites = Defaites + 1 WHERE IDU = " + IDU + ";";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Incrémente le nombre de parties gagnées par l'utilisateur IDU.
    /// Appelé potentiellement à la fin de chaque partie
    /// </summary>
    /// <param name="IDU">Identifiant de l'utilisateur</param>
    void IncrementeVictoires(int IDU)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Utilisateur SET Victoires = Victoires + 1 WHERE IDU = " + IDU + ";";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}