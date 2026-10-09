-- Databas: [IssueTracker]
-- Tabell:  [dbo].[Tickets] — ärenden
-- Dokumentation av den befintliga tabellen i SQL-format.

/*
[dbo].[Tickets]
(
    [Id]          int IDENTITY(1,1) NOT NULL, -- Unikt ärendenummer, ökar automatiskt med 1.
    [Title]       nvarchar(200)     NOT NULL, -- Ärendets rubrik, högst 200 tecken.
    [Description] nvarchar(max)     NOT NULL, -- Ärendets beskrivning.
    [Status]      nvarchar(20)      NOT NULL DEFAULT (N'Öppet'),  -- Ärendets status.
    [Priority]    nvarchar(20)      NOT NULL DEFAULT (N'Normal'), -- Ärendets prioritet.

    -- Id identifierar varje ärende och är tabellens klustrade primärnyckel.
    PRIMARY KEY CLUSTERED ([Id] ASC),

    -- Tillåtna statusvärden.
    CONSTRAINT [CK_Tickets_Status]
        CHECK ([Status] IN (N'Öppet', N'Pågående', N'Avslutat')),

    -- Tillåtna prioritetsvärden.
    CONSTRAINT [CK_Tickets_Priority]
        CHECK ([Priority] IN (N'Låg', N'Normal', N'Hög'))
)
*/

-- NOT NULL: alla kolumner måste ha ett värde.
-- DEFAULT: används när kolumnen utelämnas vid INSERT.
-- nvarchar och N'...': stödjer Unicode, inklusive å, ä och ö.
-- Tabellen har inga främmande nycklar.
-- TicketService.GetTickets() läser tabellen via Dapper och mappar till TicketDto.
-- GET /tickets returnerar DTO-listan sorterad efter Id.
-- POST /api/tickets skapar ett ärende via Dapper. Id genereras av SQL Server.
-- API-validering: Title 5–100 tecken, Description 10–1000 tecken, inga rena blanksteg.
-- POST använder Low/Normal/High, som lagras som Låg/Normal/Hög i databasen.
-- Nya ärenden lagras med Status Öppet och returneras med Status Open i POST-svaret.
