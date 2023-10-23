#Start with base .NET SDK Image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

#Set the working directory inside the container
WORKDIR /app

#Copy the project files and restore the dependencies
COPY *.csproj ./
RUN dotnet restore

#Copy the remaining files and build the app
COPY . ./
RUN dotnet publish -c Release -o out


#Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime-env
WORKDIR /app
COPY --from=build-env /app/out .

#Set entry point
ENTRYPOINT ["dotnet", "Instagram.dll"]
