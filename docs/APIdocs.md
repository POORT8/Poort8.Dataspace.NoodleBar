# Noodle Bar API Documentation

**Status: 🚧 Draft**

## Introduction

The Noodle Bar API, part of the Poort8 Dataspace, is designed to provide a structured platform for data providers, applications, and data consumers within the Basic Data Infrastructure (BDI). This API facilitates the creation of dataspaces that follow certain principles, enabling an initial setup for dataspace initiators.

## Deploying a data service in this Noodle Bar dataspace

For developers using the .NET framework, the Poort8 iSHARE core package provides an easy start: https://github.com/POORT8/Poort8.Ishare.Core. Support for other scenarios follows soon.

For all deployed data services, the authentication and authorization process is explained on this page.

## Postman Collection

Access the Noodle Bar Postman Collection for practical examples of API requests. You can find it here: [Poort8.NoodleBar.postman_collection.json](Poort8.NoodleBar.postman_collection.json)

## Authentication

All API requests require authentication via JWT (JSON Web Tokens). There are two options to obtain a token:
1. A POST request to the `/api/ishare/connect/token` endpoint with the appropriate client assertion, based on an iSHARE compliant eIDAS certificate.
2. A POST request to the `/login` endpoint with the correct credentials from the user administration of the Noodle Bar instance.

The access token obtained through either route must be included in the `Authorization` header as `Bearer <access_token>` for each subsequent API request.

### Option 1: `/api/ishare/connect/token`

```plaintext
POST /api/ishare/connect/token
Content-Type: application/x-www-form-urlencoded

grantType=client_credentials&scope=api&clientId=<your_client_id>&clientAssertionType=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&clientAssertion=<your_client_assertion>
```

### Option 2: `/login`

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

Applying Authorizations from the Noodle Bar authorization registry - checking if an action on a resource is allowed based on the policy - can be done in three different ways.

### Option 1: `/api/enforce`
This is the most straightforward method and can only be executed by the service provider. Depending on whether an authorization exists, this endpoint returns true or false. This request requires the following query parameters:
- subject: The ID of the subject
- resource: The ID of the resource
- action: The action being performed
- useCase: Specific use case

```plaintext
GET /api/enforce?subject=<subject>&resource=<resource>&action=<action>&useCase=<useCase>
Accept: application/json
Authorization: Bearer <access_token>
```

### Option 2: `/api/explained-enforce`
This method can also only be executed by the service provider. In case of a true response, it will say which policy supports this response. This request requires the following query parameters:
- subject: The ID of the subject
- resource: The ID of the resource
- action: The action being performed
- useCase: Specific use case
- issuer: The ID of the issuer
- serviceProvider: The ID of the service provider
- type: The type of resource
- attribute: The attribute of the resource

```plaintext
GET /api/explained-enforce?subject=<subject>&resource=<resource>&action=<action>&useCase=<useCase>&issuer=<issuer>&serviceProvider=<serviceProvider>&type=<type>&attribute=<attribute>
Accept: application/json
Authorization: Bearer <access_token>
```

### Option 3: `/api/ishare/delegation`
This method can be used by both the service to whom the authorization is granted, or the service user. With a request formatted as an iSHARE delegation mask, a response is sent back which serves as signed proof of authorization. Use this to set up delegations for access to your data.

```plaintext
POST /api/ishare/delegation
Content-Type: application/json
Accept: application/json
Authorization: Bearer <access_token>

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

## Response codes

- 200 - Success
- 401 - Unauthorized: The request does not contain a valid authentication token.
- 403 - Forbidden: The user or service does not have access to the requested resource.

## Support
For further support or questions, please contact our support team at hello@poort8.nl.