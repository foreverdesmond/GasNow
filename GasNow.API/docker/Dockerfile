# 使用官方的.NET 8.0 SDK镜像作为构建环境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 首先复制项目文件进行 restore（提高缓存效率）
COPY GasNow.API/*.csproj ./GasNow.API/
COPY GasNow/*.csproj ./GasNow/

# 还原 NuGet 包
RUN dotnet restore "./GasNow.API/GasNowAPI.csproj"

# 复制所有源代码
COPY GasNow.API/ ./GasNow.API/
COPY GasNow/ ./GasNow/

# 构建项目
WORKDIR /src/GasNow.API
RUN dotnet build "GasNowAPI.csproj" -c Release -o /app/build

# 发布阶段
FROM build AS publish
RUN dotnet publish "GasNowAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 运行时阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 复制发布的文件
COPY --from=publish /app/publish .

# 创建非root用户运行应用
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

# 暴露端口
EXPOSE 6000

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:6000

# 启动应用
ENTRYPOINT ["dotnet", "GasNowAPI.dll"]
