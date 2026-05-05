# Primera Part - Quantitat d’aigua als embassaments de les Conques Internes de Catalunya


Per fer aquesta pràctica ens basem en les dades [Dades Obertes Gencat - Quantitat d’aigua als embassaments de les Conques Internes de Catalunya](https://analisi.transparenciacatalunya.cat/Medi-Ambient/Quantitat-d-aigua-als-embassaments-de-les-Conques-/gn9e-3qhr/about_data)

## Aplicació WEB per a usuaris

La teva aplicació ha de mostrar aquestes pantalles als usuaris:

* Dashboard
  * Total volum d'aigua
  * % Percentatge volum embassat
  * Anar al llistat d'estacions.
* Estació
  * Estat actual de l'estació.
  * Taula de dades: anys / mesos amb % Percentatge volum embassat amb codis de colors.
* Estació i dia
* Estació i mes/any: Veure l'estat d'un mes
  concret de l'estació (mitjana dels dies)

## API

Tindrà dos end points:

* Llistat d'estacions.
* Dades estació: Retornarà les tres mètriques que apareixen al dataset corresponents a la data més nova, i la data.

## Model de dades

Pensa el model de dades i fes el gràfic M/E-IR. Pensa que podem afegir informació addicional a les dades, per exemple, informació sobre l'`estació`, com ara la capacitat total del seu volum o informació de com arribar-hi, etc.

## Entitats C#

Pensa quines són les entitats C# per emmagatzamar el model de dades. Treballarem amb EF, llavors has de crear els models per tal que `dbcontext` s'encarregui del CRUD de les dades. Per aquesta part de la pràctica no farem servir migracions, usarem el [EnsureCreated](https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created):

>The EnsureCreatedAsync and EnsureDeletedAsync methods provide a lightweight alternative to Migrations for managing the database schema. These methods are useful in scenarios when the data is transient and can be dropped when the schema changes. For example during prototyping, in tests, or for local caches.

## ETL

Procés ETL de càrrega de dades. A partir del dataset s'informen les dades a la base de dades.

## Procediment emmagatzemat per registrar dades d’embassaments

<details>

<summary>Enunciat</summary>

### Context

Les Conques Internes de Catalunya publiquen diàriament dades agregades sobre l’estat dels embassaments. Per a cada embassament es registra la informació següent:

- **Dia**: data en què s’ha pres la mesura (corresponent al dia anterior).
- **Estació**: embassament on s’ha pres la mesura.
- **Nivell absolut (msnm)**: nivell de l’aigua mesurat a l’embassament en metres sobre el nivell del mar.
- **Percentatge volum embassat (%)**: percentatge del volum d’aigua respecte de la capacitat màxima de l’embassament.
- **Volum embassat (hm³)**: volum d’aigua emmagatzemat a l’embassament en hectòmetres cúbics.

Les dades es registren en una base de dades que conté, entre altres, una taula d’**estacions (embassaments)** i una taula amb les **mesures diàries**.

Per evitar errors en la introducció de dades, es vol crear un **procediment emmagatzemat** que centralitzi la inserció de noves mesures i realitzi totes les validacions necessàries abans de guardar-les.

Dissenya un **procediment emmagatzemat** que permeti inserir una nova mesura d’un embassament a la base de dades.

El procediment ha de rebre com a paràmetres:

- la data de la mesura
- la identificació de l’estació (embassament)
- el nivell absolut
- el percentatge de volum embassat
- el volum embassat

Abans d’inserir la nova fila, el procediment ha de realitzar les validacions següents:

### Validacions d’integritat

1. **Existència de l’estació**
   - S’ha de comprovar que l’estació indicada existeix a la taula d’estacions.

2. **Duplicats**
   - No es poden inserir dues mesures per al **mateix embassament i el mateix dia**.

3. **Data vàlida**
   - La data de la mesura no pot ser futura.
   - La data no pot ser anterior a un límit raonable definit pel sistema (per exemple, abans de l’inici del registre històric).

### Validacions de rang

4. **Percentatge**
   - El percentatge de volum embassat ha d’estar entre **0 i 100**.

5. **Volum**
   - El volum embassat ha de ser **positiu o zero**.

6. **Nivell absolut**
   - El nivell absolut ha de ser un valor positiu.

### Validacions de coherència amb dades anteriors

El sistema ha de comprovar l’última mesura disponible per al mateix embassament.

7. **Canvi excessiu en menys d’un mes**
   - Si l’última mesura disponible és de **menys d’un mes abans**, la variació del volum embassat no pot superar el **50%** respecte del valor anterior.

8. **Canvi excessiu en menys de tres mesos**
   - Si l’última mesura disponible és de **menys de tres mesos abans**, la variació del volum embassat no pot superar el **75%**.

9. **Control d’incoherències**
   - Si el percentatge i el volum embassat no són coherents amb la capacitat màxima de l’embassament, el sistema ha de rebutjar la inserció.

### Resultat del procediment

El procediment ha de:

- inserir la nova mesura si totes les validacions són correctes
- retornar un **missatge d’error clar** si alguna validació falla
- evitar que s’insereixin dades inconsistents a la base de dades.

</details>

## Desplegament

La base de dades ha d'estar al núvol (per exemple a AWS). L'aplicació també.

## Documentació

Cal fer  un document tècnic, explicant el funcionament intern de l'aplicatiu. Es demana mostrar tots els esquemes de la BDD i les funcionalitats programades documentades.

## Sistemes empresarials

Instal·la Oddo mitjançant docker:

Base de dades:

### 1. Iniciar PostgreSQL

Odoo necessita una base de dades :contentReference[oaicite:1]{index=1}:

```bash
docker run -d \
  -e POSTGRES_USER=odoo \
  -e POSTGRES_PASSWORD=odoo \
  -e POSTGRES_DB=postgres \
  --name db \
  postgres:15
```

Iniciar Odoo

```bash
docker run -d \
  -p 8069:8069 \
  --name odoo \
  --link db:db \
  odoo
```

Accedir a Odoo

```
http://localhost:8069
```

Un cop engegat, prepara una factura per facturar els serveis que has fet al projecte al client Cendrassos.