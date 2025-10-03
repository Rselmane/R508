 # R5.08 - Qualité de développement - Projet d'API web .net 

#### Ce dépositoire présent un projet d'api web collaboratif, inspiré des exemples du cours R508 et intégrant de nombreuses bonnes pratiques de code.

## Principes SOLID respectés :

**Single Responsibility Principle (SRP) :** Les classes assument une unique responsabilité et n’ont qu’un seul motif de modification. Cela permet d’éviter de regrouper plusieurs rôles dans une même classe, ce qui simplifie la lisibilité et la maintenance du code.

**Open/Closed Principle (OCP) :** Les classes sont extensibles sans nécessiter de modifications directes. Cela permet d’ajouter de nouvelles fonctionnalités sans toucher au code existant et limite ainsi les risques d’erreurs.

**Liskov Substitution Principle (LSP) :** Les classes abstraite peuvent remplacer leur classe supérieure sans altérer le fonctionnement du programme. Ainsi, les instances dérivées peuvent être utilisées à la place de la classe de base sans comportement inattendu.

**Interface Segregation Principle (ISP) :** Les interfaces ne contiennent que les méthodes pertinentes pour les classes qui l’implémentent. Cela évite d’imposer à une classe des méthodes inutiles et rend ainsi le code plus clair et plus flexible.

**Dependency Inversion Principle (DIP) :** Les modules s’appuient sur des abstractions plutôt que sur des implémentations concrètes. Cela permet de réduire le couplage et améliore ainsi la testabilité et la maintenabilité du système.

## Fonctionnalités ajoutées et changement effectués :
- Ajout de `DataAnnotations` et de seeding pour le code-first de DbContext
- Expansion du Blazor pour intégrer les marques et les types de produits
 - Utilisation d'un `Automapper` et de `DTO`s lors d'appels des contrôleurs
 - Utilisation d'un dépositoire générique `Crud` pour simplifier l'implémentation de méthode récurrentes
 - Ségrégation et factorisation des interfaces avec `IProductRepository`
 - Implémentation de `IActionResult` dans les retours des contrôleurs
 - Utilisation de classes partielles pour les modèles `EntityFramework`
 - Factorisation des tests avec `TestInitialize` et `TestCleanup`
 - Ajout de nombreux tests (83) :
 - - 28 Tests Unitaires
 - - 26 Tests Mock
 - - 18 Tests de Mapping
 - - 11 Tests End-To-End
 
 ## Pistes d'amélioration :
 - Implémenter le pattern `MVVM` pour l'application Blazor
 - Créer des classes statiques et des méthodes de travail pour simplifier/factoriser les tests
 - Réduire l'utilisation de `var` en faveur de types définis

## Packages NuGet utilisés :
Avant de démarrer le projet, assurez vous que `dotnet tools` soit exécutable dans votre $PATH, et que les packages ci-dessous sont bien présents :
- App
- - AutoMapper `15.0.1`
- - Community.Toolkit.Mvvm `8.4.0`
- - Microsoft.EntityFrameworkCore.Tools `8.0.11`
- - Microsoft.Extensions.DependencyInjection `9.0.9`
- - Npgsql.EntityFrameworkCore.PostgreSQL `8.0.11`
- - Npgsql.EntityFrameworkCore.PostgreSQL.Design `1.1.0`
- - SwashBuckle.AspNetCore `6.6.2`
- BlazorApp
- - Community.Toolkit.Mvvm `8.4.0`
- Tests
- - Community.Toolkit.Mvvm `8.4.0`
- - JetBrains.Annotations `2025.2.2`
- - Kralikez.AutoFixture.Extensions.AspNetCore.WebApplicationFactory `2.1.0`
- - Microsoft.EntityFrameworkCore.InMemory `8.0.20`
- - Microsoft.NET.Test.Sdk `17.14.1`
- - Microsoft.Playwright `1.55.0`
- - Moq `4.20.72`
- - MSTest.TestAdapter `3.10.4`
- - MSTest.TestFramework `3.10.4`

Le projet vise un environnement Visual Studio 2022 sur Windows 10

## Installation :
Une fois la solution ouverte, faites un clic droit sur `Solution 'R508'`, sélectionnez `Propriétés`, `Configurer les projets de démarrage`, `Plusieurs projets de démarrage`, et désignez `App` et `BlazzorApp` en `Commencer` et `https` si ce n'est pas déjà le cas.

Vous pouvez immédiatement lancer le projet. Une base de données en ligne est utilisée par défaut, sous le connexionstring `Server=192.162.71.131;port=5432;Database=qualidev;uid=qualidev;password=qualidev`
Si vous le souhaitez, vous pouvez sinon utiliser une base de donnée locale : 
 - Dans `App/Program.cs` ligne 26, remplacez `SeriesDbContextRemote` par `SeriesDbContext`
 - Le connexionstring devient alors `Server=localhost;port=5432;Database=R508;uid=postgres;password=postgres;`, assurez vous que cette base existe et qu'elle appartient au bon utilisateur
 - Ouvrez la console du gestionnaire de package et effectuez la migration avec `dotnet ef database update --project App`

#### Tests end-to-end
Pour exécuter les tests end-to-end présents, une étape de configuration supplémentaire est nécessaire :
- Si ce n'est pas déjà fait, lancer le projet afin de créer `/bin/` et `/obj/`
- Avec powershell, déplacez vous vers `<Project>/Tests/bin/Debug/net8.0` et lancez la commande `./playwright.ps1 install` afin d'installer les navigateurs virtuels requis
- Lors de l’exécution des tests E2E, assurez vous que le projet n'est pas lancé dans une autre fenêtre ou un autre processus (port `7008` et `7777`). Si c'est le cas, le test tentera de les arrêter automatiquement, mais cette procédure peut échouer selon votre système d'exploitation et les permissions du programme.
