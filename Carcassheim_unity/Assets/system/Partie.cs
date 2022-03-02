using System;

public class Partie
{
    Plateau _plateau;
    public Plateau ZePlateau => _plateau;

    public Partie()
    {
        _plateau = new Plateau();
    }

    public void Run()
    {
        int[][] lien = new int[2][];
        
        int[] t1 = new int[3] {0, 1, 2};
        int[] t2 = new int[9] { 3, 4, 5, 6, 7, 8, 9, 10, 11};
        lien[0] = t1; lien[1] = t2;
        TypeTerrain[] terrains = new TypeTerrain[] { TypeTerrain.Ville, TypeTerrain.Pre };

        /*
        int[][] lien = new int[1][];
        
        int[] t2 = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};
        lien[0] = t2;
        TypeTerrain[] terrains = new TypeTerrain[] { TypeTerrain.Ville };*/
        
        _plateau.PoserTuile(new Tuile(0, 2, lien, terrains), 0, 0, 2);
    }
}