# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy nuget configurations for package restoring
COPY nuget.config .

COPY . .
RUN dotnet restore "CMSTrain/CMSTrain.csproj"
RUN dotnet build "CMSTrain/CMSTrain.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "CMSTrain/CMSTrain.csproj" -c Release -o /app/publish

# Final Stage (nginx)
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80