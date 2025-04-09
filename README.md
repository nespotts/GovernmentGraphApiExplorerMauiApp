# README
## Description
This is a *Postman Clone* dedicated to reaching government cloud endpoints of the Micrsoft Graph API because Microsoft's Graph Explorer does not work with the government clouds.  It is meant as a development tool for working with your organizations specific government cloud data.  
It was developed in Visual Studio using MAUI Hybrid (Blazor) for .NET 8.  


## Build From Source Windows
1. Download the Zip file of repo
2. Extract the contents
3. open a cmd prompt in the solution directory
4. Publish using the following command prompt: ```dotnet publish -f net8.0-windows10.0.19041.0 -p:RuntimeIdentifierOverride=win10-x64 -c Release -p:WindowsPackageType=None -p:SelfContained=true -p:IncludeAllContentForSelfExtract=true```
5. Navigate to the publish directory within the solution: ```./GovGraphExplorerMaui/bin/Release/net8.0-windows10.0.19041.0/win10-x64/publish```
6. Double-click on ```GovGraphExplorerMaui.exe``` to run and install the Program.
*Note: It will not work until you enter your app registration details.*

## OR Download and Install from Latest Release
1. Click on Release in Github in the Right-hand pane
2. Find the latest version and download the gov_graph_explorer_vX.X.X.zip zip file
3. Extract the contents
4. Navigate within the publish directory and double-click on ```GovGraphExplorerMaui.exe``` to run and install the Program.

## Setting Up Authentication within your Tenant
To get an access token to access your tenant's data, you need to create a web app registration in Entra Id and assign any graph API permissions to be used in the tool.  

### Create the Web App Registration
1. Login to Azure Portal
2. Search for and go to Entra Id
3. Click on App Registrations in the Side Panel
4. Click New Registration
5. Enter a Name for the Registration
6. Under Redirect URI, set the platform to *Web* and the URI to ```https://localhost:7165/signin-oidc```
7. Click Register
8. Go to the registration once it is deployed.
9. Go to the Overview Tab.
10. Note the Application (client) ID and Directory (tenant) ID
11. Go To Manage > Certificates & Secrets, create a Client Secret and note the value
12. Go To Manage > API permissions
13. Click Add a permission
14. Select Microsoft Graph
15. Select Delegated or Application permissions depending upon your needs and select the permissions that you want the explorer to have
16. You can add these at any time and most will require admin consent for functionality.

### Add the IDs and Secrets to appsettings.json
1. in the publish directory referenced above, find the appsettings.json file and open it in an editor
2. Edit the AzureAd section values with the IDs and secret saved above (TenantId, ClientId, and ClientSecret)

## Run the App
1. Double-click on ```GovGraphExplorerMaui.exe``` in the publish directory to run the program.
2. It opens up with the ```/users?$top=999``` endpoint for starters.  This requires at least the User.ReadBasic.All permission to be allowed.
3. You can also add additional example queries in appsettings.json using the following json format that will show up in the Example Queries Tab in the app for quick reference:
   ```
   {
      "Nickname": "Send Email",
      "HttpVerb": "POST",
      "Query": "/users/{sending user id}/sendmail",
      "HeaderParameters": {
        "consistencyLevel": "eventual",
        "Content-Type": "application/json"
      },
      "RequestBody": "{\n  \"message\": {\n    \"subject\": \"Test Subject\",\n    \"from\": {\n      \"emailAddress\": {\n        \"address\": \"from@email.com\"\n      }\n    },\n    \"body\": {\n      \"contentType\": \"text\",\n      \"content\": \"test message\"\n    },\n    \"toRecipients\": [\n      {\n        \"emailAddress\": {\n          \"address\": \"test@email.com\"\n        }\n      }\n    ],\n    \"ccRecipients\": [\n      {\n        \"emailAddress\": {\n          \"address\": \"test@email.com\"\n        }\n      }\n    ]\n  },\n  \"saveToSentItems\": \"true\"\n}"
    }
   ```

![Screenshot 2025-04-09 132942](https://github.com/user-attachments/assets/128e00db-d51c-4ff4-b04c-d3925b155771)


