#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy only the csproj file first to restore
COPY ["ManyToManyStudentCourse/ManyToManyStudentCourse.csproj", "ManyToManyStudentCourse/"]
RUN dotnet restore "ManyToManyStudentCourse/ManyToManyStudentCourse.csproj"

# Copy only the content of ManyToManyStudentCourse directory
COPY ManyToManyStudentCourse/ ./ManyToManyStudentCourse/

WORKDIR "/src/ManyToManyStudentCourse"
RUN dotnet build "ManyToManyStudentCourse.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ManyToManyStudentCourse.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManyToManyStudentCourse.dll"]