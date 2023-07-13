FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

RUN apt update && \
    apt install -y wget && \
    apt install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_20.x | bash - && \
    apt install -y build-essential nodejs

RUN npm i -g pnpm

COPY [".", "/src/"]
WORKDIR "/src"
RUN dotnet restore "Me.Xfox.ZhuiAnime"
RUN dotnet publish "Me.Xfox.ZhuiAnime" -c Release -o /app/publish -r linux-x64 --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Me.Xfox.ZhuiAnime.dll"]
