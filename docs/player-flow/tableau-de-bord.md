# Écran du tableau de bord (maquette)

Ce que le joueur voit sur l'ordinateur de la salle de contrôle. Les alertes viennent de la [liste des défis](defis.md).

## 1. Un joueur

```text
┌──────────────────────────────────────────────────────────┐
│  TABLEAU DE BORD                    Points : 350         │
│                                     Pannes : 1 / 3       │
├──────────────────────────────────────────────────────────┤
│  ALERTES                                                 │
│  🔴 Un disque ne répond plus     Salle des serveurs  0:45 │
│  🟠 Température trop haute       Salle des serveurs  1:20 │
├──────────────────────────────────────────────────────────┤
│  SERVEURS                                                │
│  Serveur 1  🟢 OK                                        │
│  Serveur 2  🔴 Problème                                  │
│  Serveur 3  🟠 Chaud                                     │
│  Serveur 4  ⚫ En panne                                   │
└──────────────────────────────────────────────────────────┘
```

| Zone | Ce qu'elle montre |
|---|---|
| En haut | Les points du joueur et le nombre de pannes avant de perdre |
| Alertes | Le problème, la salle où aller et le temps restant pour réparer |
| Serveurs | L'état de chaque serveur, par couleur |

| Couleur | Sens |
|---|---|
| 🟢 Vert | Tout va bien |
| 🟠 Orange | Attention, ça va bientôt casser |
| 🔴 Rouge | Problème à réparer maintenant |
| ⚫ Noir | Serveur en panne |

## 2. Deux joueurs

Même écran, mais le haut montre les scores des deux joueurs, et l'alerte indique que c'est une course.

```text
┌──────────────────────────────────────────────────────────┐
│  TABLEAU DE BORD          Joueur 1 : 400   Joueur 2 : 250 │
├──────────────────────────────────────────────────────────┤
│  ⚔️  DÉFI EN COURS : Attaque de sécurité            0:30 │
│      Le premier qui la règle gagne 150 points            │
└──────────────────────────────────────────────────────────┘
```

## À décider

- Le tableau de bord est-il seulement sur l'ordinateur, ou aussi toujours visible à l'écran (en haut à droite) ?
