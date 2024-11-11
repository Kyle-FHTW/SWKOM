FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER root
WORKDIR /app

# Install required packages
RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    libleptonica-dev \
    libtesseract-dev \
    libc6-dev \
    libjpeg62-turbo-dev \
    libgdiplus \
    ghostscript \
    libpng-dev

USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OcrWorker/OcrWorker.csproj", "OcrWorker/"]
RUN dotnet restore "OcrWorker/OcrWorker.csproj"
COPY . .
WORKDIR "/src/OcrWorker"
RUN dotnet build "OcrWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OcrWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OcrWorker.dll"]