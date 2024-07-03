# Chapter 5: Tech Stack

NoodleBar is built on a robust and scalable .NET tech stack deployed on Azure. This choice caters to the needs of current partners, stakeholders, and customers, leveraging Azure's extensive capabilities and the versatility of the .NET ecosystem.

Stage 1 is aimed towards easy deployment and working prototypes. The OAuth variant can be used in production environments.

### 5.1 Infrastructure as Code with Bicep

To streamline deployment and configuration, and to minimize errors, NoodleBar uses Bicep for infrastructure as code. Bicep is a domain-specific language (DSL) designed for declaratively deploying Azure resources. It simplifies infrastructure management by providing a more readable and modular approach compared to traditional JSON templates.

#### Key Benefits of Bicep:

- **Declarative Syntax**: Bicep's syntax is concise and easy to read, making infrastructure definitions clear and manageable.
- **Modularization**: Supports the creation of reusable modules, enabling consistent and efficient infrastructure deployment.
- **Seamless Azure Integration**: Bicep integrates directly with Azure Resource Manager (ARM), ensuring smooth deployment and management of Azure resources.

### 5.2 Deployment and Authentication Options

#### Stage 1: Local Identity Server

**Requirements**:
- NoodleBar fork on GitHub
- Azure Subscription

Stage 1 involves deploying NoodleBar using the Local Identity Server. This setup provides the essential components to get started quickly, utilizing ASP.NET Core Identity to handle authentication and issue bearer tokens. These tokens are not OAuth compatible, but they provide a straightforward and secure method for managing user authentication. The self-hosted variant of Stage 1 is designed for ease of deployment, requiring no prerequisites other than forking the code, building it, and deploying it to Azure. This variant supports two database options: SQLite and SQL Server, with additional databases planned based on demand.

#### Stage 2: OAuth Server Integration

**Requirements**:
- OAuth server, for example, Auth0

In Stage 2, an OAuth-compatible authorization server is required. Examples include Keycloak, IdentityServer, or any Identity-as-a-Service (IaaS) solution. This guide will explain how Auth0 can be configured to act as the dataspace authentication server. NoodleBar can integrate with these services using standard middleware, which simplifies the setup and enhances security and interoperability.

#### Stage 3: iSHARE Compliance

**Requirements**:
- iSHARE Satellite
- eIDAS Certificates
- iSHARE Conformance Test

For use cases that demand the highest standards of security and governance, iSHARE can be utilized. This stage involves replacing the NoodleBar Organization Register with the iSHARE Satellite, which uses X.509 certificates for robust authentication. Deploying NoodleBar using iSHARE ensures that all data exchanges are conducted with the highest level of trust and security. The NoodleBar Authorization Register can be configured to be fully iSHARE compliant. However, it's important to note that this stage has proven to be complex, costly, and time-consuming. For production environments, eIDAS certificates must be purchased by each party in the dataspace, ensuring compliance with stringent security and governance standards.

### 5.3 Technologies and Tools Used

**Build Using**:
- ASP.NET
- Blazor
- Entity Framework
- Microsoft Fluent UI Blazor components
- OpenTelemetry
- Casbin.NET
- FastEndpoints
- Poort8.Ishare.Core
- xUnit
- FluentAssertions
- Snapshooter

**Deployed On**:
- Azure App Service
- Azure SQL Server
- Application Insights

### 5.4 Future Deployment Options

Deployment using Docker containers or .NET Aspire to target other cloud providers is also on our roadmap. This will provide flexibility and enable NoodleBar to be deployed in diverse cloud environments, catering to a broader range of organizational needs.

By leveraging these technologies and deployment strategies, NoodleBar ensures a flexible, secure, and scalable solution for creating and managing dataspaces.
