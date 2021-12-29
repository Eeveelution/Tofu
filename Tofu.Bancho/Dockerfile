FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tofu.Bancho/Tofu.Bancho.csproj", "Tofu.Bancho/"]
RUN dotnet restore "Tofu.Bancho/Tofu.Bancho.csproj"
COPY . .
WORKDIR "/src/Tofu.Bancho"
RUN dotnet build "Tofu.Bancho.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tofu.Bancho.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tofu.Bancho.dll"]
