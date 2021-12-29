FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tofu.Web/Tofu.Web.csproj", "Tofu.Web/"]
RUN dotnet restore "Tofu.Web/Tofu.Web.csproj"
COPY . .
WORKDIR "/src/Tofu.Web"
RUN dotnet build "Tofu.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tofu.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tofu.Web.dll"]
