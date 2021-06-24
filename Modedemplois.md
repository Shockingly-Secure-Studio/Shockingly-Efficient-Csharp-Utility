---
title : "Mode D'emploi"
---

# 1. Shokingly Efficient C# Utility (SECU)

## 1.1. Table of contents
- [1. Shokingly Efficient C# Utility (SECU)](#1-shokingly-efficient-c-utility-secu)
  - [1.1. Table of contents](#11-table-of-contents)
  - [1.2. Setup](#12-setup)
    - [1.2.1. Téléchargement](#121-téléchargement)
    - [1.2.2. Installation](#122-installation)
    - [1.2.3. Autorisations](#123-autorisations)
  - [1.3. L'Application](#13-lapplication)
    - [1.3.1. Légende Du Menu D'Accueil](#131-légende-du-menu-daccueil)
    - [1.3.2. Des Menus & Des Scans](#132-des-menus--des-scans)
      - [1.3.2.1. Démarrer Un Nouveau Scan](#1321-démarrer-un-nouveau-scan)
      - [1.3.2.2. Sauvegarder Les Scans](#1322-sauvegarder-les-scans)
      - [1.3.2.3. Charger Un Scan](#1323-charger-un-scan)
    - [1.3.3. Les Résultats Du Scan](#133-les-résultats-du-scan)
      - [1.3.3.1. Les Affichages Spécifiques](#1331-les-affichages-spécifiques)
      - [1.3.3.2. L'Analyse Des Connections](#1332-lanalyse-des-connections)
  - [1.4. Le Rapport Automatisé](#14-le-rapport-automatisé)
  - [1.5. La Désinstallation](#15-la-désinstallation)

## 1.2. Setup


### 1.2.1. Téléchargement


Rendez-vous sur notre [site](https://secu.studio) dans l'onglet [téléchargements](https://secu.studio/telechargement.html) puis cliquez ici : 

<img src="https://i.imgur.com/jjGtd1C.png">


### 1.2.2. Installation

Une fois, le téléchargement fait, vous exécutez l'installateur.

Si vous avez un avertissement comme celui-ci :

<img src = "https://cdn.discordapp.com/attachments/766976043399381012/857575550938513408/unknown.png">

Cliquez sur les informations complémentaires :

<img src ="https://cdn.discordapp.com/attachments/766976043399381012/857575629037109248/unknown.png">

Puis appuyer sur exécuter quand même.

Choisissez "Installer pour tous les utilisateurs (recommandé)".

<img src="https://i.imgur.com/4FjI3dW.png">

Une fenêtre va s'ouvrir vous demandant de donner des droits au programme, vous acceptez.

Puis choisissez la langue :

<img src="https://i.imgur.com/oNCz0XW.png">

Lisez puis acceptez le contrat de licence :

<img src="https://i.imgur.com/20ZXxMF.png">

Choisissez un endroit pour installer le programme :

<img src="https://i.imgur.com/1iDqvvy.png">

Sélectionnez les options : installer python et installer ruby qui sont sélectionner de base. De plus, si vous ne les installez pas notre logiciel ne fonctionnera pas :

<img src="https://i.imgur.com/aAqteci.png">

Enfin, sélectionnez installer : 

<img src="https://i.imgur.com/438sZOk.png">

Attendez que l'installation se termine, et ensuite cliquez sur terminer.

<img src = "https://i.imgur.com/ykG3cuF.png">

### 1.2.3. Autorisations


Lancez l'application, autorisez l'accès au pare-feu à l'application, c'est nécessaire pour le bon fonctionnement des scans.

<img src="https://i.imgur.com/ebbpjgx.png">

## 1.3. L'Application

Une fois l'application lancé vous allez être sur la page d'acceuil :

<img src="https://i.imgur.com/cuTdgUJ.png">

### 1.3.1. Légende Du Menu D'Accueil

  - Le bouton en haut à gauche (1) vous permet d'avoir des informations sur l'application et de désinstaller le logiciel mais on y reviendra plus tard.
  - Le bouton en haut à droite (2) vous permet de quitter     l'application.
  - Le bouton au milieu vous permet de lancer un nouveau scan.

### 1.3.2. Des Menus & Des Scans

<img src= "https://i.imgur.com/HrtbbOg.png">

#### 1.3.2.1. Démarrer Un Nouveau Scan

Cliquez sur nouvelle session puis rentrez l'adresse IP que vous souhaitez scanner, si vous souhaitez analyser votre réseau local laissé le champ vide.

Ensuite, il vous faut sélectionner les options en fonction de ce que vous voulez scanner. (site web, ...)

Désactivez ou non le scan agressif. (il est à noter que le scan agressif détectera plus de choses, mais sera en revanche plus long.)

Et séléctionnez le réseau à analyser.

Ensuite appuyer sur Start scan

<img src = "https://i.imgur.com/hLfNULI.png">

#### 1.3.2.2. Sauvegarder Les Scans

Pour sauvegarder un scan, il faut tout d'abord, lancer ce dernier, cliquer sur retour au menu.

Comme ceci :

<img src = "https://i.imgur.com/gNEV5qq.png">

Puis re cliquez sur start scan et enfin cliquez sur charger une session.

<img src = "https://i.imgur.com/X11y3Iv.png">

Enfin, donnez un nom à sauvegarde puis cliquez sur Sauvegarder la session.

<img src = "https://i.imgur.com/WkmdZHf.png">

#### 1.3.2.3. Charger Un Scan

Pour charger une session, il faut en avoir enregistré une au préalable et ensuite, rentrez le nom du scan que vous avez sauvegardé puis cliquez sur charger une session.

<img src ="https://i.imgur.com/Tiyj6NE.png">

Et vous allez retourner sur la page des résultats du scan.

### 1.3.3. Les Résultats Du Scan

La page des résultats du scan s'affiche de cette façon :

<img src = "https://i.imgur.com/W1vBXHB.png">

#### 1.3.3.1. Les Affichages Spécifiques

On peut y voir le type de faille, l'ip et le port sur lequel la faille a été trouvée et aussi le point d'accès.

Ensuite, si vous cliquez sur la faille SQLi, on peut voir les informations que notre logiciel à réussis à obtenir.

<img src = "https://i.imgur.com/u9n6auU.png">

Et si on clique sur le WordPress, on va obtenir le résultat de l'analyse du WordPress :

<img src = "https://i.imgur.com/V52GqpE.png">

#### 1.3.3.2. L'Analyse Des Connections

Si vous cliquez sur le bouton liste des appareils, vous arriverez à cette fenêtre.

<img src = "https://cdn.discordapp.com/attachments/796395640405360662/857393081305137152/unknown.png">

Ce que vous voyez sur cette fenêtre, c'est les connections entrante et sortante de votre ordinateur ainsi que leur niveau de criticité, de plus notre logiciel va rechercher les clefs RSA et vérifier si ces dernières sont vulnérables.

## 1.4. Le Rapport Automatisé 

Pour obtenir le rapport automatisé, il vous faut faire et attendre que le scan soit fini. Ensuite, cliquez sur accéder à la liste des appareils :

<img src = "https://i.imgur.com/1oHPztR.png">

Une fois ici cliquez sur rapport :

<img src = "https://i.imgur.com/aF2TMp6.png">

Puis sur télécharger le rapport :

<img src = "https://i.imgur.com/sUvPKD9.png">

Enfin, indiquez le chemin où vous voulez que le rapport soit mis et cliquez sur sauvegarder :

<img src = "https://i.imgur.com/s16ADjO.png">

Ensuite, ouvrez le rapport et tout est détaillé dedans avec un tableau récapitulatif des failles trouvé ainsi qu'une description de ces dernières et des ressources pour vous en protéger.

<img src = "https://i.imgur.com/dx04unR.png">

## 1.5. La Désinstallation

Pour désinstaller l'application, il faut aller sur le menu principal puis clique sur le bouton information enfin cliquez sur désinstaller S.E.C.U. .

<img src = "https://i.imgur.com/YFv9LvU.png">

Bravo, vous venez de lire le mode d'emploi voici votre flag :

HDFR:{1_R3ad_7h3_M4nual_7f06d825a34bdcbd64fa85230fa6d0d4}