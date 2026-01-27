\# Academia Lounge



Sistema de gestão para academia, composto por:

\- \*\*API\*\* (.NET 8 + PostgreSQL) — backend REST com autenticação JWT

\- \*\*Admin Web\*\* (Blazor Web App + MudBlazor) — painel administrativo



\## Estrutura do repositório



\- `AcademiaLounge/` — \*\*API\*\* (ASP.NET Core)

\- `AcademiaLoungeAdmin/` — \*\*Admin\*\* (Host Blazor Web App)

&nbsp; - `AcademiaLoungeAdmin/AcademiaLoungeAdmin.Client/` — UI (WebAssembly)



\## Requisitos



\- .NET SDK 8

\- PostgreSQL 14+ (ou compatível)

\- (Opcional) Visual Studio 2022



\## Como rodar localmente



\### 1) Banco de dados (PostgreSQL)

Crie o banco `academia` e ajuste a connection string em:

\- `AcademiaLounge/appsettings.json`



Exemplo:

```json

"ConnectionStrings": {

&nbsp; "Default": "Host=localhost;Port=5432;Database=academia;Username=postgres;Password=SUA\_SENHA"

}



