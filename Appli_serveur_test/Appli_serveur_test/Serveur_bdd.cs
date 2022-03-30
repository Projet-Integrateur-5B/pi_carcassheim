using System;

public class Serveur_BDD
{
	public Serveur_BDD()
	{
	}

	static void Main(string[] args)
	{
		DB bd = new DB();
		Server.Server.StartListening();
	}
}
