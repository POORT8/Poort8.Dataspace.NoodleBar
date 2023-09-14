# Architecture
```mermaid
graph TB

%% Dataspace Adapter
subgraph Dataspace Adapter for non-BDI sources
    DataFetcher[Data fetcher]
    DataMapper[Data Mapper]
    AuthService[Authentication Service]
    AuthzService[Authorization Service]
    BDIService[BDI Service]
end

%% Dataspace Core
subgraph Dataspace Core
    PartyRegister[Party Register]
    AuthRegister[Authorization Register]
    CoreManager[Core Manager]
end

%% Dataspace Apps
subgraph Dataspace Prototype Apps
    RESTSrc[Retrieve Data from BDI-services]
    AuthUsers[Authenticate Users]
    PEP[Authorize Users]
    DataStore[Data Storage/Caching]
    Logic[Add Logic]
    FrontEnds[Multiple Frontends]
end

%% Connections within Dataspace Adapter
DataFetcher-->DataMapper
DataMapper-->AuthService
AuthService-->AuthzService
AuthzService-->BDIService

%% Connections within Dataspace Core
PartyRegister-->CoreManager
AuthRegister-->CoreManager

%% Connections within Dataspace Apps
RESTSrc-->AuthUsers
AuthUsers-->PEP
PEP-->DataStore
DataStore-->Logic
Logic-->FrontEnds

%% Dependencies
AuthzService-->|Uses|AuthRegister
AuthRegister-->|Used By|PEP
AuthService-->|Uses|PartyRegister
PartyRegister-->|Used By|AuthUsers
BDIService-->RESTSrc
```

## Dataspace Adapter

### Objectives

1. Convert non-BDI data sources into RESTful APIs.
2. Map data to Dataspace Schema (DCSA, P4, etc.).
3. Implement iSHARE authentication based on the Party Register.
4. Other authentication mechanisms like API-key.
5. Policy Enforcement Point (PEP) based on the Authorization Register.
6. Support for events and Pub/Sub mechanisms.

### Components

1. **API Gateway**
   - Converts non-BDI data into RESTful services.
  
2. **Data Mapper**
   - Maps raw data to dataspace schema (DCSA, P4, etc.).

3. **Authentication Service**
   - Implements iSHARE authentication based on the Party Register.
   - Supports other authentication methods like API keys.

4. **Authorization Service**
   - PEP implementation based on the Authorization Register.

5. **Event Service**
   - Supports real-time events and Pub/Sub mechanisms.

### Deployment Options

1. Integrated within a monolithic application.
2. Docker Containers (Service and Common).
3. Azure Functions.
4. Low-code platforms.

### Considerations

1. The adapter should be easily convertible to a BDI source.

---

## Dataspace Core

### Components

1. **Party Register**
    - Entails:
      - Parties
      - EP Creation?
      - Trusted List?
    - Requirements:
      - iSHARE endpoints
      - REST
      - Independent database?
    
2. **Authorization Register (AuthRegister)**
    - Entails:
      - Permissions
      - Products and features
      - Companies (parties) and users
    - Requirements:
      - iSHARE endpoints
      - REST
      - Independent database?
  
3. **Core Manager**
    - Web app for managing the Party Register and Authorization Register.
    - IAA of the app?
    - Use of Keyper?

---

## Dataspace Apps

1. Retrieve data from REST sources (both non-BDI and BDI).
2. Authenticate users or machines.
3. PEP based on Authorization Register.
4. Data storage/caching.
5. Can add logic and combine sources.
6. Multiple frontends (chat, web app, email, etc.).

### Deployment Options

1. Integrated within a monolithic application with REST-like interfaces.
2. Separate backend system.

Does this technical design align with your expectations and requirements? Please confirm or elaborate further.
