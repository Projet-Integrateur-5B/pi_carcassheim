# Projet Intégrateur
## Partie "Reseau"

[![pipeline status](https://git.unistra.fr/projet-integrateur-5b/pi_carcassonne/badges/reseau/pipeline.svg)](https://git.unistra.fr/projet-integrateur-5b/pi_carcassonne/-/commits/reseau) [![coverage report](https://git.unistra.fr/projet-integrateur-5b/pi_carcassonne/badges/reseau/coverage.svg)](https://git.unistra.fr/projet-integrateur-5b/pi_carcassonne/-/commits/reseau) [![Latest Release](https://git.unistra.fr/projet-integrateur-5b/pi_carcassonne/-/badges/release.svg)](https://git.unistra.fr/projet-integrateur-5b/pi_carcassonne/-/releases)

### Equipe

> **Responsable & Développeur** : Matthieu FREITAG <br>
> **Développeur** : Victor HAHNSCHUTZ
----
### Architecture

- Client : structure des clients
- Server : structure du serveur
- Assets : éléments communs aux programmes
- UnitTest : les tests unitaires

### Programmes

- Client
- Server

### Assets

- Packet : définit la forme des messages échangés entre les clients et le serveur

### Test unitaires
- Tests_Assets_Packet : vérifie le bon fonctionnement de la classe Packet
----
### Intégration continue

Lors d'un push sur la branche "reseau", on vérifie :
- le build
- les tests unitaires
- le style
- le coverage

### Le style

Installer/update la commande : 
```
dotnet tool update -g dotnet-format
```
Vérifier que le style ne comporte pas d'erreurs :
```
dotnet format --verify-no-changes --verbosity diag
```
Corriger (partiellement!!!) les erreurs :
```
dotnet format --verbosity diag
```




