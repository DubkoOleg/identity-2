В командной строке на уровне солюшена выполнить команды: 
// dotnet tool install --global dotnet-ef

dotnet ef migrations add initial -s OlMag.Manufacture.Api -p OlMag.Manufacture.DataAccess

dotnet ef database update -s OlMag.Manufacture.Api -p OlMag.Manufacture.DataAccess