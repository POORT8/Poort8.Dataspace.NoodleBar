# Chapter 4: NoodleBar Implementation Stages

Poort8 has been the frontrunner in creating solutions for dataspaces. We believe it is important to have a running prototype as soon as possible, which is why we created NoodleBar. Beginning with the iSHARE framework from the start has proven to be far too complex, and frankly, most use cases don't need this level of security and governance. With NoodleBar, we set out to create a solution that makes it possible to set up a dataspace prototype in a day. This prototype can be integrated with existing data sources or synthetic data to use the dataspace from day one.

Another hurdle with iSHARE is that it does not use any existing standards. This means there are no packages, middleware, or even code examples available for developers to use. Developers need to first learn this complex iSHARE standard and then implement everything from scratch. To address this, we added the de-facto standard to NoodleBar: OAuth.

OAuth is an open standard for access delegation, commonly used for token-based authentication and authorization. It allows third-party services to exchange information without exposing user credentials. The added value of OAuth includes:

- **Ease of Use**: OAuth is widely recognized and supported, making it easier for developers to integrate and use.
- **Security**: OAuth provides a secure way to authorize access without sharing credentials, reducing the risk of credential theft.
- **Interoperability**: OAuth supports integration with various services and platforms, enhancing the flexibility and scalability of the dataspace.

### Stage 1: Deploy NoodleBar Using the Local Identity Server

The first stage involves deploying NoodleBar using the Local Identity Server. This setup provides the essential components to get started quickly:

- **Organization Register**: Manages organizational identities, ensuring proper identification and authentication of all participating entities.
- **Authorization Register**: Manages access control, allowing the definition and enforcement of authorization policies for organizations.
- **NoodleBar Web App**: A user-friendly interface for managing organizations and adding authorization policies.

Using this setup, authentication and tokens are managed by the Organization Register. Data providers can modify their existing APIs to integrate with the NoodleBar system. This involves using the enforce API of the Authorization Register to ensure that data is only returned if the enforce API returns an allowed response.

### Stage 2: Deploy NoodleBar Using an OAuth Server

The second stage leverages the OAuth protocol for enhanced federated capabilities:

- **OAuth Identity Server**: Supports OAuth-based authentication and authorization, providing a flexible and scalable solution.
- **Organization and Authorization Registers**: Continue to manage identities and access control.

This stage maintains the same core functionalities as Stage 1 but enhances security and interoperability by using the OAuth standard. This allows for seamless integration with external systems, making it suitable for more complex and distributed environments.

### Stage 3: Deploy NoodleBar Using iSHARE

For use cases that demand the highest standards of security and governance, iSHARE can be utilized:

- **iSHARE Satellite**: Replaces the NoodleBar Organization Register. The iSHARE Satellite, which uses X.509 certificates for robust authentication, must be deployed and maintained by experienced developers. For more information, visit the [iSHARE Satellite GitHub repository](https://github.com/iSHAREScheme/iSHARESatellite).
- **iSHARE Authorization Register**: Ensures compliance with stringent security and governance standards.

Deploying NoodleBar using iSHARE ensures that all data exchanges are conducted with the highest level of trust and security. The NoodleBar Authorization Register can be configured to be fully iSHARE compliant. However, it's important to note that this stage has proven to be complex, costly, and time-consuming. For production environments, eIDAS certificates must be purchased by every party in the dataspace, further increasing the cost, complexity, and maintainability demands.

### Advantages of NoodleBar

- **Quick Deployment**: NoodleBar can be deployed in a day, providing all the essential components for a fully functional pilot.
- **Versatility**: NoodleBar supports various deployment models, from simple local identity management to advanced OAuth and iSHARE compliant setups.
- **Scalability**: Designed to grow with organizational needs, supporting increasing data sharing requirements.
- **Compliance**: Aligns with the iSHARE Trust Framework, ensuring adherence to high security and governance standards.
- **Flexibility**: NoodleBar can be used to quickly set up a dataspace in a day and is also being used in fully compliant federated dataspaces.

By following these stages, organizations can leverage NoodleBar’s full potential, starting from a basic setup to a highly secure and compliant dataspace solution.
