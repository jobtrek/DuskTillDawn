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
    [Fact]
    public void Le_temps_ne_baisse_pas_tant_que_le_defi_est_en_attente()
    {
        var defi = new DefiDeTest();

        defi.Tick(10);

        Assert.Equal(60, defi.TempsRestant);
        Assert.Equal(EtatDefi.EnAttente, defi.Etat);
    }

    [Fact]
    public void Le_temps_baisse_quand_le_defi_est_en_cours()
    {
        var defi = new DefiDeTest();
        defi.Lancer();

        defi.Tick(10);

        Assert.Equal(50, defi.TempsRestant);
        Assert.Equal(EtatDefi.EnCours, defi.Etat);
    }

    [Theory]
    [InlineData(60)]
    [InlineData(90)]
    public void Le_defi_est_rate_quand_le_temps_arrive_a_zero(double secondes)
    {
        var defi = new DefiDeTest();
        defi.Lancer();

        defi.Tick(secondes);

        Assert.Equal(0, defi.TempsRestant);
        Assert.Equal(EtatDefi.Rate, defi.Etat);
    }

    [Fact]
    public void Un_defi_rate_ne_peut_pas_etre_relance()
    {
        var defi = new DefiDeTest();
        defi.Lancer();
        defi.Tick(60);
        defi.Lancer();
        Assert.Equal(EtatDefi.Rate, defi.Etat);
    }

    [Fact]
    public void Reparer_un_defi_en_cours_le_reussit()
    {
        var defi = new DefiDeTest();
        defi.Lancer();
        defi.Tick(20);
        
        defi.Reparer();

        Assert.Equal(EtatDefi.Reussi, defi.Etat);
        Assert.Equal(40, defi.TempsRestant);
    }

    [Fact]
    public void Reparer_un_defi_en_attente_ne_fait_rien()
    {
        var defi = new DefiDeTest();

        defi.Reparer();

        Assert.Equal(EtatDefi.EnAttente, defi.Etat);
    }

    [Fact]
    public void Reparer_un_defi_rate_ne_fait_rien()
    {
        var defi = new DefiDeTest();
        defi.Lancer();
        defi.Tick(60);

        defi.Reparer();

        Assert.Equal(EtatDefi.Rate, defi.Etat);
    }

    [Fact]
    public void Un_defi_reussi_ne_bouge_plus()
    {
        var defi = new DefiDeTest();
        defi.Lancer();
        defi.Reparer();
        defi.Tick(60);
        Assert.Equal(EtatDefi.Reussi, defi.Etat);
        Assert.Equal(60, defi.TempsRestant);
    }
}
