# Poort8 Dataspace NoodleBar

## Overview

NoodleBar, developed by Poort8, is a cutting-edge dataspace solution designed to facilitate secure, controlled, and efficient data sharing among businesses. Known for its "dataspace in a day" capability, NoodleBar provides all the necessary building blocks to get a dataspace up and running quickly. Inspired by the iSHARE Trust Framework, NoodleBar adopts the same dataspace concepts and roles, ensuring seamless integration with iSHARE-compliant systems.

## Table of Contents

1. [Introduction](docs/01%20-%20Introduction.md)
2. [Dataspace Concepts](docs/02%20-%20Dataspace%20Concepts.md)
3. [Customer Journeys](docs/03%20-%20Customer%20Journeys.md)
4. [NoodleBar Implementation Stages](docs/04%20-%20NoodleBar%20Implementation%20Stages.md)
5. [Tech Stack](docs/05%20-%20Tech%20Stack.md)
6. [Deployment Using a Local Identity Server](docs/06%20-%20Deployment%20Using%20a%20Local%20Identity%20Server.md)
7. [Deployment Using OAuth Server](docs/07%20-%20Deployment%20Using%20OAuth%20Server.md)
8. [Deployment Using iSHARE](docs/08%20-%20Deployment%20Using%20iSHARE.md)
9. [Database Migrations](docs/09%20-%20Database%20Migrations.md)
10. [NoodleBar Showcase](docs/10%20-%20NoodleBar%20Showcase.md)

## Context

The project is under the Basis Data Infrastructuur (BDI) umbrella, pending its ongoing development.

## Objective

To facilitate setting up dataspaces that follow certain principles, serving as an initial platform for data providers, apps, and data consumers.

## Roles

- **Data Providers**: Organizations that either offer a data source with raw data or an app with processed data. In all cases, access conditions are set by the data owner.
- **App Providers**: Organizations that act as intermediaries, adding value to raw data. They act as a Data Consumer on behalf of their end users, and as a Data Provider for their end users.
- **Data Consumers**: Organizations that use data via Service Providers or directly.
- **Dataspace Initiators**: Organizations that setup and manage the dataspace.

## Principles

1. Data owners (issuers) can issue access to their data, even if through federated apps.
2. Data stays at its source unless caching or staging is essential.
3. Data consumers choose their identity providers.

## Customer Journeys

The wiki describes the following [Customer Journeys](docs/03%20-%20Customer%20Journeys.md) in more detail:

- Initiating Dataspace Core
- Onboarding Data Sources
- Onboarding Data Owners and Consumers
- Data Sources Becoming Independent
- Adding providers and apps

The first 3 journeys comprise the launch of a first (prototype) of a dataspace. Journeys 4 and 5 allow data sources and Service Providers to become independent contributors to the dataspace.

## Functionalities

The Dataspace Core provides services for the organization registry, the organization onboarding process, the dataspace manager and (optionally) an authorization registry. With the dataspace manager the dataspace standards can be managed, such as the requirements for authentication and onboarding, and the definition of a dataspace data model.
Secondly (and optionally), the dataspace initiator can provide Dataspace Adapters to Data Providers, with services to support them with mapping to the dataspace data model, and Identification, Authentication and Authorisation (IAA) according to the Dataspace standards. Dataspace Adapters are expected to be made redundant as Data Providers create independent solutions for this.
Thirdly (and optionally), the dataspace initiator may choose to launch the dataspace with a prototype app, using the Dataspace Prototype services for logic, IAA, and multiple front-end channels for the end user. Such a Dataspace Prototype app can be removed when additional apps are added to the dataspace.
See the [architectural outline](docs/02%20-%20Dataspace%20Concepts.md) of these functions for more detail.

## Challenges

To make the software modular and customizable so as to allow stakeholders to evolve independently but in compliance with the principles and regulations of the dataspace.

## Tech Stack

For the current partners, stakeholders, and customers, the .NET tech stack deployed on Azure was chosen for NoodleBar. 

- **Built With**:
  - .NET 8
  - Casbin.NET
  - xUnit
  - Entity Framework
  - Blazor
  - Microsoft Fluent UI Blazor components
  - OpenTelemetry
  - FastEndpoints
  - Poort8.Ishare.Core
  - Snapshooter

- **Deployed On**:
  - Azure App Service
  - SQL Server
  - Application Insights

### Support for New Apps

New apps can utilize the dataspace data model, authorization registry, and organization registry but must handle authentication themselves.

### Disambiguation

In the developing realm of federated dataspace schemes, different - sometimes ambiguous - terminology is used. If possible, the Poort8 dataspace follows [schema.org](https://schema.org) for terminology and data models. For authorizations, Poort8 uses terminology in line with [casbin.org](https://casbin.org). In the table below we provide a non-exhaustive overview of how typical terminology is mapped to our Dataspace software.

| Poort8.Dataspace        | equivalent to ...                                   |
| ----------------------- | --------------------------------------------------- |
| organisation            | party                                               |
| issuer                  | entitledParty, data owner                           |
| subject                 | accessSubject, data consumer, data service consumer |
| provider                | service provider, data service provider             |
| federated app           | application of a service provider                   |
| resource group, service | resourceType                                        |
| resources               | attributes                                          |
| policy                  | authorization, permission                           |

## Getting Started

### Prerequisites

Install .NET 8.

### Installation

1. Clone the repo:
   ```sh
   git clone https://github.com/POORT8/Poort8.Dataspace.NoodleBar.git
2. Build and start Poort8.Dataspace.CoreManager from your favourite IDE or CLI. The CoreManager web app including an instance of Poort8.Dataspace.AuthorizationRegistry and Poort8.Dataspace.OrganizationRegistry will be started.