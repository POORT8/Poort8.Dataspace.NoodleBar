``` mermaid
graph TD
    A((Start)) --> B[Login Page]
    B -->|Sales Rep| C[Offer Management]
    B --> D[Dashboard]
    
    C -->|Create Offer| E[Select Features]
    E --> F[Save Offer]
    F --> C
    
    D --> G[Organisation List]
    G -->|Create Organisation| H[Organisation Form]
    H --> I[Save Organisation]
    I --> G
    G -->|View| J[Organisation Details]
    J -->|Add Employee| K[Employee Form]
    K --> L[Save Employee]
    L --> J
    J -->|Subscribe to Offer| M[Select Offer]
    M --> N[Confirm Subscription]
    N --> J
    
    F --> D
```

A typical way of how a dataspace is used, includes the functionality that a data owner or a service provider assigns certain offers to another organisation. This authorizes the other organisation to use features of that offer as a data service consumer. Although data owners can manage this directly with their data service provider, the dataspace incubater stack includes optional components that can be used for this.

This [mock-up page](/Poort8.Dataspace.Mockup/Poort8.Keyper.subscriptionManager.html) shows basic functionality how a data owner or service provider could use this to manage access to their resources.