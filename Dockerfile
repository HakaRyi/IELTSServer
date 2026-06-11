# Dockerfile dùng chung cho mọi .NET service.
# Truyền PROJECT_PATH (đường dẫn .csproj) và ASSEMBLY (tên .dll) qua build args.
# Build context = thư mục backend/ (để các ProjectReference như Shared.Core resolve được).

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT_PATH
WORKDIR /src
COPY . .
RUN dotnet publish "$PROJECT_PATH" -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
ARG ASSEMBLY
WORKDIR /app
COPY --from=build /app .
ENV ASSEMBLY=$ASSEMBLY
# shell form để biến ASSEMBLY được expand lúc chạy
ENTRYPOINT ["sh", "-c", "dotnet $ASSEMBLY"]
