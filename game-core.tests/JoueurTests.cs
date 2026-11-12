using System;

namespace DuskTillDawn.Core.Tests;

public class JoueurTests
{
    [Fact]
    public void Un_nouveau_joueur_a_zero_point()
    {
        var joueur = new Joueur("Bilal");

        Assert.Equal("Bilal", joueur.Nom);
        Assert.Equal(0, joueur.Points);
    }

    [Fact]
    public void Gagner_des_points_les_ajoute()
    {
        var joueur = new Joueur("Bilal");

        joueur.GagnerPoints(150);
        joueur.GagnerPoints(100);

        Assert.Equal(250, joueur.Points);
    }

    [Fact]
    public void On_ne_peut_pas_gagner_des_points_negatifs()
    {
        var joueur = new Joueur("Bilal");

        Assert.Throws<ArgumentOutOfRangeException>(() => joueur.GagnerPoints(-10));
        Assert.Equal(0, joueur.Points);
    }
}
