UbuntuDashboard
A web-based dashboard for monitoring and managing an Ubuntu server, built with Blazor and .NET 10.

UbuntuDashboard is a personal project focused on bringing useful server information and common service-management actions into one web interface.
Features
- System monitoring
  - View RAM usage.
  - View server uptime.
  - Monitor CPU load per core.
- System services
  - View the status of system services.
  - Start and stop services.
- Configured service ports
  - Open services exposed on configured ports.
Tech stack
- C#
- .NET 10
- Blazor
- ASP.NET Core
- Ubuntu Linux
- SSH / Linux service and resource-management tools
The exact implementation and deployment requirements depend on the current project configuration.

Getting started
Prerequisites
- .NET 10 SDK
- An Ubuntu server or environment for the server-side functionality
- Git
Run locally
1. Clone the repository:
   git clone https://github.com/dzemiol/UbuntuDashboard.git
   cd UbuntuDashboard
2. Open the solution or project in Visual Studio, or use the .NET CLI.
3. Restore dependencies:
   dotnet restore
4. Run the project:
   dotnet run
5. Open the local URL shown in the terminal.
Note: Server monitoring and service-management features may require Ubuntu-specific configuration and appropriate permissions. Review the project configuration before exposing the application to a network.

Configuration and security
Before deploying the application:
- Configure the target Ubuntu environment and any required connection details.
- Grant only the minimum permissions needed for monitoring and service operations.
- Do not commit passwords, private keys, tokens, or other secrets to source control.
- Restrict network access and use an appropriate authentication and HTTPS setup for your environment.


Project status
In development. Features and configuration may change as the project evolves.
Repository
github.com/dzemiol/UbuntuDashboard
