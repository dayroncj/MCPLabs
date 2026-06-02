# Lab de MCP servers con .NET

Repositorio de intro a servidores **MCP (Model Context Protocol)** con .NET.

⚠️ Aunque el proyecto incluye configuración de DevContainer, no usarlo ya que no es posible usar el MCP Inspector ni comunicarse con el MCP server desde un devcontainer

## Estructura del proyecto

```
MCPLabs/
├── .vscode/               # Configuración del workspace para VS Code
│   ├── mcp.json           # Registro de servidores MCP para que VS Code / GitHub Copilot los descubra automáticamente
│   └── extensions.json    # Extensiones recomendadas: se instalan automáticamente al abrir el proyecto
│
├── ro_snacks/             # Servidor MCP de snacks: expone tools y resources sobre el agendamiento
│   ├── Tools/             # SnackTools: herramientas para consultar y registrar snacks via API
│   └── Resources/         # SnacksResources: menú semanal y calorías por fruta como recursos MCP
│
├── SnacksApi/             # API REST (ASP.NET Core, .NET 9) que sirve de backend para ro_snacks
│                          # Endpoints: GET/POST /snacksSchedule
│
├── weather/               # Nada importante solo un api con el que se empezaron las pruebas de mcp desde un devcontainer
│   └── WeatherTools/      
│
├── .devcontainer/         # Configuración de Dev Container con .NET preinstalado
└── global.json            # Versión del SDK de .NET requerida
```

## Prerrequisitos

- Rancher desktop (... para usar el Dev Container)
- O bien .NET SDK 9+ instalado localmente
   - Las extensiones de VS Code requeridas se instalan automáticamente al abrir el proyecto (ver `.vscode/extensions.json`)

---

## Cómo empezar?

### Probemos que snacksApi compile y se ejecute
```
cd SnacksApi
dotnet build
dotnet run
```

Luego abrir el archivo SnacksApi.http, teniendo la extensión Rest API instalada será posible hacer peticiones al api desde este archivo.

### Ahora el MCP Server
```
cd ro_snacks
dotnet build
dotnet run
```

#### Prueba desde la terminal
Una vez ha iniciado el MCP Server, se quedará escuchando, podemos enviar este json rpc y recibiremos una respuesta en el mismo formato:

`{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test","version":"1.0"}}}`

#### Prueba desde MCP Inspector
Abrir otra terminal y ejecutar el inspector
```
cd ro_snacks
npx @modelcontextprotocol/inspector dotnet run --project .
```

## Cómo publicar MCP Server en Azure?

### Prerequisitos
Crear web application 
`dotnet new web -n ro_soccer`

E instalar:
`dotnet add package ModelContextProtocol --prerelease`
`dotnet add package ModelContextProtocol.AspNetCore --prerelease`

Tener instalado az login

### Usando Azure CLI
az login

#### Crear grupo de recursos
az group create --name rg-mcp-soccer --location eastus

#### Crear App Service Plan
az appservice plan create --name plan-mcp-soccer \
  --resource-group rg-mcp-soccer \
  --sku B1 --is-linux

#### Crear la Web App
az webapp create --name ro-soccer-mcp \
  --resource-group rg-mcp-soccer \
  --plan plan-mcp-soccer \
  --runtime "DOTNETCORE:10.0"

#### Publicar (desde la carpeta ro_soccer/)
cd ro_soccer
dotnet publish -c Release -o ./publish
az webapp deploy --resource-group rg-mcp-soccer \
  --name ro-soccer-mcp \
  --src-path ./publish \
  --type zip