# Catálogo de Produtos

Projeto de estudos: API para cadastro de produtos e categorias, com autenticação JWT e persistência em SQLite.

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- (Opcional) [Visual Studio Code](https://code.visualstudio.com/) ou outro editor de sua preferência

## Como executar

1. **Restaurar dependências**

   No terminal, execute:
   ```sh
   dotnet restore
   ```

2. **Aplicar as migrations e criar o banco de dados**

   ```sh
   dotnet ef database update
   ```

3. **Executar a aplicação**

   ```sh
   dotnet run
   ```

   A API estará disponível em `http://localhost:5217` (ou porta configurada).

4. **Testar a API**

   Você pode usar os arquivos `.http` ([Catalogo-de-Produtos.http](Catalogo-de-Produtos.http) ou [teste-catalogo-api.http](teste-catalogo-api.http)) no VS Code com a extensão "REST Client" ou utilizar ferramentas como Postman/Insomnia.

## Funcionalidades

- CRUD de Categorias (`/api/categoria`)
- CRUD de Produtos (`/api/produto`)
- Filtros e paginação para produtos
- Cadastro e login de usuários (`/api/auth/cadastro` e `/api/auth/login`)
- Autenticação JWT

## Configuração

As configurações de conexão e JWT estão em [appsettings.json](appsettings.json).

## Observações

- Para acessar os endpoints protegidos, é necessário autenticar-se e enviar o token JWT no header `Authorization: Bearer {token}`.
- O banco de dados SQLite será criado automaticamente como `catalogo.db` na raiz do projeto.

---
*PROJETO DE ESTUDOS CATALOGO DE PRODUTOS*