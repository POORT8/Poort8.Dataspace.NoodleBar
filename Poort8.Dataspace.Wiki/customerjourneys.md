# Customer Journeys:

## 1. Initiating a Dataspace

1. **Dataspace Initiator Engagement**: The dataspace initiator goes through the incubator program to align with the principles and governance of a dataspace.
2. **Authentication Decision**: Decide what modes of authentication will be allowed for companies and employees.
3. **Organization Register Setup**: Define what data needs to be collected for users in the dataspace organization register.
4. **Onboarding**: Onboarding of new data consumers can be done a) manually in the dataspace core manager, b) automatically via an onboarding flow in the Dataspace Core and/or c) automatically via an onboarding flow in a federated app.
5. **Data model**: The dataspace initiator can define data model(s) that are used in the dataspace. If not, data sources can define their own data models and must organise authorizations accordingly.
6. **Authorization model**: Optionally, establish an authorization registry for permissions and access controls in the dataspace. _Based on the access control rules, the correct authorization model and authorization rules can be selected from existing models or a new one must be developed._

## 2. Onboarding Data Sources (Depends on Journey 1)

1. **Data Source Selection**: Identify potential data sources that align with the dataspace objectives.
2. **Register Data Sources**: Verify compliancy and register Data Sources in the organization register.
3. **Mapping Specification**: Data sources specify how their data should be mapped to the dataspace data model.
4. **Access Control**: Data owners define conditions under which their data can be accessed by data consumers. _(Uses Organization Registry from 1.3 and can use Authorization Registry from 1.5)_  
_Optionally_ the Dataspace initiator can decide to provide a Dataspace Adapter to kickstart Data sources in the Dataspace
5. _Data Provisioning_: Data sources provide raw data in one of the formats supported by the dataspace (API, Data dump, Custom, etc.)
6. _Data Standardization_: The Dataspace Adapter is configured to map the raw data to the Dataspace data model and provide a BDI service for it, including IAA.

## 3. Launching a Prototype App
The Dataspace initiator can decide to connect a service to the onboarding process to kickstart usage of the dataspace.
1. _Initial App Design_: Decide on functionalities and commands to transform the raw data into valuable insights. _(Uses 2.2 and 2.3)_
2. _Demonstration Setup_: Develop a demonstration flow to guide users through the features of the app.
3. _Dataspace Prototyping_: Create the service using functionalities of the Dataspace Prototyping stack

## 4. Onboarding Data Owners and Consumers (Depends on Journey 2) 
1. **Onboarding**: Organizations use the Dataspace onboarding service(s) from 1.6. to become a member of the Dataspace.
2. **Authentication and registration**: A representative of the organization uses an accepted Identity Provider to authenticate themselves as formal representation, and may provide additional details.
3. **Organization Register**: Verify compliancy and register Data Sources in the organization register.

## 5. Data Sources Becoming Independent

1. **Development Planning**: Data sources decide on the tech stack and design to build their independent implementations.
2. **Dataspace Update**: Update dataspace registration to reflect their new status.
3. **App Compatibility**: Ensure that the prototype app and third-party apps still function with minimal code changes.

## 6. Adding Providers and Apps to the Dataspace

1. **New App Development**: Organizations design and build new apps in alignment with the dataspace. _(Uses 2.2 and 2.3)_
2. **Dataspace Registration**: New apps register themselves with the dataspace as both data service consumers and providers.
3. **Usage Terms**: Set conditions under which the prototype app can be used. _(Can use Authorization Registry from 1.4)_
4. **User Onboarding**: Registered users can now use these new apps to interact with data sources.


