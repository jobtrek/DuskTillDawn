# Diagramme de classes (orienté objet)

Les classes à coder en C#. Elles viennent du [diagramme du jeu](game-states.md), de la [liste des défis](defis.md) et du [tableau de bord](tableau-de-bord.md).

Se lit de haut en bas : « Le tableau de bord lit la partie », « Une partie contient des joueurs », etc.

## 1. Les classes principales

```mermaid
%%{init: {"flowchart": {"curve": "linear"}}}%%
flowchart TD
    Tableau["<b>TableauDeBord</b><br/>Affiche les alertes et les points"]
    Partie["<b>Partie</b> (abstraite)<br/>Lance les défis<br/>Dit si la partie est finie"]
    Solo["<b>PartieSolo</b><br/>Finit après 3 pannes"]
    Duo["<b>PartieDuo</b><br/>Finit après le dernier défi<br/>Donne le gagnant"]
    Joueur["<b>Joueur</b><br/>Nom, points, salle<br/>Se déplace, fait un défi"]
    Serveur["<b>Serveur</b><br/>Numéro, état<br/>Tombe en panne, est réparé"]
    Defi["<b>Defi</b> (abstraite)<br/>Alerte, salle, points, temps<br/>Réussi ou raté"]

    Tableau -->|"lit"| Partie
    Partie -->|"peut être"| Solo
    Partie -->|"peut être"| Duo
    Partie -->|"contient 1 ou 2"| Joueur
    Partie -->|"contient 4"| Serveur
    Partie -->|"lance"| Defi
```

## 2. Les défis

```mermaid
%%{init: {"flowchart": {"curve": "linear"}}}%%
flowchart TD
    Defi["<b>Defi</b> (abstraite)"]
    Defi -->|"peut être"| A["AttaqueSecurite"]
    Defi -->|"peut être"| B["DisqueCasse"]
    Defi -->|"peut être"| C["ServeurTropChaud"]
    Defi -->|"peut être"| D["Feu"]
    Defi -->|"peut être"| E["CableDebranche"]
    Defi -->|"peut être"| F["FauxEmail"]
```

## 3. Les idées orientées objet utilisées

| Idée | Où dans le diagramme |
|---|---|
| **Héritage** | `PartieSolo` et `PartieDuo` héritent de `Partie`. Chaque défi hérite de `Defi`. |
| **Classe abstraite** | `Partie` et `Defi` : on ne les crée jamais seules, seulement leurs enfants. |
| **Polymorphisme** | « La partie est-elle finie ? » : en solo après 3 pannes, en duo après le dernier défi. Même question, réponse différente. |
| **Encapsulation** | Les points d'un joueur changent seulement par une méthode, jamais directement. |
| **Composition** | Une `Partie` contient ses joueurs et ses serveurs. Si la partie s'arrête, ils disparaissent avec elle. |
| **Séparation** | `TableauDeBord` lit la partie mais ne la modifie jamais (règle du [contrat](../contract.md)). |

## À décider ensemble

- Noms en français ou en anglais dans le code ? Le code actuel est en anglais (`GameState`).
