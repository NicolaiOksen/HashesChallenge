﻿FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Processor/Processor.csproj", "Processor/"]
RUN dotnet restore "Processor/Processor.csproj"
COPY . .
WORKDIR "/src/Processor"
RUN dotnet build "Processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Processor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processor.dll"]
