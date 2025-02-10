
# FinPriceFeed App

- It consists of a **REST API** and a **WebSocket**.  
- REST API includes Controller just to send events to WebSocket but cannot receive any broadcast.
- Implementated integration with 2 External Providers.  
  -  **TwelveData**: RST API & WebSocket Integration.
  -  **Tiingo**: Only REST API (*WebSocket not implemented yet*).
  -  **ApiKey** should be placed in **appsettings.json**.
  -  **Default selected**: **"TwelveData"**. Can be changed in appsettings.json


**Swagger for REST API**:
  [http://localhost:5174/swagger](http://localhost:5174/swagger)  
**Plain Website to demo WebSocket Functionality**:
   [http://localhost:5174/websocket/index.html](http://localhost:5174/websocket/index.html)



## How to Build

### 1. Install .NET SDK
   - This project is built on ASP.NET Core 8.
   - Download and install the .NET SDK.

### 2. Clone the Repository  
   Open a terminal in VS Code and clone your repository:
   ```bash
   git clone https://github.com/Vortakis/AmegaTask.git
   ```

### 3. Restore Dependencies & Build
   In the terminal, navigate to the project folder and run:
   ```bash
   dotnet restore
   dotnet build
   ```
   
---

## Debugging Setup in VS Code

### 1. Create launch.json Configuration
Create the following `launch.json` configuration in the `.vscode` folder:
```json
{
    "version": "0.2.0",
    "configurations": [
      {
        "name": ".NET Core Launch (web)",
        "type": "coreclr",
        "request": "launch",
        "preLaunchTask": "build",
        "program": "${workspaceFolder}/bin/Debug/net8.0/FinPriceFeed.dll", 
        "args": [],
        "cwd": "${workspaceFolder}",
        "stopAtEntry": false,
        "serverReadyAction": {
          "action": "openExternally",
          "pattern": "http[s]?:\/\/localhost:\d+",
          "uriFormat": "http://localhost:5174/swagger"
        },
        "env": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        },
        "sourceFileMap": {
          "/Views": "${workspaceFolder}/Views"
        }
      }
    ]
}
```

### 2. Create tasks.json Configuration
Create the following `tasks.json` configuration in the `.vscode` folder to automate the build task:
```json
{
    "version": "2.0.0",
    "tasks": [
      {
        "label": "build",
        "type": "shell",
        "command": "dotnet build",
        "group": {
          "kind": "build",
          "isDefault": true
        },
        "problemMatcher": "$msCompile"
      }
    ]
}
```

---

## Running and Debugging

### 1. Run and Debug
- Press **F5** or go to **Run > Start Debugging** to run and debug the project.
- Your browser will open automatically to [http://localhost:5174/swagger](http://localhost:5174/swagger)

### 2. WebSocket Testing
- Once the project is running, you can visit [http://localhost:5174/websocket/index.html](http://localhost:5174/websocket/index.html) to test the WebSocket functionality.
