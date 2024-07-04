# 6: Deployment Using a Local Identity Server

The Stage 1 deployment of NoodleBar involves setting up the core components necessary for a functional dataspace prototype. This stage focuses on deploying the Local Identity Server, which handles authentication and authorization, and the NoodleBar Web App for managing organizations and policies.

### 6.1 Prerequisites

Before you begin the deployment, ensure that you have the following prerequisites in place:

- **NoodleBar fork on GitHub**: Fork the Poort8.Dataspace.NoodleBar repository to your GitHub account.
- **Azure Subscription**: Ensure you have an active Azure subscription to deploy the necessary resources.

### 6.2 Setting Up the Environment

1. **Fork the NoodleBar Repository**:
   Fork the Poort8.Dataspace.NoodleBar repository to your GitHub account:
   ```sh
   git clone https://github.com/YourAccount/Poort8.Dataspace.NoodleBar.git
   cd Poort8.Dataspace.NoodleBar
   ```

2. **Modify Bicep Files for Deployment**:
   The repository includes Bicep files for deployment. Modify these files where applicable to fit your specific environment and requirements. The main Bicep files are:
   - `main.bicep`: This is the main entry point for the deployment and references other Bicep modules.
   - `dataspaceAlertModule.bicep`: Deploys alert rules for monitoring the resources.
   - `dataspaceWorkspaceModule.bicep`: Sets up the Azure Log Analytics workspace.
   - `customDomainModule.bicep`: Configures custom domain settings.
   - `resourceGroupModule.bicep`: Creates the resource group for all resources.
   - `sslBindingModule.bicep`: Sets up SSL bindings for secure connections.
   - `appServiceModule.bicep`: Deploys the Azure App Service to host the NoodleBar application.

3. **Set Configuration and Secrets**:
   Configure environment variables and secrets needed for the deployment. Secrets must be configured in the GitHub repository settings under "Settings" -> "Secrets and variables" -> "Actions". Add the following secrets:
   - `AZURE_CLIENT_ID`
   - `AZURE_TENANT_ID`
   - `AZURE_SUBSCRIPTION_ID`
   - Any other secrets required for your specific setup, such as database connection strings.

4. **Set Up GitHub Workflow to Deploy NoodleBar**:
   Configure a GitHub workflow to automate the deployment of NoodleBar. Ensure that the workflow uses the modified Bicep files and secrets for deployment.

   Here's an example of a GitHub Actions workflow file (`.github/workflows/deploy.yml`):

   ```yaml
   name: Deploy NoodleBar

   on:
     workflow_dispatch:
     push:
       branches:
         - master
       paths:
         - 'Poort8.Dataspace.CoreManager/**'
         - 'Poort8.Dataspace.AuthorizationRegistry/**'
         - 'Poort8.Dataspace.OrganizationRegistry/**'
         - 'Poort8.Dataspace.API/**'

   permissions:
     id-token: write
     contents: read

   jobs:
     deploy-preview:
       runs-on: ubuntu-latest
       env:
         resourceGroupName: 'NoodleBar-Preview'
         resourceGroupLocation: 'westeurope'

       steps:
         - uses: actions/checkout@v4
      
         - name: Setup .NET
           uses: actions/setup-dotnet@v4
           with:
             dotnet-version: 8.x
             
         - name: Azure Login
           uses: azure/login@v2
           with:
             client-id: ${{ secrets.AZURE_CLIENT_ID }}
             tenant-id: ${{ secrets.AZURE_TENANT_ID }}
             subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
             allow-no-subscriptions: true
             
         - name: Publish
           run: dotnet publish ./Poort8.Dataspace.CoreManager/Poort8.Dataspace.CoreManager.csproj -c Release -o publish/app

         - name: Deploy resource group
           uses: azure/arm-deploy@v2
           with:
             scope: subscription
             subscriptionId: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
             region: ${{ env.resourceGroupLocation }}
             template: ./Poort8.Dataspace.CoreManager/deploy/resourceGroupModule.bicep
             parameters: 'resourceGroupName=${{ env.resourceGroupName }} resourceGroupLocation=${{ env.resourceGroupLocation }}'

         - name: Deploy preview bicep
           uses: azure/arm-deploy@v2
           with:
             subscriptionId: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
             resourceGroupName: ${{ env.resourceGroupName }}
             template: ./Poort8.Dataspace.CoreManager/deploy/main.bicep
             parameters: 'environment=preview'
             
         - name: Deploy app to preview
           uses: azure/webapps-deploy@v3
           with:
             app-name: NoodleBar-Preview
             package: publish/app
   ```

### 6.3 Deploying the NoodleBar Application

All components of NoodleBar, including the Organization Register, Authorization Register, and the NoodleBar Web App, are deployed as a single application. Use the GitHub Actions workflow to deploy these components using the modified Bicep files.

1. **Run the Deployment Workflow**:
   Trigger the GitHub Actions workflow to deploy the NoodleBar application. This workflow will use the Azure CLI and ARM templates to set up the necessary resources and deploy the application.

2. **Verify the Deployment**:
   Ensure the deployment is successful by accessing the service endpoint:
   ```sh
   curl http://<your-app-url>/health
   ```

### 6.4 Configuring the NoodleBar System

1. **Register as a New User**:
   Go to `http://<your-app-url>` to register as a new user. 

2. **Add Organizations**:
   Use the NoodleBar web app to add new organizations to the Organization Register.

3. **Define Authorization Policies**:
   Set up authorization policies for the organizations using the Authorization Register.

4. **Integrate Data Providers**:
   Modify existing APIs to use the enforce API of the Authorization Register to control data access.

By following these steps, you can successfully deploy NoodleBar using the Local Identity Server, setting up a fully functional dataspace prototype quickly and efficiently.