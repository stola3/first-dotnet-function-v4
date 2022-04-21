# AdminPortal Function API

First .net Azure Function (v4). Usage with care!
## Developoment

* Install Azure Fucntion Core Tools
* Add `src/local.settings.json`
    ```
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "FunPS-CreateUser": "<replace-URL>"
      }
    }
    ```

* Run
    ```
    cd src
    func start
    ```
* Example POST `http://localhost:7071/api/users` with body
    ```
    {
    "givenName" : "Mairo",
    "surename" : "Super",
    "SamAccountName" : "super.mario",
    "userPrincipalName" : "super.mario",
    "EmailAddress" : "super.mario@localhost",
    "Path" : "DN-Path"
    }
    ```