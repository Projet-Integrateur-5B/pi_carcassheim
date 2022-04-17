create table Extension
(
    IDE INTEGER not null
        primary key autoincrement
        unique,
    Nom TEXT    not null,
    check (length(Nom) <= 12)
);

create table Modele
(
    IDM   INTEGER not null
        primary key autoincrement
        unique,
    Proba INTEGER not null,
    Nom   INTEGER not null,
    check ("Proba" > 0)
);

create table Tuile
(
    IDT   INTEGER not null
        primary key autoincrement
        unique,
    IDM   INTEGER not null
        references Modele,
    Image BLOB    not null
);

create table Utilisateur
(
    IDU       INTEGER not null
        primary key autoincrement
        unique,
    Pseudo    TEXT    not null,
    MDP       TEXT    not null,
    Mail      TEXT    not null,
    Photo     BLOB    default NULL,
    XP        INTEGER default 0 not null,
    Niveau    INTEGER default 1,
    Victoires INTEGER default 0,
    Defaites  INTEGER default 0,
    NbParties INTEGER default 0,
    DateNaiss TEXT    default NULL,
    check ("DateNaiss" IS strftime('%Y-%m-%d', "DateNaiss")),
    check ("Defaites" <= "NbParties"),
    check ("NbParties" >= 0),
    check ("Niveau" >= 1),
    check ("Victoires" <= "NbParties"),
    check ("XP" < 100 AND "XP" >= 0),
    check (length("Pseudo") <= 32)
);

create table Partie
(
    IDP        INTEGER not null
        primary key autoincrement
        unique,
    Moderateur INTEGER not null
        references Utilisateur,
    Statut     INTEGER not null,
    NbMaxJ     INTEGER not null,
    Prive      NUMERIC not null,
    Timer      INTEGER not null,
    TMaxJ      INTEGER not null,
    Meeples    INTEGER not null,
    check ("Meeples" > 0),
    check ("NbMaxJ" >= 2 AND "NbMaxJ" <= 8),
    check ("Prive" IN (0, 1)),
    check ("Statut" IN (0, 1)),
    check ("TMaxJ" > 0),
    check ("Timer" > 0)
);

create table PartieExt
(
    IDP INTEGER
        references Partie,
    IDE INTEGER
        references Extension,
    primary key (IDP, IDE)
);


