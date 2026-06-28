FROM node:22-alpine AS frontend
WORKDIR /src/pharmacy-ui
COPY pharmacy-ui/package.json pharmacy-ui/package-lock.json ./
RUN npm ci
COPY pharmacy-ui/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend
WORKDIR /src
COPY PharmacyApi/ PharmacyApi/
COPY --from=frontend /src/pharmacy-ui/dist/pharmacy-ui/browser ./PharmacyApi/wwwroot
RUN dotnet publish PharmacyApi/PharmacyApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=backend /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "PharmacyApi.dll"]
