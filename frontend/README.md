# ProcessoTop - Frontend

Este é o frontend do sistema ProcessoTop, responsável por toda a interface de gerenciamento de processos, andamentos e partes.

## Tecnologias e Bibliotecas Usadas

- **Angular 20**: O framework usado, rodando com a abordagem de Standalone Components (sem NgModules).
- **PrimeNG**: Biblioteca de componentes de UI.
- **PrimeFlex**: Classes utilitárias de CSS no estilo do Tailwind.
- **Angular Reactive Forms**: Usado para construir os formulários de cadastro e aplicar validações.
- **RxJS**: Controle do fluxo das requisições para a API e reatividade de componentes.

## Estrutura de Diretórios

O projeto foi organizado de forma modular, priorizando a divisão por contexto de negócio (Features) e separando os componentes que fazem lógica mais complexa dos componentes puramente visuais (Smart vs Dumb Components).

```text
src/app/
├── core/         # Serviços - interceptor de errors global
├── features/     # Páginas e fluxos principais
│   ├── entidades-legais/
│   └── processos/
└── shared/       # Componentes simples e reutilizáveis
    ├── components/         # Formulários, tags, botões, etc
    └── ui/                 # Componentes de layout - menu, header, etc
```

## Como Rodar

A maneira recomendada e mais fácil de rodar o projeto inteiro (banco de dados, API e frontend) é utilizando o Docker Compose. 

As instruções de como rodar com Docker estão no **[README principal](../README.md)**.


### Rodando o Frontend isoladamente (Node)

Caso queira subir apenas a interface gráfica direto na sua máquina (sem Docker):

1. Dentro desta pasta `frontend`, instale as dependências:
   ```bash
   npm install
   ```
2. Inicie o servidor de desenvolvimento:
   ```bash
   npm start
   ```

*(O servidor subirá em `http://localhost:4200` e tentará se comunicar com a API mapeada nas `environments`)*.
