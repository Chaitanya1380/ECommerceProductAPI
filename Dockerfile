# Use official ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ECommerceProjectAPI/ECommerceProjectAPI.csproj"
RUN dotnet publish "ECommerceProjectAPI/ECommerceProjectAPI.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerceProjectAPI.dll"]
