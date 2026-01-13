# 🛠️ SuporteIA – HelpTRAVAS  
### Sistema Inteligente de Suporte Técnico com IA

O **SuporteIA – HelpTRAVAS** é um sistema web completo de **gestão de chamados técnicos**, desenvolvido em **ASP.NET Core MVC (.NET 8)** e integrado com **Inteligência Artificial (OpenRouter AI)** para gerar sugestões automáticas de solução, classificações e apoio técnico imediato.

O sistema oferece uma interface moderna, com autenticação, permissões, painel administrativo, FAQ inteligente e suporte à LGPD.

---

## 🚀 Funcionalidades

### ✔ **Chamados**
- Criação de chamados técnicos
- Sugestão automática via IA (OpenRouter)
- Atualização de status, prioridade e categoria
- Histórico e logs de auditoria de IA
- Lista geral com filtros

### ✔ **Autenticação e Permissões**
- Login e registro de usuários (ASP.NET Identity)
- Papéis:
  - Usuário
  - Técnico
  - Administrador
- Painel de gestão de usuários (Admin)

### ✔ **Inteligência Artificial**
- IA sugere solução curta e objetiva
- Fallback automático de modelos gratuitos:
  - Llama 3.1 8B
  - Mistral 7B
  - MythoMax

### ✔ **Dashboard**
- Gráficos de chamados por status e categoria
- Estatísticas gerais

### ✔ **FAQ Inteligente**
- Busca por problemas comuns
- Integração com sugestões automatizadas

### ✔ **LGPD**
- Usuário pode excluir todos os dados pessoais
- Dados anonimizados nos chamados

---

## 🧰 Tecnologias Utilizadas

| Camada | Tecnologia |
|--------|------------|
| Backend | ASP.NET Core MVC (.NET 8) |
| Frontend | Bootstrap 5, HTML, CSS, Razor |
| Banco de Dados | SQL Server / SQLite (modo portable) |
| Autenticação | ASP.NET Identity |
| IA | OpenRouter API |
| Build | Publicação Standalone (.exe) |

---

## 🛑 Requisitos

- .NET 8 instalado  
- SQL Server LocalDB ou SQLite  
- Chave da OpenRouter: https://openrouter.ai  
- Windows 10+ (para a versão .exe publicada)

---

## ⚙️ Configuração

### appsettings.json

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=SuporteIA.db"
},
"OpenRouter": {
  "ApiKey": "SUA_API_KEY",
  "Referer": "HelpTravas",
  "AppName": "SuporteIA"
}
