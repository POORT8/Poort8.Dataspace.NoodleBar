# Project Description

1. **Context**:  
The project is under the Basis Data Infrastructuur (BDI) umbrella, pending it's ongoing development.

2. **Objective**:  
To facilitate setting up data spaces that follow certain principles, serving as an initial platform for data providers, apps, and data consumers.

3. **Roles**:
  - Data Providers: either a data source offering raw data or an app offering processed data. In all cases, access conditions are set by the data owner.
  - Apps: Act as intermediaries, adding value to raw data. They act as data consumers on behalf of their end user, and as Data Provider for their end users.
  - Data Consumers: Use data via apps or directly.
  - Dataspace initiators: setup and manage the dataspace.

4. **Principles**:
    1. Data owners control access to their data.
    2. Data stays at its source unless caching or staging is essential.
    3. Data consumers choose their identity providers.

5. **Customer Journeys**:  
The wiki describes the following [Customer Journeys](/Poort8.Dataspace.Wiki/customerjourneys.md) in more detail:
 - Initiating Dataspace Core
 - Onboarding Data Sources
 - Launching Prototype Apps
 - Data Sources Becoming Independent
 - Adding Extra Apps  
The first 3 journeys comprise the launch of a first (prototype) of a data space. Journeys 4 and 5 allow data sources and Apps to become independent contributors to the data space.

6. **Functionalities**:  
The Dataspace Core provides services for the authorization registry, the party registry and the definition of the dataspace data model.  
Initially (and optionally), the Dataspace provides additional services to support data providers with caching, mapping to the dataspace data model, and Identification, Authentication and Authorisation (IAA). These are expected to be made redundant as data providers create independent solutions. 
Finally (and optionally), the Dataspace provides services that facilitate a prototype app: logic, IAA, and multiple front-end channels for the end user. These are expected to be made obsolete as additional apps are added to the dataspace.
See the [architectural](/Poort8.Dataspace.Wiki/architecture.md) outline of these functions for more detail.

7. **Challenges**:  
To make the software modular and customizable so as to allow stakeholders to evolve independently but in compliance with the principles and regulations of the dataspace.

8. **Tech Stack**:
    - No strong preferences for technologies.
    - Independent data sources must align with the dataspace data models and Identification, Authentication and Authorisation requirements.

9. **Support for New Apps**:
    - New apps can utilize the dataspace data model, authorization registry, and party registry but must handle authentication themselves.
