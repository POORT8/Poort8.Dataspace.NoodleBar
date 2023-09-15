# Project Description

1. **Context**:  
The project is under the Basis Data Infrastructuur (BDI) umbrella, pending it's ongoing development.

2. **Objective**:  
To facilitate setting up data spaces that follow certain principles, serving as an initial platform for data providers, apps, and data consumers.

3. **Roles**:
  - Data Providers: either a data source offering raw data or an app offering processed data. In all cases, access conditions are set by the data owner.
  - Service Providers: Act as intermediaries, adding value to raw data. They act as a Data Consumers on behalf of their end users, and as a Data Provider for their end users. An equivalent term for a service provider would be a 'federated app', but this is not so common in Dataspace terminologie.
  - Data Consumers: Use data via Service Providers or directly.
  - Dataspace initiators: setup and manage the dataspace.

4. **Principles**:
    1. Data owners control access to their data, even if through service providers.
    2. Data stays at its source unless caching or staging is essential.
    3. Data consumers choose their identity providers.

5. **Customer Journeys**:  
The wiki describes the following [Customer Journeys](/Poort8.Dataspace.Wiki/customerjourneys.md) in more detail:
 - Initiating Dataspace Core
 - Onboarding Data Sources
 - Onboarding Data Owners and Consumers 
 - Data Sources Becoming Independent
 - Adding Service Providers
The first 3 journeys comprise the launch of a first (prototype) of a data space. Journeys 4 and 5 allow data sources and Service Providers to become independent contributors to the data space.

6. **Functionalities**:  
The Dataspace Core provides services for the party registry, the party onboarding process, the data space manager  and  (optionally) an authorization registry. With the dataspace manager the data space standards can be managed, such as the requirements for authentication and onboarding, and the definition of a dataspace data model.
Secondly (and optionally), the Dataspace initiator can provide Dataspace Adapters to Data Providers, with services to support them with mapping to the dataspace data model, and Identification, Authentication and Authorisation (IAA) according to the Dataspace standards. Dataspace Adapters are expected to be made redundant as Data Providers create independent solutions for this.
Thirdly (and optionally), the Dataspace initiator may choose to launch the dataspace with a prototype app, using the Dataspace Prototype services for logic, IAA, and multiple front-end channels for the end user. Such a Dataspace prototype app can be removed when additional apps are added to the dataspace.
See the [architectural](/Poort8.Dataspace.Wiki/architecture.md) outline of these functions for more detail.

7. **Challenges**:  
To make the software modular and customizable so as to allow stakeholders to evolve independently but in compliance with the principles and regulations of the dataspace.

8. **Tech Stack**:
    - No strong preferences for technologies.
    - Independent data sources must align with the dataspace data models and Identification, Authentication and Authorisation requirements.

9. **Support for New Apps**:
    - New apps can utilize the dataspace data model, authorization registry, and party registry but must handle authentication themselves.
