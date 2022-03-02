FROM mcr.microsoft.com/dotnet/sdk:3.1 AS netbuilder
WORKDIR /repo
COPY VVVVID-Downloader .
RUN dotnet publish VVVVID-Downloader.WebApi/VVVVID-Downloader.WebApi.csproj -o /publish/VVVVID-Downloader

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS prod
WORKDIR /api
COPY --from=netbuilder /publish/VVVVID-Downloader .
ENTRYPOINT ["dotnet", "VVVVID-Downloader.WebApi.dll"]