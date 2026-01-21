# ClientesApp

Aplicação web para gerenciamento de clientes, desenvolvida em **ASP.NET Core**, com foco em organização, boas práticas e escalabilidade.

---

## Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- C#
- MVC / Razor Pages
- Banco de Dados: SQL Server ou MySQL
- Git & GitHub

---

## 📂 Estrutura do Projeto

```text
ClientesApp/
│
├── Controllers/        # Controllers da aplicação
├── Data/               # Contexto do banco de dados (EF Core)
├── Filters/            # Filtros personalizados
├── Helpers/            # Classes utilitárias
├── Migrations/         # Migrations do Entity Framework
├── Models/             # Entidades do domínio
├── Views/              # Views Razor
├── wwwroot/            # Arquivos estáticos (CSS, JS, imagens)
│
├── Program.cs          # Configuração principal da aplicação
├── ClientesApp.sln     # Solution
├── ClientesApp.csproj  # Projeto
└── appsettings.json    # Configurações gerais
