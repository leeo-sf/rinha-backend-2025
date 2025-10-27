# Documentação técina da solução

Este documento tem como objetivo apresentar as principais tecnologias e frameworks utilizados no projeto, bem como o motivo que levaram a escolha das mesmas.
A intenção é fornecer uma visão técnica e clara sobre as decisões de arquitetura e de stack adotadas, servindo como referência para manutenção e evolução.

## 📚 Índice

A documentação esta dividida nas seguintes camadas:

- [Ir para Infraestrutura](#-infraestrutura)
- [Ir para Padrão de Arquitetura e Design](#-padrão-de-arquitetura-e-design)
- [Ir para Backend](#️-backend)
- [Ir para Banco de Dados](#-banco-de-dados)

## 🌐 Infraestrutura

### Nginx
- Utilizado como proxy reverso e balanceador de carga conforme regra do desafio.
- Leve e performático.
- Permite cache, compressão e roteamento eficiente de requisições.
- Facilita a integração com containers Docker.

### Docker & Docker Compose
- Facilita a escalabilidade e o deploy da aplicação.
- Permite isolar serviços (API, banco, Nginx, etc.) em containers independentes.

## 🧱 Padrão de Arquitetura e Design

### Clean Architecture
- Facilita a testabilidade e a substituição de implementações sem afetar outras partes do sistema.
- Garante independência de frameworks, infraestrutura e detalhes externos.
- Melhora a organização do código e a escalabilidade a longo prazo.

### Abstract Factory
- Selecionar dinamicamente a implementação de determinado serviço com base em parâmetros ou configurações.
- Evita o uso de condicionais extensas (`if/else` ou `switch`) no código de negócio.
- Facilita a extensibilidade — novos provedores podem ser adicionados sem alterar o código existente.

## 🏗️ Backend

### Minimal API's
- Menor overhead: menos abstrações e menos uso de reflection.
- Código mais leve, menos dependências do pipeline MVC tradicional.
- Criação novos endpoints com pooucas linhas, sem necessidade de Controllers, atributos ou roteamento complexo.
- Ideal para APIs pequenas e específica, focadas em uma única responsabilidade.

### Entity Framework Core
- Facilita a persistência de dados sem a necessidade de escrever SQL manual.
- Permite o uso de migrations para controle de versões do banco.
- Integra-se nativamente ao .NET.

### Channels .NET
- Proporciona um modelo de processamento assíncrono sem bloqueio (async/await).
- Substitui implementações manuais de filas com `ConcurrentQueue` e `SemaphoreSlim`.
- Lida com alta concorrência e throughput.

### IMemoryCache
- Melhora significativamente o desempenho em consultas frequentes.
- Evita chamadas desnecessárias ao banco de dados ou serviços externos.
- Simples de implementar e já integrado ao ecossistema do .NET.

### Refit
- Simplifica chamadas HTTP a APIs externas utilizando interfaces e atributos.
- Facilita a integração com serviços externos sem precisar lidar diretamente com `HttpClient`.
- Código mais limpo, desacoplado e fácil de testar.
- Integração fluida com injeção de dependência.

## 💾 Banco de Dados

### PostgreSQL
- Open-source, robusto e altamente confiável.
- Suporte avançado a transações e queries complexas.
- Integração nativa com Entity Framework Core.