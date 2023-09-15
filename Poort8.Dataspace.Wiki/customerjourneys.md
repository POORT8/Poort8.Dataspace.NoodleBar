# Customer Journeys:

## 1. Initiating a Dataspace

1. **Dataspace Owner Engagement**: The dataspace owner goes through the incubator program to align with the principles and governance of a dataspace.
2. **Authentication Decision**: Decide what modes of authentication will be allowed for companies and employees.
3. **Party Register Setup**: Define what data needs to be collected for users in the dataspace party register.
4. **Authorization Register Setup**: Optionally, establish an authorization registry that will define permissions and access controls for the dataspace.

## 2. Onboarding Data Sources (Depends on Journey 1)

1. **Data Source Selection**: Identify potential data sources that align with the dataspace objectives.
2. **Register Data Sources**: Verify compliancy and register Data Sources in the party register.
3. **Mapping Specification**: Data sources specify how their data should be mapped to the dataspace data model.
4. **Access Control**: Data sources define conditions under which their data can be accessed by data consumers. _(Uses Party Registry from 1.3 and can use Authorization Registry from 1.4)_  
_Optionally_ the Dataspace initiator can decide to provide a Dataspace Adapter to kickstart Data sources in the Data space
5. _Data Provisioning_: Data sources provide raw data in one of the formats supported by the dataspace (API, Data dump, Custom, etc.)
6. _Data Standardization_: The Dataspace Adapter is configured to map the raw data to the Dataspace data model and provide a BDI service for it, including IAA.

## 3. Onboarding Data Owners and Consumers Launching Prototype Apps (Depends on Journey 2) 
1. **Onboarding**: Parties use the Dataspace onboarding service to become a member of the Dataspace
2. **Authentication and registration**: A representative of the party use an accepted Identity Provider to authenticate themselves as formal representation, and may provide additional details.
3. **Party Register**: Verify compliancy and register Data Sources in the party register, preferably automatically.

## 4. Data Sources Becoming Independent

1. **Development Planning**: Data sources decide on the tech stack and design to build their independent implementations.
2. **Dataspace Update**: Update dataspace registration to reflect their new status.
3. **App Compatibility**: Ensure that the prototype app and third-party apps still function with minimal code changes.

## 5. Adding Service Providers to the Dataspace

1. **New App Development**: Parties design and build new apps in alignment with the dataspace. _(Uses 2.2 and 2.3)_
2. **Dataspace Registration**: New apps register themselves with the dataspace as both data service consumers and providers.
3. **Usage Terms**: Set conditions under which the prototype app can be used. _(Can use Authorization Registry from 1.4)_
4. **User Onboarding**: Registered users can now use these new apps to interact with data sources.

## 6. _Optionally_ Create a prototpye service
The Dataspace initiator can decide to connect a service to the onboarding process to kickstart usage of the dataspace.
1. _Initial App Design_: Decide on functionalities and commands to transform the raw data into valuable insights. _(Uses 2.2 and 2.3)_
2. _Demonstration Setup_: Develop a demonstration flow to guide users through the features of the app.
3. _Dataspace Prototyping_: Create the service using functionalities of the Dataspace Prototyping stack
