# API de Gerenciamento de Usuários

## Descrição
Este projeto foi desenvolvido como parte da Avaliação Semestral da disciplina Desenvolvimento Backend.
Ele consiste em uma API REST construída com ASP.NET Core Minimal APIs, seguindo princípios de Clean Architecture e aplicando padrões de projeto profissionais.

A API permite:
 - Criar usuários
 - Consultar usuários
 - Atualizar informações
 - Deletar usuários (soft delete)
 - Validar dados com FluentValidation
 - Persistir informações usando EF Core + SQLite

## Tecnologias Utilizadas
 - .NET 8.0
 - ASP.NET Core Minimal APIs
 - Entity Framework Core 8.0
 - SQLite
 - FluentValidation 11.3
 - Swagger/OpenAPI

## Padrões de Projeto Implementados
 - Repository Pattern
Responsável por toda a camada de acesso ao banco de dados.
Interface: IUsuarioRepository
Implementação: UsuarioRepository

 - Service Pattern
Lida com a lógica de negócio.
Interface: IUsuarioService
Implementação: UsuarioService

 - DTO Pattern
Separa as entidades reais da troca de dados na API.
    DTOs implementados:
    UsuarioCreateDto
    UsuarioReadDto
    UsuarioUpdateDto

## Como Executar o Projeto

### Pré-requisitos
- .NET SDK 8.0 ou superior
- Visual Studio 2022, VS Code ou qualquer editor compatível

### Passos
# 1. Clonar o repositório
git clone https://github.com/paulodagostin/api-usuarios-as-paulo-dagostin.git

# 2. Entrar no projeto
cd api-usuarios-as-paulo-dagostin/APIUsuarios

# 3. Aplicar as migrations
dotnet ef database update

# 4. Executar o projeto
dotnet run

- Swagger disponível em:
http://localhost:5295/swagger/index.html

### Endpoints Disponíveis
- GET /usuarios - Lista todos os usuários
- GET /usuarios/{id} - Busca usuário por ID
- POST /usuarios - Cria novo usuário
- PUT /usuarios/{id} - Atualiza usuário completo
- DELETE /usuarios/{id} - Remove usuário (soft delete)

## Estrutura do Projeto
APIUsuarios/
├── APIUsuarios.csproj
├── APIUsuarios.db
├── APIUsuarios.http
│
├── Domain/
│   └── Entities/
│       └── Usuario.cs
│
├── Application/
│   ├── DTOs/
│   │   ├── UsuarioCreateDto.cs
│   │   ├── UsuarioReadDto.cs
│   │   └── UsuarioUpdateDto.cs
│   │
│   ├── Interfaces/
│   │   ├── IUsuarioRepository.cs
│   │   └── IUsuarioService.cs
│   │
│   ├── Services/
│   │   └── UsuarioService.cs
│   │
│   └── Validators/
│       ├── UsuarioCreateDtoValidator.cs
│       └── UsuarioUpdateDtoValidator.cs
│
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs
│   │
│   └── Repositories/
│       └── UsuarioRepository.cs
│
├── Migrations/
│   ├── 20251203032929_Initial.cs
│   ├── 20251203032929_Initial.Designer.cs
│   └── AppDbContextModelSnapshot.cs
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── README.md

## Exemplos de Requisições

### Criar Usuário
```json
POST /usuarios
{
  "nome": "Maria Silva",
  "email": "maria@gmail.com",
  "senha": "123456",
  "dataNascimento": "2000-05-20",
  "telefone": "(47) 99999-8888"
}

Atualizar usuário – PUT /usuarios/
{
  "nome": "Maria Souza",
  "email": "maria@gmail.com",
  "dataNascimento": "2000-05-20",
  "telefone": "(47) 99999-8888",
  "ativo": true
}

Erro de validação (exemplo)
{
  "errors": [
    {
      "propertyName": "Email",
      "errorMessage": "'Email' deve ser um endereço de email válido."
    }
  ]
}

## Autor

Paulo Ricardo Dagostin Rosso

RA: Terça-Feira - Noite

Curso: Analise e Desenvolvimento de Sistemas