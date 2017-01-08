FROM microsoft/aspnetcore:1.1.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/Debug/netcoreapp1.1/publish} .
ENTRYPOINT ["dotnet", "CoreSample.dll"]
