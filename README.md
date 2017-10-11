# CurrencyTrackerServer

Run `dotnet restore` in project root directory or launch Visual Studio to download packages

In directory `/CurrencyTrackerServer.Web`

Restore angular packages with `npm install`

To start server:

`dotnet run`

or launch

`watch.dotnet.bat`

To start angular app:

`ng serve --proxy-config proxy.config.json`

or launch

`watch.angular.bat`

`proxy.config.json` is used during development to relay api requests from default angular port 4200 to port 5000 of the ASP.NET Core server

Navigate to [localhost:4200](http://localhost:4200)
