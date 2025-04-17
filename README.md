# MicrosoftGraphFunctionApp

This project is an Azure Function app that retrieves user profiles from Microsoft Graph API. It includes a single HTTP-triggered function, `GetUserProfile`, which automatically fetches user profile information based on the logged in user.

## Features

- Fetches user profiles from Microsoft Graph API.
- Supports both local development and Azure deployment using appropriate credentials.
- Logs execution details and errors for better observability.

## Prerequisites

- .NET SDK installed.
- Azure CLI installed for local development.
- Azure subscription for deployment.
- Microsoft Graph API permissions configured.

## How to Run Locally

1. Clone the repository:
    ```bash
    git clone <repository-url>
    cd MicrosoftGraphFunctionApp
    ```

2. Install dependencies:

        dotnet restore

3. Set up local environment variables in `local.settings.json`:

    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
      }
    }
    ```

4. Run the Azure Function locally:

        func start

5. Test the function by sending a request to:

        http://localhost:7071/api/GetUserProfile

## Deployment

1. Log in to Azure:

        az login

2. Deploy the function app infrastructure:

      Run the [script](https://learn.microsoft.com/en-us/azure/azure-functions/scripts/functions-cli-create-function-app-connect-to-storage-account#run-the-script) to quickly deploy the function app to Azure.

3. Enable system-assigned identity for the Azure Function App:

    ```bash
    az functionapp identity assign --name <FunctionAppName> --resource-group <ResourceGroupName>
    ```

4. Configure Microsoft Graph permissions for the system-assigned identity:

    Follow the tutorial [Secure access to Microsoft Graph in Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/scenario-secure-app-access-microsoft-graph-as-app?tabs=azure-cli) to grant the required permissions (e.g., `User.Read.All`) to the system-assigned identity.

5. Configure authentication for the Azure Function App:

    Follow the tutorial [Configure authentication in Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad?tabs=workforce-configuration) to enable Azure Active Directory (AAD) authentication for the Function App.

6. Deploy the function app:

        func azure functionapp publish <FunctionAppName>

## Usage
HTTP Request

* Method: `GET` or `POST`
* Headers: 
  * `X-MS-CLIENT-PRINCIPAL-ID`: The client principal ID of the user, and this header is automatically provided if running the Function app in Azure
* Response:
  * `200 OK`: Returns the user profile in JSON format.
  * `500 Internal Server Error`: Returns an error message if the profile cannot be retrieved.

## License
This project is licensed under the MIT License. See the LICENSE file for details.
