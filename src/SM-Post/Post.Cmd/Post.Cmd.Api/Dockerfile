FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj", "SM-Post/Post.Cmd/Post.Cmd.Api/"]
COPY ["CQRS-ES/CQRS.Core/CQRS.Core.csproj", "CQRS-ES/CQRS.Core/"]
COPY ["SM-Post/Post.Common/Post.Common/Post.Common.csproj", "SM-Post/Post.Common/Post.Common/"]
COPY ["SM-Post/Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj", "SM-Post/Post.Cmd/Post.Cmd.Domain/"]
COPY ["SM-Post/Post.Cmd/Post.Cmd.Infrastructure/Post.Cmd.Infrastructure.csproj", "SM-Post/Post.Cmd/Post.Cmd.Infrastructure/"]
RUN dotnet restore "SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj"
COPY . .
WORKDIR "/src/SM-Post/Post.Cmd/Post.Cmd.Api"
RUN dotnet build "Post.Cmd.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Post.Cmd.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Cmd.Api.dll"]
