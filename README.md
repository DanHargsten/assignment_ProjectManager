# Översikt #

Detta projekt är en konsolbaserad applikation som hanterar anställda och projekt. Systemet implementerar CRUD-funktionalitet för både anställda och projekt, med ett användarvänligt konsolgränssnitt.


## Uppfyllda krav ##
- Fungerande databas som använder SQL Server med Entity Framework Core.
- Samtliga databasoperationer fungerar korrekt (CRUD).
- Repositories och Services hanterar databaskommunikationen via Dependency Injection.
- Backend-logiken är implementerad enligt kursens riktlinjer.
- Alla enhetstester har genomförts och godkänts.
- Användargränssnittet i konsolapplikationen möjliggör visning, skapande, uppdatering och radering av både anställda och projekt.

## Kända problem ##
Radering av anställda: Det var svårt att hantera separationen mellan att ta bort en anställd från ett projekt och att radera en anställd helt. Testerna har blivit godkända, så jag tror att backend fungerar korrekt.
Jag klev ner i ett kaninhål med ChatGPT för att få det att fungera, men jag kom längre och längre ifrån målet. Så *DeleteEmployeeDialog* fungerar inte som det ska i konsolapplikationen.

## Lärdomar & Utmaningar ##
Den här kursen har varit både lärorik och utmanande. Jag har fått en djupare förståelse för backend-logik och hur man bygger en robust CRUD-applikation.
Samtidigt var det svårt att veta exakt vilka krav som gällde för slutresultatet. Sent in i projektet fick jag höra från kurskamraterna att enhetstester inte behövdes, och att frontend inte alls betygsattes.

