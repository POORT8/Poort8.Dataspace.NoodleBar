# Noodlebar API Documentation

## Introduction

The Noodlebar API, part of the Poort8 Dataspace, is designed to provide a structured platform for data providers, applications, and data consumers within the Basic Data Infrastructure (BDI). This API facilitates the creation of dataspaces that follow certain principles, enabling an initial setup for dataspace initiators.

## Deploying a data service in this Noodlebar dataspace

For developers using a .NET framework, the Poort8 iSHARE core package provides an easy start: https://github.com/POORT8/Poort8.Ishare.Core. Support for other scenarios follows soon.

For all deployed data services, authentication and authorization process is explained on this page.

## Postman Collection

Access the Noodlebar Postman Collection for practical examples of API requests. Find it here: [Poort8.Noodlebar.postman_collection.json](Poort8.Noodlebar.postman_collection.json)

## Authentication

All API requests require authentication via JWT (JSON Web Tokens). There are two options to obtain a token:
1. A POST request to the `/api/ishare/connect/token` endpoint with the appropriate client_assertion, based on an iSHARE compliant eIDAS certificate.
2. A POST request to the `/login` endpoint with the correct credentials from the user administration of the Noodlebar instance.

The access_token obtained through either route must be included in the `Authorization` header as `Bearer <access_token>` for each subsequent API request.

### Option 1. `/api/ishare/connect/token`

```plaintext
POST /api/ishare/connect/token
Content-Type: application/x-www-form-urlencoded

grantType=client_credentials&scope=api&clientId=<your_client_id>&clientAssertionType=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&clientAssertion=<your_client_assertion>
```

### Option 2. `/login`

```plaintext
POST /login
Content-Type: application/json

{
  "username": "<your_username>",
  "password": "<your_password>",
  "useCookies": true,
  "useSessionCookies": true
}
```

## Authorization

Applying Authorizations from the Noodlebar authorization registry can be done in three ways:

Applying Authorizations from the Noodlebar authorization register can be accomplished in three ways:
1. A GET request to `/enforce`. This is the most straightforward method and can only be executed by the service provider. Depending on whether an authorization exists, this endpoint returns true or false.
2. A GET request to `/explained-enforce`. This method can also only be executed by the service provider, and in case of a true response, it also provides which policy supports this true response.
3. A POST request to `/ishare/delegation`. This method can be used by both the service to whom the authorization is granted or the service user. With a request in the form of an iSHARE delegation mask, a response is sent back which serves as a signed proof of authorization.

### Option 1. `/api/enforce`

```plaintext
GET /api/enforce
Check if an action on a resource is allowed based on the policy.
Parameters:
- subject: ID of the subject
- resource: ID of the resource
- action: The action being performed
- useCase: Specific use case
```

### Option 2. Explained-enforce

```plaintext
GET /api/explained-enforce?subject=<string>&resource=<string>&action=<string>&useCase=<string>&issuer=<string>&serviceProvider=<string>&type=<string>&attribute=<string>
Accept: application/json

This method also can only be executed by the service provider, and in case of a true response, it also provides which policy supports this true response.
```

### Option 3. `/api/ishare/delegation`

```plaintext
POST /api/ishare/delegation
Use this to set up delegations for access to your data.
Request Body:
{
  "delegationRequest": {
    "policyIssuer": "string",
    "target": {
      "accessSubject": "string"
    },
    "policySets": [
      {
        "policies": [
          {
            "target": {
              "resource": {
                "type": "string",
                "identifiers": ["string"],
                "attributes": ["string"]
              },
              "actions": ["string"],
              "environment": {
                "serviceProviders": ["string"]
              }
            },
            "rules": [
              {
                "effect": "string"
              }
            ]
          }
        ]
      }
    ]
  }
}
```

## Error Codes

- 200 - Success
- 401 - Unauthorized: The request does not contain a valid authentication token.
- 403 - Forbidden: The user or service does not have access to the requested resource.

## Support

For further support or questions, please contact our support team at hello@poort8.nl.
