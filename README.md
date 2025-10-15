# Rinha Backend 2025

## 🎯Desafio
O objetivo deste desafio consiste em desenvolver uma solução backend resiliente que lide com grandes cargas de requisições com pouca infraestrutura.

O backend irá receber solicitações de pagamentos, que deveram ser enviadas para algum dos serviço externo `Payment Processor Default` ou `Payment Processor Fallback`. Como nem tudo é perfeito, ambos dos serviços podem sofre lentidão ou indisponibilidade por um determinado tempo. 
O objetivo é processar TODOS os pagamentos recebidos para não afetar o lucro da companhia e a indisponibilidade de pagamentos.

[Instruções e detalhes do desafio](https://github.com/zanfranceschi/rinha-de-backend-2025/blob/main/INSTRUCOES.md)

## Arquitetura do projeto

Resumo da arquitetura do sistema

![System design](/doc/arch-drawing.png)

## Como rodar o projeto

1. Clone este projeto projeto

```sh
git clone https://github.com/leeo-sf/rinha-backend-2025.git
```

2. Clone o projeto do payment processor

```sh
git clone https://github.com/zanfranceschi/rinha-de-backend-2025.git
```

*Necessário ter o docker instalado na máquina!*. Execute o comando abaixo para rodar os serviços em modo detached

3. Dentro a pasta `payment-processor` do projeto Payment Processor execute o comando abaixo
```sh
docker compose up -d
```

4. Agora, na pasta raiz deste projeto execute o comando abaixo
```sh
docker compose up -d
```

OBS: É possível executar a aplicação backend sem o docker, mas ainda é necessário subir os serviços do payment processor e subir um container postgres para a aplicação interagir algum banco de dados.

## 💻​ Tecnologias Utilizadas
- .NET 9
- PostgreSQL
- Docker & Docker compose
- Nginx