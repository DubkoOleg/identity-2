В командной строке на уровне солюшена выполнить команды: 
// dotnet tool install --global dotnet-ef

1. dotnet ef migrations add initial --context SaleManagementContext

2. временно сменить в строке соединения на "Server=localhost..."

3. dotnet ef database update --context SaleManagementContext

4. вернуть строку соединения на "Server=host.docker.internal..."

5. dotnet ef migrations add AddBlogCreatedTimestamp --context SaleManagementContext

В PowerShell:

1. Add-Migration initial --context SaleManagementContext

2. временно сменить в строке соединения на "Server=localhost..."

3. Update-Database --context SaleManagementContext

4. вернуть строку соединения на "Server=host.docker.internal..."

5. Add-Migration AddBlogCreatedTimestamp --context SaleManagementContext



*https://learn.microsoft.com/ru-ru/ef/core/managing-schemas/migrations/?tabs=vs*