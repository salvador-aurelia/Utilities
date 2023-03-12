FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ObsidianNoteLink/ObsidianNoteLink.csproj", "ObsidianNoteLink/"]
RUN dotnet restore "ObsidianNoteLink/ObsidianNoteLink.csproj"
COPY . .
WORKDIR "/src/ObsidianNoteLink"
RUN dotnet build "ObsidianNoteLink.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ObsidianNoteLink.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ObsidianNoteLink.dll"]
