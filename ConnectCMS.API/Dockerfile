# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder
WORKDIR /app

# Copy nuget configurations for package restoring
COPY nuget.config .

# Copy everything and build
COPY . .
RUN dotnet restore CMSTrain.API/ --configfile nuget.config
RUN dotnet publish -c Release -r linux-musl-x64 --self-contained false -o release

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

# Copy the build output from the builder stage
COPY --from=builder /app/release .
COPY --from=builder /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/

# Install necessary libraries
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib

# Change permissions to allow writing to wwwroot
RUN chmod -R 777 /app/wwwroot

# Set environment variable to ensure globalization works
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Define the entry point for the application
CMD ["dotnet", "CMSTrain.API.dll"]