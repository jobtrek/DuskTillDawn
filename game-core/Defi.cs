namespace DuskTillDawn.Core;

public enum EtatDefi
{
    EnAttente,
    EnCours,
    Reussi,
    Rate,
}

public enum Salle
{
    Toit,
    Controle,
    Serveurs,
}

public abstract class Defi
{
    private const double MargeArrondi = 1e-9;

    protected Defi(string alerte, Salle salle, int points, double dureeSecondes)
    {
        Alerte = alerte;
        Salle = salle;
        Points = points;
        DureeSecondes = dureeSecondes;
        TempsRestant = dureeSecondes;
    }

    public string Alerte { get; }
    public Salle Salle { get; }
    public int Points { get; }
    public double DureeSecondes { get; }
    public double TempsRestant { get; private set; }
    public EtatDefi Etat { get; private set; } = EtatDefi.EnAttente;

    public void Lancer()
    {
        if (Etat == EtatDefi.EnAttente)
        {
            Etat = EtatDefi.EnCours;
        }
    }

    public void Tick(double secondes)
    {
        if (Etat != EtatDefi.EnCours)
        {
            return;
        }

        TempsRestant -= secondes;

        if (TempsRestant <= MargeArrondi)
        {
            TempsRestant = 0;
            Etat = EtatDefi.Rate;
        }
    }

    public void Reparer(Joueur joueur)
    {
        if (Etat != EtatDefi.EnCours)
        {
            return;
        }
        Etat = EtatDefi.Reussi;
        joueur.GagnerPoints(Points);
    }
}
