# Nuclear no Dia a Dia — NuclearQuiz (.NET)

Aplicação console em **C# / .NET 10** que gerencia o banco de perguntas do quiz educativo *"Nuclear no Dia a Dia"*, com cadastro, consulta e um modo de jogo, tudo persistido em **Oracle**.

---

## Grupo e integrantes

| Nome | RM | Turma |
|---|---|---|
| Mariana de Paula Aguiar | RM566850 | 2TDSPB |
| Gustavo Miguel Martins de Oliveira | RM566666 | 2TDSPB |
| Manuella Moraes Rodrigues Rinaldi | RM567915 | 2TDSPS |

---

## Descrição resumida da solução

O **Nuclear no Dia a Dia** é uma solução educativa que aproxima jovens e escolas do tema da energia nuclear, mostrando onde ela aparece na vida cotidiana (saúde, energia, alimentos, meio ambiente etc.) por meio de curiosidades, quizzes, mapa de aplicações e ranking.

Esta entrega é o **módulo .NET** da solução: um console que administra o **banco de perguntas** do quiz. Cada pergunta pertence a um **conteúdo**, que por sua vez pertence a uma **categoria**, e a aplicação garante que apenas perguntas válidas entrem na base.

Este módulo **complementa a API Java** do grupo: enquanto a API Java cuida do CRUD de conteúdos/curiosidades, o console .NET cuida das **perguntas do quiz**, com suas próprias regras de negócio.

---

## O que foi desenvolvido nesta Sprint

- **Cadastro de perguntas** com alternativas A, B, C (obrigatórias) e D (opcional), resposta correta e vínculo com um conteúdo existente.
- **Consulta de perguntas**: listar todas, buscar por **categoria** e buscar por **trecho do enunciado** (busca ignora maiúsculas/minúsculas e acentos).
- **Modo jogo**: sorteia 5 perguntas aleatórias, recebe as respostas do jogador e exibe a pontuação final.
- **Regras de negócio e validações** (camada de domínio):
  - enunciado com 10 a 250 caracteres;
  - alternativas A, B e C obrigatórias (mínimo de 3 opções) e D opcional, cada uma com no máximo 120 caracteres;
  - alternativas não podem se repetir;
  - resposta correta deve ser A, B, C ou D (e D só vale se a alternativa D existir);
  - a pergunta precisa estar ligada a um conteúdo existente (que já traz a categoria);
  - não é permitido cadastrar enunciado duplicado dentro do mesmo conteúdo.
- **Tratamento de erros** com exceções próprias (`ValidacaoException` e `RegraNegocioException`), exibindo mensagens claras ao usuário.
- **Persistência em Oracle** com Entity Framework Core (`Oracle.EntityFrameworkCore`), mapeando as tabelas `CATEGORIA`, `CONTEUDO` e `PERGUNTA`, e usando a sequence `SEQ_PERGUNTA` para gerar os IDs.
- **Arquitetura em camadas**, separando domínio, serviço, repositório e acesso a dados.

### Estrutura do projeto

```
NuclearQuiz/
├── NuclearQuiz.sln
├── NuclearQuiz.Domain/          # Regras e modelos (sem dependência de banco)
│   ├── Models/                  # Categoria, Conteudo, Pergunta (com Validar/Normalizar)
│   ├── Exceptions/              # ValidacaoException, RegraNegocioException
│   └── Utils/                   # Texto.Normalizar (minúsculas + sem acentos)
└── NuclearQuiz/                 # Aplicação console
    ├── Program.cs               # Menu e interação com o usuário
    ├── Services/                # PerguntaService (regras) e QuizJogo (modo jogo)
    ├── Repositories/            # PerguntaRepository (consultas EF Core)
    └── Data/                    # QuizDbContext (mapeamento Oracle)
```

---

## Instruções de execução

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Acesso a um banco **Oracle** com as tabelas `CATEGORIA`, `CONTEUDO`, `PERGUNTA` e a sequence `SEQ_PERGUNTA` já criadas e populadas (scripts DDL/inserts da entrega de Banco de Dados).

### 1. Clonar o repositório

```bash
git clone https://github.com/GustavoMiguelk/NuclearQuiz.git
cd NuclearQuiz
```

### 2. Configurar a conexão com o Oracle

A aplicação lê a connection string da variável de ambiente `ORACLE_CONNECTION_STRING` (nenhuma senha fica no código).

**Windows (PowerShell):**
```powershell
$env:ORACLE_CONNECTION_STRING = "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL"
```

**Linux / macOS:**
```bash
export ORACLE_CONNECTION_STRING="User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL"
```

> Ajuste usuário, senha e `Data Source` para o seu ambiente. Se a variável não estiver definida, o programa avisa e encerra.

### 3. Restaurar, compilar e executar

```bash
dotnet restore
dotnet build
dotnet run --project NuclearQuiz
```

### 4. Usando o menu

```
=== Nuclear no Dia a Dia - Banco de Perguntas ===
1 - Cadastrar pergunta
2 - Listar todas as perguntas
3 - Buscar por categoria
4 - Buscar por trecho do enunciado
5 - Jogar (5 perguntas sorteadas)
0 - Sair
```

<details>
<summary>Estrutura de tabelas esperada pela aplicação (referência)</summary>

Se precisar recriar as tabelas, estas são as colunas que o `QuizDbContext` espera (os tipos abaixo foram deduzidos das regras de validação):

```sql
CREATE TABLE CATEGORIA (
    ID        NUMBER PRIMARY KEY,
    NOME      VARCHAR2(100) NOT NULL,
    DESCRICAO VARCHAR2(255)
);

CREATE TABLE CONTEUDO (
    ID           NUMBER PRIMARY KEY,
    TITULO       VARCHAR2(150) NOT NULL,
    NIVEL        VARCHAR2(30)  NOT NULL,
    CATEGORIA_ID NUMBER NOT NULL REFERENCES CATEGORIA (ID)
);

CREATE TABLE PERGUNTA (
    ID               NUMBER PRIMARY KEY,
    ENUNCIADO        VARCHAR2(250) NOT NULL,
    OPCAO_A          VARCHAR2(120) NOT NULL,
    OPCAO_B          VARCHAR2(120) NOT NULL,
    OPCAO_C          VARCHAR2(120) NOT NULL,
    OPCAO_D          VARCHAR2(120),
    RESPOSTA_CORRETA CHAR(1) NOT NULL,
    CONTEUDO_ID      NUMBER NOT NULL REFERENCES CONTEUDO (ID)
);

CREATE SEQUENCE SEQ_PERGUNTA START WITH 1 INCREMENT BY 1;
```

</details>

---

## Como o .NET se aplica à solução do grupo?

O .NET é a camada responsável pelo **banco de perguntas** do quiz do Nuclear no Dia a Dia, funcionando como uma ferramenta de administração que complementa a API Java (que gerencia os conteúdos e curiosidades). Com C# e Entity Framework Core, a aplicação se conecta ao mesmo banco Oracle do projeto e permite cadastrar e consultar as perguntas que alimentam o quiz do app mobile. O ponto central é a **regra de negócio no domínio**: uma pergunta só é aceita se tiver enunciado válido, ao menos três alternativas distintas, resposta correta coerente e vínculo com um conteúdo (e, portanto, com uma categoria) existente, evitando que dados inconsistentes cheguem ao jogo. A separação em camadas (Domain, Services, Repositories e Data) deixa o código organizado e reaproveitável, e o modo de jogo no console permite testar rapidamente se o banco de perguntas está funcionando como o jogador final vai enxergar.
