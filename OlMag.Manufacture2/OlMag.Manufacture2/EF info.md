В командной строке на уровне солюшена выполнить команды: 
// dotnet tool install --global dotnet-ef

1. dotnet ef migrations add initial

2. временно сменить в строке соединения на "Server=localhost..."

3. dotnet ef database update

4. вернуть строку соединения на "Server=host.docker.internal..."