using FastEndpoints.Swagger;
using NSwag;

namespace Poort8.Dataspace.API.Extensions;

public static class ApiDefinitionExtensions
{
    private const string Description = @"
The NoodleBar API, developed by Poort8, is an advanced dataspace solution designed to enable secure, controlled, and efficient data sharing between businesses. NoodleBar is known for its ""dataspace in a day"" capability, meaning it provides all the necessary building blocks to quickly set up and operate a dataspace. The API is inspired by the iSHARE Trust Framework and adopts the same dataspace concepts and roles, ensuring seamless integration with iSHARE-compatible systems.

# Organization Registry

The Organization Registry is a central database within the NoodleBar dataspace that stores information about all organizations (also referred to as parties) participating in the dataspace. It manages organizational identities, ensuring that all entities are properly identified and authenticated. This registry includes details such as:

-   **Identifiers**: Unique IDs for each organization.
-   **Names**: Official names of the organizations.
-   **Adherence Status**: Compliance information with dataspace standards.
-   **Roles**: The roles an organization plays within the dataspace (e.g., data provider, data consumer).
-   **Additional Properties**: Other relevant details like contact information, certificates, and services offered.

# Authorization Registry

The Authorization Registry is responsible for managing access control within the dataspace. It defines and enforces policies (also known as authorizations or permissions) that determine the rules for accessing resources. Key functions include:

-   **Policy Management**: Creation, updating, and deletion of policies that govern resource access.
-   **Access Control**: Enforcement of policies to ensure only authorized entities can perform specific actions on resources.
-   **Delegation Evidence**: Providing signed proofs of authorization when required.

# Dataspace

The Organization Registry and Authorization Registry work closely together to provide a secure and controlled environment for data sharing:

1.  **Authentication and Identification**:

    -   When a user or system (subject) attempts to access a resource, the Organization Registry authenticates and identifies the entity using stored credentials and identifiers.
    -   It ensures that only recognized and authenticated participants of the dataspace can proceed with access requests.
2.  **Authorization Enforcement**:

    -   After authentication, the Authorization Registry checks whether the authenticated subject has the necessary permissions to perform the requested action on the resource.
    -   It evaluates policies based on various factors such as the subject's role, the resource's attributes, the action requested, and the specific use case.
3.  **Access Decision**:

    -   The outcome of the authorization check determines whether access is granted or denied.
    -   If permitted, the subject can proceed with the action; if denied, the action is blocked to maintain security and compliance.

By integrating the management of organizational identities and access control policies, the Organization Registry and Authorization Registry ensure that data sharing within the NoodleBar dataspace is both secure and efficient. They maintain data sovereignty by allowing data owners to control who accesses their data and under what conditions.

# Authentication

Our API uses OAuth 2.0 Client Credentials Flow for authentication. You need to obtain an access token to access the API. Contact Poort8 for access.

Request a bearer token:
```
POST https://poort8.eu.auth0.com/oauth/token
Content-Type: application/json

{
	""client_id"": ""{{clientId}}"",
	""client_secret"": ""{{clientSecret}}"",
	""audience"": ""Poort8-Dataspace-CoreManager"",
	""grant_type"": ""client_credentials"",
	""scope"": ""delegation"",
	""requestedPermission"": ""read:policies""
}
```
Add the ```access_token``` from the token response to call the API. For example:
```
GET {{host}}/api/policies?useCase=ishare
Accept: application/json
Authorization: Bearer {{access_token}}
```
## Permissions

You must request the appropriate ```requestedPermission``` for each endpoint. The following permissions can be requested:
- ```read:resources```
- ```write:resources```
- ```delete:resources```
- ```read:policies```
- ```write:policies```
- ```delete:policies```
- ```read:or-organizations```
- ```read:ar-organizations```

";

    private const string ResourcesDescription = @"
The **Resources** endpoints are your gateway to managing the fundamental assets within your dataspace---be they datasets, APIs, or physical devices. As a developer, you can:

-   **Integrate Diverse Assets**: Seamlessly add various types of resources to your dataspace, enhancing the capabilities of your applications.
-   **Customize and Enrich**: Define and modify resource attributes to tailor them to your application's needs.
-   **Optimize Data Utilization**: Efficiently organize and access resources to improve data retrieval and application performance.

These endpoints empower you to handle resources flexibly, enabling the creation of robust and scalable solutions without the hassle of low-level management tasks.
";

    private const string ResourceGroupsDescription = @"
The **Resource Groups** endpoints enable efficient management of collections of resources, also known as services or resource types. With these endpoints, you can:

-   **Organize Logically**: Group related resources to reflect the structure of your application or business logic.
-   **Streamline Access Control**: Apply policies and permissions at the group level, simplifying security management.
-   **Enhance Scalability**: Manage large numbers of resources collectively, reducing administrative overhead.

By leveraging resource groups, you can maintain an organized dataspace and simplify the management of complex resource hierarchies.
";

    private const string PoliciesDescription = @"
The **Policies** endpoints are essential for defining and managing the rules that govern access to resources within your dataspace. They allow you to:

-   **Implement Fine-Grained Access Control**: Specify detailed permissions for who can access which resources and under what conditions.
-   **Adapt to Changing Needs**: Quickly adjust access rules in response to evolving requirements or user roles.
-   **Maintain Security Compliance**: Ensure your applications adhere to security standards and regulatory policies.

Utilizing these endpoints helps you safeguard your resources while providing the flexibility needed to support dynamic environments.
";

    private const string OrganizationRegistryDescription = @"
Accessing the **Organization Registry** endpoints lets you interact with the central database of all organizations participating in the dataspace. This interaction enables you to:

-   **Authenticate Participants**: Verify the identities of organizations to establish trust within the dataspace.
-   **Retrieve Organizational Information**: Access details such as roles, attributes, and compliance status to inform your application's logic.
-   **Facilitate Secure Collaboration**: Use organizational data to manage partnerships and data-sharing agreements securely.

This functionality is crucial for building applications that require secure and verified interactions between multiple entities.
";

    private const string AuthorizationRegistryDescription = @"
Through the **Authorization Registry Organizations** endpoints, you can:

-   **Audit Access Rights**: Retrieve information about the permissions and policies associated with organizations in the authorization registry.
-   **Monitor and Enforce Compliance**: Ensure that organizations adhere to the defined access control policies and take corrective actions if necessary.
-   **Enhance Security Oversight**: Gain insights into who has access to what, strengthening your application's security posture.

This capability allows you to maintain control over data access within the dataspace, ensuring that your applications remain secure and compliant.
";

    private const string AuthoriztionDescription = @"
The **Authorization** endpoints provide powerful mechanisms to enforce and audit access control decisions within your applications. They enable you to:

-   **Verify Permissions in Real-Time**: Check whether a user or system has the rights to perform a specific action on a resource, ensuring immediate compliance with policies.
-   **Understand Access Decisions**: Obtain detailed explanations of authorization outcomes to facilitate debugging and transparency.
-   **Manage Delegated Access**: Handle permission delegations securely, allowing for flexible access hierarchies and trust relationships.

These endpoints are vital for maintaining robust security and ensuring that only authorized entities can access sensitive resources.
";

    public static IServiceCollection AddApiDefinition(this IServiceCollection services)
    {
        services.SwaggerDocument(options =>
        {
            options.DocumentSettings = s =>
            {
                s.Title = "NoodleBar API";
                s.Version = "v1";
                s.Description = Description.Trim();

                s.AddAuth("Bearer", new()
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });
            };

            options.TagDescriptions = t =>
            {
                t["Organization Registry"] = OrganizationRegistryDescription.Trim();
                t["Authorization Registry Organizations"] = AuthorizationRegistryDescription.Trim();
                t["Policies"] = PoliciesDescription.Trim();
                t["Resources"] = ResourcesDescription.Trim();
                t["Resource Groups"] = ResourceGroupsDescription.Trim();
                t["Authorization"] = AuthoriztionDescription.Trim();
            };

            options.EnableJWTBearerAuth = false;
            options.AutoTagPathSegmentIndex = 0;
            options.ExcludeNonFastEndpoints = true;
            options.EndpointFilter = ep => !(ep.EndpointTags?.Contains("ExcludeFromSwagger") ?? false);
        });

        return services;
    }
}
