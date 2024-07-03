# Chapter 1: Introduction

### 1.1 Dataspaces and Federated Data Sharing

Dataspaces represent a paradigm shift in how organizations share and manage data. Unlike traditional centralized data storage systems, dataspaces allow data to remain with its original owner while enabling secure, controlled access and sharing. This approach is built on trust frameworks that define agreements on identification, authentication, and authorization to ensure data integrity and security.

Federated data sharing facilitates collaboration across different organizations by allowing them to access and use data without physically transferring it. In federated data sharing, data can be shared via a service provider, but the owner always remains in control. This method not only enhances data privacy and security but also ensures compliance with various legal and regulatory standards. The key principles of federated data sharing include:

- **Data Sovereignty**: Data owners retain control over their data at all times.
- **Interoperability**: Seamless integration across different systems and organizations.
- **Scalability**: Ability to scale data sharing across various sectors and regions.
- **Trust and Security**: Robust mechanisms for identification, authentication, and authorization.

By leveraging these principles, federated data sharing enables more efficient and effective use of data, fostering innovation and collaboration while maintaining strict security and privacy standards.

### 1.2 Overview of NoodleBar

NoodleBar, developed by Poort8, is a cutting-edge dataspace solution designed to facilitate secure, controlled, and efficient data sharing among businesses. NoodleBar is known for its "dataspace in a day" capability, providing all the necessary building blocks to get a dataspace up and running quickly. The main building blocks are an organization register and an authorization register, which are the core components of every dataspace. These components can be integrated with other systems to make the dataspace more federated. Inspired by the iSHARE Trust Framework and the principles of federated data sharing, NoodleBar adopts the same dataspace concepts and roles, ensuring seamless integration with iSHARE-compliant systems. NoodleBar offers a comprehensive platform for creating and managing dataspaces, with robust tools for data transformation and integration. These features aim to streamline data workflows, enhance data accessibility, and support informed decision-making.

### 1.3 About Poort8

Poort8 was the first to implement an iSHARE-compliant authorization register and has been pioneering in the field of dataspaces across multiple sectors. We are innovators, creating solutions quickly and efficiently, moving from pilot to production with ease. Poort8 focuses on delivering tangible results—no lengthy PowerPoint presentations, just working pilots. We excel at translating business requirements into functional dataspaces, establishing ourselves as authorities in the field of dataspaces.

### 1.4 Purpose and Scope

This documentation aims to provide a comprehensive guide to understanding, deploying, and using NoodleBar. The scope of this documentation includes:

- Basic concepts and core functionalities of NoodleBar.
- Step-by-step deployment instructions and system requirements.
- User guides for navigating and utilizing the platform.
- Detailed descriptions of advanced features and customization options.
- Administrative and troubleshooting tips to ensure smooth operation.

### 1.5 Audience

This documentation is intended for:
- **IT Administrators**: For deployment, management, and maintenance of NoodleBar in their infrastructure.
- **Developers**: For extending and integrating NoodleBar with other systems through APIs and custom plugins.
- **Business Stakeholders**: To understand the benefits and applications of NoodleBar in driving data-driven decision-making within their organizations.

Prerequisites for the audience include a basic understanding of data management principles and IT infrastructure.

### 1.6 Integration with iSHARE and Federated Data Sharing

NoodleBar seamlessly integrates with the iSHARE Trust Framework, leveraging its robust identification, authentication, and authorization mechanisms. This integration ensures that data exchanges are secure, transparent, and controlled, aligning with the highest standards of data governance. By adopting iSHARE's dataspace concepts and roles, NoodleBar provides a familiar and compliant environment for organizations already using or planning to use iSHARE.

Inspired by the principles of federated data sharing, NoodleBar ensures that data remains at its source while enabling controlled access and sharing across different sectors and organizations. This approach enhances data accessibility and trust, promoting responsible data usage, privacy, and transparency.

For more information on iSHARE, visit the [iSHARE Trust Framework website](https://framework.ishare.eu/is/).

### 1.7 Context and Objective

The project is under the Basis Data Infrastructuur (BDI) umbrella, pending its ongoing development. The objective is to facilitate setting up dataspaces that follow certain principles, serving as an initial platform for data providers, apps, and data consumers.

### 1.8 Roles

- **Data Providers**: Organizations that either offer a data source with raw data or an app with processed data. In all cases, access conditions are set by the data owner.
- **App Providers**: Organizations that act as intermediaries, adding value to raw data. They act as a Data Consumer on behalf of their end users, and as a Data Provider for their end users.
- **Data Consumers**: Organizations that use data via Service Providers or directly.
- **Dataspace Initiators**: Organizations that setup and manage the dataspace.

### 1.9 Principles

- **Data Sovereignty**: Data owners (issuers) can issue access to their data, even if through federated apps.
- **Data Localization**: Data stays at its source unless caching or staging is essential.
- **Identity Flexibility**: Data consumers choose their identity providers.

### 1.10 Customer Journeys

The wiki describes the following Customer Journeys in more detail:

- **Initiating Dataspace Core**
- **Onboarding Data Sources**
- **Onboarding Data Owners and Consumers**
- **Data Sources Becoming Independent**
- **Adding Providers and Apps**

The first three journeys comprise the launch of a first (prototype) of a dataspace. Journeys 4 and 5 allow data sources and Service Providers to become independent contributors to the dataspace.
