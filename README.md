# TheLibrayan

## Descrizione
TheLibrayan è un progetto per la gestione di una libreria. Include la creazione di un database SQL Server con tabelle per categorie, libri, utenti e relazioni tra libri e categorie. Fornisce un'API per la gestione delle operazioni relative ai libri e alle categorie.

## Struttura del Progetto
- `DB Backup/DBscript.sql`: Script SQL per la creazione e configurazione del database `TheLibrayan`.
- `DB Backup/TheLibrayan.bak`: File di backup del database `TheLibrayan`.
- `Controllers/`: Contiene i controller per le operazioni API.
- `Data/`: Contiene i contesti per l'accesso ai dati.
- `Models/`: Contiene i modelli di dati utilizzati nel progetto.

## Requisiti
- SQL Server 2016 o successivo
- SQL Server Management Studio (SSMS)
- .NET 6.0 o successivo
- Visual Studio o JetBrains Rider

## Istruzioni per l'Installazione
1. **Creazione e Ripristino del Database da Backup:**
   - Apri `DB Backup/DBscript.sql` in SQL Server Management Studio (SSMS).
   - In fondo allo script troverai la seguente scrittura:
     ```sql
     USE master;
     GO
     RESTORE DATABASE TheLibrayan
     FROM DISK = '[Indirizzo assoluto che porta alla Repo in questione]\TheLibrayan\DB Backup\TheLibrayan.bak'
     WITH REPLACE;
     GO
     ```
   - Modifichi l'indirizzo all'istruzione "FROM DISK" come indicato tra parentesi quadre [].
   - Esegui lo script per creare e configurare il database `TheLibrayan`.
     

2. **Configurazione dell'Applicazione:**
   - Aggiorna il file `appsettings.json` con la stringa di connessione al database e la chiave segreta.
   - Costruisci e avvia l'applicazione utilizzando Visual Studio o JetBrains Rider.

## Tabelle del Database
- **Categorie**: Contiene le categorie dei libri.
- **Libri**: Contiene i dettagli dei libri.
- **LibriCategorie**: Relazione tra libri e categorie.
- **Utenti**: Contiene i dettagli degli utenti.

## API Endpoints
### BookController
- `GET /api/book/get-all-books`: Recupera tutti i libri.
- `POST /api/book/add-book`: Aggiunge un nuovo libro.
- `DELETE /api/book/delete-book`: Elimina un libro.
- `GET /api/book/search-books`: Cerca libri in base a vari criteri.
- `PUT /api/book/update-book`: Aggiorna un libro.

### CategoryController
- `POST /api/category/add-category`: Aggiunge una nuova categoria.
- `DELETE /api/category/delete-category`: Elimina una categoria se è vuota.

## Configurazioni del Database
- Compatibilità: Livello 160
- Recovery: FULL
- Query Store: ON

## Note
- Assicurati di aggiornare il percorso assoluto nel comando `RESTORE DATABASE` con il percorso corretto del file di backup sul tuo sistema.
