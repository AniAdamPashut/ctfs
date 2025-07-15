FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /build

COPY server .

RUN dotnet build

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /build/bin/Debug/net9.0/ .
COPY flag .

ENTRYPOINT [ "./server" ]