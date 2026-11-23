# Archivo:    backend/Dockerfile
# Qué hace:   Imagen multi-stage de los proyectos .NET (API y servicio simulado).
# Recibe:     ARG PROJECT (Acompana.Api | Acompana.Integrations.Mock) desde docker-compose.yml.
# Procesa:    restore + publish Release con el SDK 8; copia el resultado a la imagen aspnet:8.0.
# Devuelve:   Contenedor que escucha en ASPNETCORE_HTTP_PORTS (8080 api, 8081 mock).
# Se conecta: ← docker-compose.yml (servicios api e integrations-mock)
# Estado:     Completo
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT=Acompana.Api
WORKDIR /src
COPY . .
RUN dotnet publish src/${PROJECT}/${PROJECT}.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:11.0
ARG PROJECT=Acompana.Api
ENV APP_DLL=${PROJECT}.dll
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["sh", "-c", "exec dotnet \"$APP_DLL\""]
