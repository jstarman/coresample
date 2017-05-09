FROM microsoft/aspnetcore
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/Debug/netcoreapp1.1/publish} .
ENTRYPOINT ["dotnet", "CoreSample.dll"]
