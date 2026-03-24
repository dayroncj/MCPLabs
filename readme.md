El devcontainer ya se encarga de hacer donet restore

Ubicarse desde el folder de la herramienta y ejecutar la aplicación
`cd weather && dotnet run`

Prueba de diagnóstico
Una vez ha iniciado la aplicación se quedará escuchando, podemos enviar este json rpc y recibiremos una respuesta en el mismo formato:
`{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test","version":"1.0"}}}`

Probar desde MCP Inspector
Abrir otra terminal y ejecutar el inspector
`cd weather && npx @modelcontextprotocol/inspector dotnet run --project .`