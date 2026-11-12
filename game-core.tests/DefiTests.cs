namespace DuskTillDawn.Core.Tests;

public class DefiTests
{
    private sealed class DefiDeTest() : Defi("Alerte de test", Salle.Serveurs, points: 100, dureeSecondes: 60);
    [Fact]
    public void Un_defi_neuf_est_en_attente_avec_tout_son_temps()
    {
        var defi = new DefiDeTest();

        Assert.Equal(EtatDefi.EnAttente, defi.Etat);
        Assert.Equal(60, defi.TempsRestant);
    }

    [Fact]
    public void Un_defi_garde_son_alerte_sa_salle_et_ses_points()
    {
        var defi = new DefiDeTest();

        Assert.Equal("Alerte de test", defi.Alerte);
        Assert.Equal(Salle.Serveurs, defi.Salle);
        Assert.Equal(100, defi.Points);
        Assert.Equal(60, defi.DureeSecondes);
    }

    [Fact]
    public void Lancer_un_defi_le_met_en_cours()
    {
        var defi = new DefiDeTest();

        defi.Lancer();

        Assert.Equal(EtatDefi.EnCours, defi.Etat);
    }

    [Fact]
    public void Lancer_deux_fois_ne_change_rien()
    {
        var defi = new DefiDeTest();
        defi.Lancer();

        defi.Lancer();

        Assert.Equal(EtatDefi.EnCours, defi.Etat);
    }
    
}
