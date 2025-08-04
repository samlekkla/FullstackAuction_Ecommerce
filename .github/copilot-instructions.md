<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# Auction Commerce API - Copilot Instructions

Detta är ett ASP.NET Core Web API-projekt (.NET 8) för en auktionsapplikation.

## Projektöversikt
- **Framework**: ASP.NET Core Web API (.NET 8)
- **Databas**: Entity Framework Core med SQL Server
- **Autentisering**: Microsoft Identity med JWT-förberedelser
- **API-dokumentation**: Swagger/OpenAPI

## Modeller
- **User**: Utökar IdentityUser med DisplayName, IsEmailVerified, IsPhoneVerified
- **Auction**: Auktioner med titel, beskrivning, startpris, start-/slutdatum
- **Bid**: Bud på auktioner med belopp och tidsstämpel

## Databaskontext
- **ApplicationDbContext**: Ärver från IdentityDbContext<User>
- Konfigurerade relationer mellan User, Auction och Bid
- Indexes för prestanda

## Riktlinjer för kodgenerering
1. Använd async/await för databasoperationer
2. Inkludera proper error handling och validering
3. Följ RESTful API-konventioner
4. Använd DTOs för API-responses när lämpligt
5. Implementera proper logging
6. Följ SOLID-principerna
7. Använd Entity Framework Best Practices
8. Säkerställ att JWT-authentication kan läggas till enkelt

## Säkerhet
- Använd [Authorize] attribut för skyddade endpoints
- Validera input data
- Implementera rate limiting för kritiska endpoints
- Förbered för JWT-implementering

## Databas
- Använd migrations för schema-ändringar
- Seed data finns för admin-användare
- ConnectionString konfigurerad för SQL Server LocalDB
