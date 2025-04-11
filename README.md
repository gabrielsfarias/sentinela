# Sentinela de Documentos 🚦

[![Status](https://img.shields.io/badge/status-em_desenvolvimento-yellow)](https://github.com/SEU_USUARIO/SentinelaDeDocumentos) **Um rastreador de validade de documentos para licitações, feito com .NET.**

Este projeto visa ajudar empresas a controlar os prazos de validade de certidões e outros documentos necessários para participar de licitações públicas no Brasil, enviando alertas antes do vencimento.

## 🎯 Objetivo do MVP (Produto Mínimo Viável)

* Autenticação de usuários (Cadastro/Login).
* CRUD (Criar, Ler, Atualizar, Deletar) de Documentos da Empresa, com registro manual da **Data de Validade**.
* Listagem de documentos com status (Válido, Próximo Vencimento, Vencido).
* Serviço de background diário para verificar validades.
* Envio de **notificações por email** para documentos próximos do vencimento.

## 🛠️ Tecnologias

* **Backend:** C# / .NET 8 (ou superior)
* **API:** ASP.NET Core Web API
* **Banco (Dev):** SQLite
* **ORM:** Entity Framework Core
* **Auth:** ASP.NET Core Identity
* **Jobs:** `IHostedService`
* **Email:** Serviço externo (SendGrid/Mailgun/etc.)

## 🏗️ Estrutura (Simplificada)

* **Domain:** Entidades (`DocumentoEmpresa`, `TipoDocumento`, etc.), Interfaces.
* **Application:** Lógica de Casos de Uso (Serviços), DTOs.
* **Infrastructure:** EF Core (`DbContext`, Repositórios), Serviços Externos (Email).
* **Api:** Controllers ASP.NET Core, Configuração (`Program.cs`).

## 🚀 Como Executar Localmente

**Pré-requisitos:**
* SDK do .NET 8 (ou superior)
* Git

**Passos:**

1.  **Clonar:**
    ```bash
    git clone [https://github.com/SEU_USUARIO/SentinelaDeDocumentos.git](https://github.com/SEU_USUARIO/SentinelaDeDocumentos.git)
    cd SentinelaDeDocumentos
    ```
    *(Substitua pela URL do seu repositório)*
2.  **Restaurar Pacotes:**
    ```bash
    dotnet restore Sentinela.sln
    ```
3.  **Aplicar Migrations (Criar/Atualizar BD):**
    ```bash
    # Estando na pasta raiz da solução:
    dotnet ef database update --project src/Sentinela.Infrastructure --startup-project src/Sentinela.Api
    ```
    *(Cria/atualiza o arquivo .db no projeto Api)*
4.  **Configurar Segredos (Ex: Chave API Email - Fase 2):**
    ```bash
    # Exemplo (na pasta src/Sentinela.Api):
    # dotnet user-secrets set "EmailSettings:ApiKey" "SUA_CHAVE"
    ```
5.  **Executar a API:**
    ```bash
    dotnet run --project src/Sentinela.Api/Sentinela.Api.csproj
    ```
6.  Acesse a API (ex: `https://localhost:7XXX`) via Swagger ou Postman/Insomnia.

## 📜 Licença

Distribuído sob a licença **MIT**. Veja o arquivo `LICENSE` para mais detalhes.

---
Gabriel Souza Farias<>*gabrielsfarias@outlook.com*
