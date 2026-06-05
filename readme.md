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

## 4. Cómo publicar MCP Server en Azure?

### 4.1 Prerequisitos
Crear web application 
`dotnet new web -n ro_soccer`

E instalar:
`dotnet add package ModelContextProtocol --prerelease`
`dotnet add package ModelContextProtocol.AspNetCore --prerelease`

Tener instalado az login

### 4.2 Usando Azure CLI
az login

#### 4.3 Crear grupo de recursos
az group create --name rg-mcp-soccer --location eastus

#### 4.4 Crear App Service Plan
az appservice plan create --name plan-mcp-soccer \
  --resource-group rg-mcp-soccer \
  --sku B1 --is-linux

#### 4.5 Crear la Web App
az webapp create --name ro-soccer-mcp \
  --resource-group rg-mcp-soccer \
  --plan plan-mcp-soccer \
  --runtime "DOTNETCORE:10.0"

#### 4.6 Publicar (desde la carpeta ro_soccer/)
cd ro_soccer
dotnet publish -c Release -o ./publish
az webapp deploy --resource-group rg-mcp-soccer \
  --name ro-soccer-mcp \
  --src-path ./publish \
  --type zip

## 5. Alternativa: Usando Azure Container Apps

Esta opción no requiere App Service Plan y evita problemas de cuota de VMs.

#### 5.1 Prerrequisitos
```bash
az extension add --name containerapp
az provider register --namespace Microsoft.App
az provider register --namespace Microsoft.OperationalInsights
```

#### 5.2 Crear grupo de recursos
`az group create --name rg-mcp-soccer --location eastus`

#### 5.3 Crear el entorno de Container Apps
```
az containerapp env create \
  --name env-mcp-soccer \
  --resource-group rg-mcp-soccer \
  --location eastus
```

#### 5.4 Publicar y desplegar desde la carpeta ro_soccer
```
cd ro_soccer
dotnet publish -c Release -o ./publish
zip -r publish.zip ./publish

az containerapp create \
  --name ro-soccer-mcp \
  --resource-group rg-mcp-soccer \
  --environment env-mcp-soccer \
  --artifact ./publish.zip \
  --ingress external \
  --target-port 8080
```

#### 5.5 Obtener el url del servicio
```
az containerapp show \
  --name ro-soccer-mcp \
  --resource-group rg-mcp-soccer \
  --query properties.configuration.ingress.fqdn \
  --output tsv
```

La URL resultante será la base para conectar el cliente MCP, por ejemplo: https://ro-soccer-mcp.<id>.eastus.azurecontainerapps.io/sse
