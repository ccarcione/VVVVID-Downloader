# VVVVid-download

Angular + .Net Core project for download video from VVVVID (non-paid content).

## Disclaimer!!!

This project was created for _educational purposes only_. I do not take responsibility for use for illegal purposes.
Do not use the software to break copyright rules.

The software is released under the WTFPL license.

If you like my work follow my git profiles :)

### How to start the project?

- **angular** client app (run command into _angular-client_ folder:

> npm install
>
> ng serve

- **.net core** api:

> dotnet restore
>
> dotnet build
>
> dotnet run

Alternatively you can use Visual Studio 2019

### How to use the project with docker?

- **testdev**: start projects inside docker containers (use _docker compose_ command. For linux edit with _docker-compose_).
  1. it is necessary to execute the project package restore commands (see above)
  2. run script from root folder --> scripts\run-testdev.ps1
- **prod**: creates two docker images ready for production. If you want to use them locally, or without the main nginx, be sure to open the _angular-client_ container ports: you can do this by editing the _docker\docker-compose.prod.yml_ file.