# Rakenduse ametlik dokumentatsioon

### Autor: Andrei Grigorjev

#### Funktsionaalsus:
1. saab lisada uusi üritusi (nimetus, millal - tulevikus, kus, lisainfo - max 1000);
2. saab muuta ürituste andmeid;
3. saab vaadata ürituste andmeid;
4. saab üritusi (mis toimuvad tulevikus) kustutada (ürituse osalejaid kustutakse ka);
5. saab üritustele luua osalejaid (füüsiline - "lisainfo" välja pikkus on 1500 märki või juriidiline - "lisainfo" välja pikkus on 5000 märki);
6. saab vaadata/muuta üritustele registreeritud osalejate andmeid;
7. saab lisada üritusele ka baasis olemasolevaid osalejaid (juhul kui selline osaleja ei osale veel selles ürituses);
8. saab täielikult kustutada osalejaid (sel juhul pole võimalik neid lisada teistele üritustele).


Arenduskeskkonnaks oli valitud JetBrains-i oma Rider.

Rakenduse valmimiseks olid kasutatud sellised tööriistad nagu C# (dotnet6.0), Razor Pages ja andmebaasi mootoriks SQLite.
Selleks, et andmebaas saaks suhelda rakendusega, on kasutusel ORM EF6 (Entity Framework 6).

Andmebaasi ERD skeem sisaldab 3 olemit ning 1 klass on kasutusel ENUM-na, seega selle jaoks puudub eraldi olem baasis.
**ERD skeemi loomiseks oli kasutatud rakendus "QSEE SuperLite" (seal puudub võimalus kasutada andmetüübiks GUID, seega skeemil on ID väli märgitud VARCHAR-na).**
Kuna kõik olemid kasutavad selliseid veerge nagu "ID", "CreatedAt", "UpdatedAt", siis olid tehtud eraldi interface-d ja baasolemid.
Rakenduse põhiklassid siis implementeerivad funktsionaalsust (koodi paindlikuse eesmärgil).
Baasolemid asuvad projektides "Base.Contracts", "Base.Domain" (abstraktsed klassid). Rakenduse põhiolemid projektis "App.Domain".

Selleks, et EF saaks suhelda andmebaasiga, oli loodud eraldi projekt DAL, kus on seadistatud andmebaasi ühendus + mõned reeglid baasi loomiseks.
Samas projektis on kaust andmebaasi migratsiooniga, mida kasutatakse andmebaasi automaatseks loomiseks rakenduse käivitades.

Rakenduse põhiprojektiks on "WebApp", kust rakendust käivitatakse.


### Selleks, et rakendust käivitada:
kuna kasutusel on mälupõhine andmebaas, siis rakenduse käivitamiseks tuleb vajutada "Run Code" (SHIFT+F10 Rideri puhul) peale, siis rakenduse kood saab kompileeritud ja käivitatud (kui kompileerimine õnnestus).
**Testimiseks - failis "program.cs" saab märkida, et igal käivitamisel andmebaas kustutakse ära käsuga "EnsureDeleted();".**

### Autommattestide käivitamiseks:
projektis "AutomatedTests" on automaattestid rakenduse kvaliteedi kontrollimiseks. 
Kasutusel on Seleniumi WebDriver Google Chrome jaoks (versioon: 99.0.4844.51). 
Teise versiooni jaoks draiverit saab alla laadida aadressilt "https://chromedriver.chromium.org/downloads" ning sisu lahti pakkida kausta "drivers", mis asub samas projektis.
Rakendus hakkab otsima draiverit just sellest kaustast!

Testide käivitamiseks **rakendus peab olema käivitatud** ka (muidu testid feilivad ühenduse puudumise tõttu).
Testid eeldavad, et rakendust käivitatakse aadressilt **"https://localhost:7031/"**. Port on vaikimisi seadistatud 7031-ks failis "WebApp/Properties/launchSettings.json".
Käivitamiseks tuleb vajutada **Run All** ikooni testide juures.