using System;
using Assets.system;
public class Partie
{
    Plateau _plateau;
    int[] _idJoueurs;
    int _idCurrentJoueur;
    readonly int _idClient;
    bool _over;
    int _nbTour;
    public Plateau Plateau => _plateau;

    public Partie(params int[] idJoueurs)
    {
        _plateau = new Plateau();
        _idJoueurs = idJoueurs;
        _over = false;
        _nbTour = 0;
    }

    public void Run()
    {
        while (!_over)
        {
            _idCurrentJoueur = _idJoueurs[_nbTour];

            if (_nbTour % _idJoueurs.Length == _idClient)
                JouerTour();
            else
                TourAutreJoueur();

            _nbTour++;
        }
    }

    private void JouerTour()
    {
        bool placementPossible = false;
        Position[] positionPossible;
        Tuile tuile = Tuile.DicoTuiles[0];

        while (!placementPossible)
        {
            // DEMANDE TUILES

            Tuile[] tuileRecues = new Tuile[3];
            // receptionne Tuiles

            foreach (var item in tuileRecues)
            {
                positionPossible = _plateau.PositionsPlacementPossible(item);
                if (positionPossible.Length > 0)
                {
                    placementPossible = true;
                    tuile = item;
                }
            }
        }

        // ENVOYER AU FRONT POSITION POSSIBLE POUR QU'IL LES METTENT EN VALEURS

        // RECEPTIONNER LA POSITION CHOISIE
        Position placementJoueur = new Position();

        _plateau.PoserTuile(tuile, placementJoueur);

        int[] pion = _plateau.EmplacementPionPossible(tuile, _idCurrentJoueur);

        // ENVOYER pion AU FRONT

        // RECEVOIR PLACEMENT PION DU JOUEUR

        int slot = 0;

        _plateau.PoserPion(_idCurrentJoueur, tuile, slot);
    }

    void TourAutreJoueur()
    {
        bool pasPlacable = false;
        if (pasPlacable)
        {
            // RECEVOIR TUILES
            Tuile[] tuiles = new Tuile[3];

            foreach (var item in tuiles)
            {
                if (_plateau.PositionsPlacementPossible(item).Length >= 0)
                {
                    // ENVOYER TRICHEUR
                }
            }
        }

        // RECEPTIONNE COUP
        int idTuile = 0, slot = 0;
        Position pos = new Position();
        _plateau.PoserTuile(idTuile, pos);
        _plateau.PoserPion(_idCurrentJoueur, idTuile, slot);
    }
}