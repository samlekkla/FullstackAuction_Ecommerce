# Auction Commerce API

Ett ASP.NET Core Web API-projekt (.NET 8) för auktionsapplikation med användarhantering och budgivning.

## Funktioner

- **Användarhantering**: Microsoft Identity med utökad User-modell
- **Auktioner**: Skapa och hantera auktioner med start-/slutdatum
- **Budgivning**: Lägg bud på aktiva auktioner
- **API-dokumentation**: Swagger UI för API-testning
- **Databas**: Entity Framework Core med SQL Server

## Modeller

### User (utökar IdentityUser)
- DisplayName
- IsEmailVerified
- IsPhoneVerified
- CreatedAt

### Auction
- Id, Title, Description
- StartingPrice, StartDate, EndDate
- CreatedByUserId (relation till User)

### Bid
- Id, Amount, CreatedAt
- AuctionId, UserId (relationer)

## Installation och konfiguration

### Förutsättningar
- .NET 8 SDK
- SQL Server eller SQL Server LocalDB
- Visual Studio Code eller Visual Studio

### Steg för att komma igång

1. **Klona eller ladda ner projektet**

2. **Återställ NuGet-paket**
   ```bash
   dotnet restore
   ```

3. **Konfigurera databas**
   
   Uppdatera connection string i `appsettings.json` om behövs:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionCommerceDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Skapa och tillämpa migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Kör applikationen**
   ```bash
   dotnet run
   ```

6. **Öppna Swagger UI**
   
   Navigera till `https://localhost:5001` eller `http://localhost:5000` i din webbläsare.

## API Endpoints

### Auctions
- `GET /api/auctions` - Hämta alla auktioner
- `GET /api/auctions/{id}` - Hämta specifik auktion
- `GET /api/auctions/active` - Hämta aktiva auktioner

### Bids
- `GET /api/bids/auction/{auctionId}` - Hämta bud för en auktion
- `GET /api/bids/{id}` - Hämta specifikt bud

### Users
- `GET /api/users` - Hämta alla användare
- `GET /api/users/{id}` - Hämta specifik användare

## Standard admin-användare

När applikationen startar första gången skapas ett admin-konto:
- **Email**: admin@auctioncommerce.com
- **Lösenord**: Admin123!
- **Roll**: Admin

## Kommande funktioner

- JWT Authentication implementation
- Auction CRUD operations
- Bid placement endpoints
- Real-time bidding med SignalR
- File upload för auktionsbilder
- Email notifications

## Teknik stack

- **Backend**: ASP.NET Core 8.0
- **Databas**: Entity Framework Core + SQL Server
- **Authentication**: Microsoft Identity
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Built-in ASP.NET Core logging

## Utveckling

Projektet är strukturerat för enkel vidareutveckling:

- `Models/` - Datamodeller
- `Data/` - DbContext och seed data
- `Controllers/` - API controllers
- `appsettings.json` - Konfiguration

För att lägga till nya funktioner, följ established patterns och använd async/await för databasoperationer.
