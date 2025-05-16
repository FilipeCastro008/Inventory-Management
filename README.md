# 📦 InventoryManagement

Sistema de gerenciamento de estoque com API RESTful construída em .NET.

---

## ✅ Pré-requisitos

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ ou VS Code
- SQL Server (local)

---

## ▶️ Como rodar o projeto

1. **Restaure os pacotes NuGet**

```bash
dotnet restore
```

2. **Configure a string de conexão**

No arquivo `appsettings.json` do projeto `InventoryManagement`, configure sua string de conexão com o banco:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InventoryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. **Execute as migrations**

```bash
dotnet ef database update
```

4. **Execute a aplicação**

```bash
dotnet run --project InventoryManagement
```

A API estará disponível em:

- `http://localhost:5129`

---

## 🧪 Como rodar os testes unitários

1. **Acesse o diretório de testes**

```bash
cd InventoryManagement.Tests
```

2. **Execute os testes**

```bash
dotnet test
```

Os testes utilizam:

- **xUnit** – estrutura de testes
- **Moq** – mocks dos repositórios

---

## 📂 Estrutura de Projetos

- `InventoryManagement` — Projeto principal da API
- `InventoryManagement.Application` — Lógica de negócio e serviços
- `InventoryManagement.Domain` — Entidades e interfaces de domínio
- `InventoryManagement.Infra` — Implementação de repositórios
- `InventoryManagement.Tests` — Testes unitários com xUnit
- `InventoryManagement.View` — Páginas Razor
- `InventoryManagement.Presentation` — Controller

---

## ✍️ Observações

- Ao adicionar um novo produto, o sistema valida o nome, descrição e preço.
- O preço pode ser inserido como string (`"R$ 1.234,56"`) e será convertido automaticamente.
- Produtos com estoque 0 não podem ser removidos.

---# Inventory-Management
