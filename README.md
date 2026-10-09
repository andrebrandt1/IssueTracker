# IssueTracker

ASP.NET Core-API för den befintliga SQL Server-databasen `IssueTracker`.

Databasmodellen finns i [data/IssueTracker.sql](data/IssueTracker.sql), med
tabellens kolumner, standardvärden och constraints samt förklarande kommentarer.

## Starta API:et

Konfigurera databasanslutningen med User Secrets på din dator. Kör följande från
projektroten för Windows-autentisering mot `.\SQLEXPRESS`:

```powershell
dotnet user-secrets set "ConnectionStrings:IssueTracker" "Server=.\SQLEXPRESS;Database=IssueTracker;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;" --project IssueTracker.Api
```

Projektet har redan ett `UserSecretsId`. User Secrets lagras utanför projektet och
laddas automatiskt när API:et körs i Development. Varje utvecklare behöver spara
sin egen anslutningssträng. Byt servernamnet i kommandot om du använder en annan server.
Kontot som kör API:et behöver läsrättigheter till `IssueTracker.dbo.Tickets`.
`TrustServerCertificate=True` används för den lokala utvecklingsservern.

I Visual Studio kan du redigera värdet genom att högerklicka på `IssueTracker.Api`
och välja **Manage User Secrets**.

```powershell
dotnet run --project IssueTracker.Api --launch-profile https
```

Anropa `GET https://localhost:8000/tickets`, eller använd `IssueTracker.Api.http`.
Svaret är `200 OK` med en JSON-lista, sorterad efter `id`:

```json
[
  {
    "id": 1,
    "title": "Exempelärende",
    "description": "Beskrivning av ärendet",
    "status": "Öppet",
    "priority": "Normal"
  }
]
```

En tom tabell ger `200 OK` med `[]`.

## Skapa supportärende

Anropa `POST https://localhost:8000/api/tickets` med `Content-Type: application/json`:

```json
{
  "title": "Kan inte logga in",
  "description": "Användaren kan inte logga in i systemet.",
  "priority": "High"
}
```

Titel måste vara 5–100 tecken och beskrivning 10–1000 tecken. Båda är obligatoriska
och får inte enbart innehålla blanksteg. Prioritet är obligatorisk och måste vara
exakt `Low`, `Normal` eller `High`. Fält som `id` och `status` får inte skickas;
okända JSON-fält ger `400 Bad Request`.

Ett giltigt anrop ger `201 Created` med ärendets databasgenererade ID, titel,
beskrivning, status `Open` och angiven engelsk prioritet. Valideringsfel ger
`400 Bad Request` med en `errors`-samling som kan innehålla fel för flera fält.

Databasen behåller svenska värden: `Low` → `Låg`, `Normal` → `Normal`,
`High` → `Hög` och nya ärendens status lagras som `Öppet`. Det befintliga
`GET /tickets` returnerar fortsatt databasens svenska status- och prioritetsvärden.

## Implementation och tester

`Ticket` beskriver databasmodellen. `TicketService.GetTickets()` öppnar en SQL
Server-anslutning med `Microsoft.Data.SqlClient` och hämtar ärenden asynkront med
Dappers `QueryAsync<Ticket>()`. SQL-frågan väljer de fem kolumnerna från
`dbo.Tickets` och sorterar efter `Id`. Modellerna mappas sedan till `TicketDto`.
`TicketsController` returnerar DTO-listan via HTTP.
API:et skapar eller ändrar inte databasens schema.

Databastesterna körs uttryckligen och använder anslutningen i User Secrets (eller
`ConnectionStrings__IssueTracker`). De kontrollerar ID-generering och svenska
lagringsvärden med riktiga SQL Server-inserts. Testärendena rullas tillbaka;
SQL Servers IDENTITY-räknare kan ändå öka.

```powershell
dotnet test --solution IssueTracker.slnx --configuration Release --explicit only --filter-class IssueTracker.Api.Tests.TicketServiceDatabaseTests
```

Utanför Development behöver anslutningssträngen konfigureras, exempelvis via
miljövariabeln `ConnectionStrings__IssueTracker`. Använd serverns giltiga certifikat
i produktion i stället för `TrustServerCertificate=True`.
