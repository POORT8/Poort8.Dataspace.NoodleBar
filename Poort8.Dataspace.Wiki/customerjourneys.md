# Dataspace Noodle Bar
Create your own dataspace in 6 steps.

## 1. Deploy the base for your dataspace
The base layer of any dataspace must be organized by a dataspace initiator.

1. **Dataspace Initiator Engagement**: The dataspace initiator goes through the incubator program to align with the principles and governance of a dataspace.
2. **Authentication Decision**: Decide what modes of authentication will be allowed for companies and employees. The following authentication mechanisms are supported:
   - Username en passwords via ASP.NET Core Identity
   - Magic link via email
   - eHerkenning (_Requires implementation with external service provider_)
   - Office 365 (_Requires implementation with external service provider_)
4. **Organization Register Setup**: Define what data needs to be collected for users in the dataspace organization register.  
```
deploy
Organization Register
Dataspace Manager
```
4. **Registering participants**: Use the Dataspace Manager to register all participating organisations in the dataspace, or allow them to use self-service onboarding via the Dataspace Manager (customer journey 3).

## 2. Create a landing zone for data sources

The dataspace initiator can decide how much support the dataspace provides to data sources.
1. **Data Source Selection**: Identify potential data sources that align with the dataspace objectives.
2. **Register Data Sources**: Verify compliancy and register Data Sources in the organization register.
3. **Data model**: The dataspace initiator can define data model(s) that are used in the dataspace. If not, data sources can define their own data models and must organise authorizations accordingly.
4. **Authorization registry setup**: Optionally, the Dataspace initiator can establish an authorization registry for permissions and access controls in the dataspace.
   - _Authorization usecase_: Based on the access control rules, an authorization usecase and authorization rules can be selected from existing models or a new one must be developed. Out-of-the-box, the following usecases are available:_
     - Access Control Lists
     - ABAC (basic iSHARE)
     - Subscription-based Access Control  
   - Or a custom ABAC usecase can be created for non-standard authorization rules.
5. **Dataspace Adapter**: Optionally, the Dataspace initiator can decide to provide a Dataspace Adapter to kickstart Data sources in the Dataspace. Each of these usecases can be applied in accordance with BDI standards for IAA.
    - _Data Mapping_: Data sources specify how their data should be mapped to the dataspace data model.
    - _Data Provisioning_: Data sources provide raw data in one of the formats supported by the dataspace (API, Data dump, Custom, etc.)
    - _Data Standardization_: The Dataspace Adapter is configured to map the raw data to the Dataspace data model and provide a BDI service for it, including IAA.

```
deploy 
Authorization Register
Authorization usecase
Dataspace adapter(s)
```  

## 3. Registering Data Owners and Consumers
Allow Data Owners and Consumers to use self-service onboarding via the Dataspace Manager.
1. **Authentication and registration**: A representative of the organization uses an accepted Identity Provider to authenticate themselves as formal representation, and may provide additional details.
2. **Onboarding**: Organizations use the Dataspace onboarding service(s) to become a member of the Dataspace and be registered. _(Uses Organisation Register 1.3)_
3. **Access Control**: Data owners define conditions under which their data can be accessed by data consumers. _(Uses Authorization Registry from 2.4)_

## 4. Use Keyper for easy onboarding  
Use SaaS service with implemented authentication solutions to start immediately with high trust levels.
1. **Connect dataspace**: connect Keyper to your dataspace and select allowed authentication providers.
2. **Use Keyper**: Keyper provides seamless onboarding and management processes for BDI dataspaces, equivalent to those of journey 4, but including eHerkenning and Office 365 implementations.

```
configure 
Keyper
``` 

## 5. Add a Prototype App
Use SaaS service to facilitate multi-channel access to prototype commands that combine data sources into valuable tools for users.
1. **Dataspace Prototype**: The Dataspace initiator can decide to connect a prototype app to kickstart usage of the dataspace.
2. **Initial App Design**: Decide on functionalities and commands to transform the raw data into valuable insights. _(Uses 2.2 and 2.3)_
3. **Demonstration Setup**: Develop a demonstration flow to guide users through the features of the app.

```
configure 
Prototype
``` 
## 6. Seasoning with non-standardized data
Create temporary solutions to include non-standardized data in your dataspace.

```
develop 
Data ETL
``` 

## 7. Extensions: Adding Providers and Apps to the Dataspace

1. **New App Development**: Organizations design and build new apps in alignment with the dataspace. _(Uses 2.2 and 2.3)_
2. **Dataspace Registration**: New apps register themselves with the dataspace as both data service consumers and providers.
3. **Usage Terms**: Set conditions under which the prototype app can be used. _(Can use Authorization Registry from 1.4)_
4. **User Onboarding**: Registered users can now use these new apps to interact with data sources.


