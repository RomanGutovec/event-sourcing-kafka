FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SM-Post/Post.Query/Post.Query.Api/Post.Query.Api.csproj", "SM-Post/Post.Query/Post.Query.Api/"]
RUN dotnet restore "SM-Post/Post.Query/Post.Query.Api/Post.Query.Api.csproj"
COPY . .
WORKDIR "/src/SM-Post/Post.Query/Post.Query.Api"
RUN dotnet build "Post.Query.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Post.Query.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Query.Api.dll"]
