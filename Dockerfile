# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia tudo da raiz do projeto (onde est� o .sln)
COPY . .

# Restaura depend�ncias e publica o projeto de API
RUN dotnet restore "BackEndElog.Api/BackEndElog.Api.csproj"
RUN dotnet publish "BackEndElog.Api/BackEndElog.Api.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

# DEBUG opcional � verificar se o bin�rio foi copiado corretamente
RUN ls -la

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BackEndElog.Api.dll"]
