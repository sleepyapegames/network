build:
    dotnet build -c Debug

server:
    ./test/Netcode.Server/bin/Debug/net6.0/Netcode.Server

client:
    ./test/Netcode.Client/bin/Debug/net6.0/Netcode.Client
