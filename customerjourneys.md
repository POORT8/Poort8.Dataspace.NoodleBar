# Customer Journeys:

## 1. Initiating a Dataspace

1. **Dataspace Owner Engagement**: The dataspace owner goes through the incubator program to align with the principles and governance of a dataspace.
2. **Authentication Decision**: Decide what modes of authentication will be allowed for companies and employees.
3. **Party Register Setup**: Define what data needs to be collected for users in the dataspace party register.
4. **Authorization Register Setup**: Establish an authorization registry that will define permissions and access controls for the dataspace.

## 2. Onboarding Data Sources (Depends on Journey 1)

1. **Data Source Selection**: Identify potential data sources that align with the dataspace objectives.
2. **Data Provisioning**: Data sources provide raw data in one of the formats supported by the dataspace (API, Data dump, Custom, etc.)
3. **Mapping Specification**: Data sources specify how their data should be mapped to the dataspace data model.
4. **Access Control**: Data sources define conditions under which their data can be accessed by data consumers. **(Uses Authorization Registry and Party Registry from 1.3 and 1.4)**

## 3. Launching Prototype Apps (Depends on Journey 2)

1. **Initial App Design**: Decide on functionalities and commands to transform the raw data into valuable insights. **(Uses 2.2 and 2.3)**
2. **Usage Terms**: Set conditions under which the prototype app can be used. **(Uses Authorization Registry from 1.4)**
3. **Demonstration Setup**: Develop a demonstration flow to guide users through the features of the app.

## 4. Data Sources Becoming Independent

1. **Development Planning**: Data sources decide on the tech stack and design to build their independent implementations.
2. **Dataspace Update**: Update dataspace registration to reflect their new status.
3. **App Compatibility**: Ensure that the prototype app and third-party apps still function with minimal code changes.

## 5. Adding Extra Apps to the Dataspace

1. **New App Development**: Parties design and build new apps in alignment with the dataspace.
2. **Dataspace Registration**: New apps register themselves with the dataspace as both data service consumers and providers.
3. **User Onboarding**: Registered users can now use these new apps to interact with data sources.