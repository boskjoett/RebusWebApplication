FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

# Copy binaries from local relative path to WORKDIR in image
COPY RebusWebApplication/bin/Release/netcoreapp3.1/publish/ ./

ENTRYPOINT ["dotnet", "RebusWebApplication.dll"]
