public class MainBD
{
    public static void Main(String[] args)
    {

        Base_Donnees bd = new Base_Donnees();
        bd.command("select * from Utilisateur;");
        bd.Adduser("jean", "12345678912", "mail@mail.fr", "", 0, 1, 0, 0, 0, "2000-07-11");
    }

}

