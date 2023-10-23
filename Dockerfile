FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG TARGETARCH

RUN apt update && \
    apt install -y wget && \
    apt install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_18.x | bash - && \
    apt install -y build-essential nodejs && \
    npm install -g pnpm

COPY "Me.Xfox.ZhuiAnime.sln" "/src/"
COPY "NuGet.config" "/src/"
COPY "Me.Xfox.ZhuiAnime/Me.Xfox.ZhuiAnime.csproj" "/src/Me.Xfox.ZhuiAnime/"
COPY "Me.Xfox.ZhuiAnime/packages.lock.json" "/src/Me.Xfox.ZhuiAnime/"
COPY "Me.Xfox.ZhuiAnime.WebUI/package.json" "/src/Me.Xfox.ZhuiAnime.WebUI/"
COPY "Me.Xfox.ZhuiAnime.WebUI/pnpm-lock.yaml" "/src/Me.Xfox.ZhuiAnime.WebUI/"
WORKDIR "/src"
RUN dotnet restore "Me.Xfox.ZhuiAnime" --locked-mode -a $TARGETARCH

COPY "." "/src/"
RUN dotnet build "Me.Xfox.ZhuiAnime" -c Release --no-self-contained --no-restore -a $TARGETARCH
RUN dotnet publish "Me.Xfox.ZhuiAnime" -c Release -o /app/publish --no-self-contained  --no-restore -a $TARGETARCH

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Me.Xfox.ZhuiAnime.dll"]
