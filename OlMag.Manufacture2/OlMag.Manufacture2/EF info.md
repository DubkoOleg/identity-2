В командной строке на уровне солюшена выполнить команды: 
// dotnet tool install --global dotnet-ef

1. dotnet ef migrations add initial --context SalesManagementContext

2. временно сменить в строке соединения на "Server=localhost..."

3. dotnet ef database update --context SalesManagementContext

4. вернуть строку соединения на "Server=host.docker.internal..."

5. dotnet ef migrations add AddBlogCreatedTimestamp --context SalesManagementContext

В PowerShell:

1. Add-Migration initial --context SalesManagementContext

2. временно сменить в строке соединения на "Server=localhost..."

3. Update-Database --context SalesManagementContext

4. вернуть строку соединения на "Server=host.docker.internal..."

5. Add-Migration AddBlogCreatedTimestamp --context SalesManagementContext



*https://learn.microsoft.com/ru-ru/ef/core/managing-schemas/migrations/?tabs=vs*