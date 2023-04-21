#添加.net6基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#容器工作目录
WORKDIR /app

#指定项webapi或mvc站点的内部启动端口，默认是80
#ENV ASPNETCORE_URLS http://+:5196

#暴露docker容器端口
#EXPOSE 5196
#EXPOSE 443

#拷贝发布目录下的所有文件到容器的工作根目录
#COPY . . (这样写也可以)
COPY ./ ./

# 如果用System.SqlClient.dll访问数据库，需要加这一句
#RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf

#容器入口点
ENTRYPOINT ["dotnet", "ArsWebApiService.dll"]