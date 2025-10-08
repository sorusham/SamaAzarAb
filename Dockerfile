# استفاده از .NET 9.0 SDK برای build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# کپی کردن فایل‌های پروژه
COPY ["MessageForAzarab.csproj", "."]
RUN dotnet restore "MessageForAzarab.csproj"

# کپی کردن تمام فایل‌ها
COPY . .
WORKDIR "/src"

# Build کردن پروژه
RUN dotnet build "MessageForAzarab.csproj" -c Release -o /app/build

# Publish کردن پروژه
FROM build AS publish
RUN dotnet publish "MessageForAzarab.csproj" -c Release -o /app/publish /p:UseAppHost=false

# استفاده از .NET 9.0 Runtime برای اجرا
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# نصب curl برای health check
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# کپی کردن فایل‌های publish شده
COPY --from=publish /app/publish .

# ایجاد دایرکتوری برای فایل‌ها
RUN mkdir -p /app/wwwroot/uploads /app/wwwroot/keys /app/logs

# تنظیم مجوزها
RUN chmod 755 /app/wwwroot/uploads /app/wwwroot/keys /app/logs

# تنظیم متغیرهای محیطی
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# اجرای برنامه
ENTRYPOINT ["dotnet", "MessageForAzarab.dll"]