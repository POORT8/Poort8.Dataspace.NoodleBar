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