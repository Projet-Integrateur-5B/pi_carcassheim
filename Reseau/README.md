# Projet Intégrateur
## Partie "Reseau"

### Equipe

> **Développeur** : Victor HAHNSCHUTZ <br>
> **Développeur** : Matthieu FREITAG
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
- le style
- les tests unitaires

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




