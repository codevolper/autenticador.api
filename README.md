# Autenticador.API

Objetivo
--------
Esta API tem como objetivo prover um serviço mínimo de autenticação para aplicações, seguindo princípios de Clean Architecture, Clean Code e SOLID. A API permite cadastro de usuário (e-mail + senha), autenticação via login que gera um token JWT e validação desse token por outros serviços.

Visão técnica
-------------
- Arquitetura em camadas (limpa): Domain, Application, Infrastructure e API.
- Persistência: não usa Entity Framework. A camada de infraestrutura mantém um DataSet/DataTable em memória (simulando tabelas relacionais com PK e constraint única em E-mail). Isso facilita testes e demonstração sem banco externo.
- Mapeamento/ORM: Dapper é referenciado para compatibilidade futura, mas o armazenamento atual usa DataTable diretamente para cumprir o requisito de DataSet/DataTable.
- Hash de senha: PBKDF2 (Rfc2898DeriveBytes) com salt e iterações; senha armazenada como "iteracoes.salt.hash" em Base64.
- Autenticação: JWT (Bearer). Configurações em appsettings.json: Jwt:ChaveSecreta, Jwt:Emissor, Jwt:Publico, Jwt:ExpiracaoMinutos (padrão 60).
- Endpoints principais:
  - POST /api/usuarios/cadastrar -> cadastra usuário (email, senha)
  - POST /api/usuarios/login -> autentica e retorna access_token (JWT)
  - GET  /api/usuarios/perfil -> protegido, retorna dados do usuário autenticado
  - POST /api/usuarios/validar-token -> valida um token JWT enviado no body ou no header Authorization
- Swagger: integrado e habilitado por padrão para testes interativos.

Como baixar e executar
----------------------
1. Clone o repositório:

```powershell
git clone https://github.com/codevolper/Autenticador.API.git
cd Autenticador.API
```

2. Restaurar dependências e executar (PowerShell):

```powershell
dotnet restore src/Autenticador.API/Autenticador.API.csproj
dotnet run --project src/Autenticador.API/Autenticador.API.csproj
```

3. Acesse o Swagger para testar os endpoints: a URL será exibida no console (ex.: https://localhost:5001/swagger ou http://localhost:5000/swagger).

Observações de segurança e produção
----------------------------------
- Troque a chave em appsettings.json (Jwt:ChaveSecreta) por um segredo forte e gerenciado (Azure Key Vault, AWS Secrets Manager, etc.).
- Em produção, use um banco de dados real e ajuste a implementação de repositório para usar Dapper/IDbConnection contra SQL Server ou outro SGBD.

Dependências e versão do .NET
-----------------------------
- Target Framework: .NET 10 (net10.0)
- Pacotes NuGet usados:
  - Dapper 2.1.0
  - Swashbuckle.AspNetCore 6.5.0 (Swagger)
  - Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0

Suporte e próximos passos
------------------------
Posso:
- Adicionar validações adicionais e testes automatizados.
