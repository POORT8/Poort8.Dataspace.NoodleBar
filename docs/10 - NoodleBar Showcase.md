# 10: NoodleBar Showcase
The NoodleBar Core Manager is a web application for managing the Organization Register and Authorization Register. This showcase displays the basics of the Core Manager. To keep things simple, this showcase functions using a local identity server.

### 10.1 Registering and logging in
When launching the Core Manager, the user is met with a welcome page. From here, the user can go to the login page. All other menu options are visible, but the user will automatically be directed to the login page if they are not authenticated.
![Home](showcase-images/01-home.png)
![Login](showcase-images/02-login.png)

If the user isn't registered yet, they can do so with an email address and password. If the registration is complete, the user can login using their credentials.
![Registering](showcase-images/03-registering.png)
![Logging in](showcase-images/04-logging-in.png)

### 10.2 Organization Register
Within the Organization Register, the user can manage their organizations. They can add a new organization using the **New Organization** button.
![OR](showcase-images/05-or.png)
![OR - Add organization](showcase-images/06-or-add-organization.png)

The user can delete an organization by clicking the delete button, or view additional details by clicking the **i** button.

### 10.3 Authorization Register
Within the Authorization Register, the user can manage their organizations mandates, resource groups, and policies.

#### 10.3.1 Organization mandates
The user can add an organization mandate using the **Add Organization** button. Each organization can only have one mandate, the *Add Organization* form contains a dropdown menu with available organizations from the Organization Register. This means that an organization mandate cannot be added if there are no organizations in the Organization Register. The user can delete an organization mandate by clicking the trash can button.
![AR - Organization mandates](showcase-images/07-ar-organization-mandates.png)
![Ar - Add organization mandate](showcase-images/08-ar-add-organization-mandate.png)

By clicking the label button, the user can manage named properties of an organization mandate.
![AR - Add organization mandate propery](showcase-images/09-ar-add-organization-mandate-property.png)

By clicking the **i** button, the user can view additional details of an organization mandate. From here, the user can manage employees of the organization mandate.
![AR - Organization mandate details](showcase-images/10-ar-organization-mandate-details.png)
![AR - Add organization mandate employee](showcase-images/11-ar-add-organization-mandate-employee.png)

#### 10.3.2 Resource groups
The user can create a new resource group using the **New ResourceGroup** button. The *Provider* field in the *New ResourceGroup* form contains a dropdown menu with organizations from the Organization Register. This means that a resource group cannot be created if there are no organizations in the Organization Register. By clicking the label button, the user can manage named properties of a resource group. The user can delete a resource group by clicking the trash can button.
![AR - Resource groups](showcase-images/12-ar-resourcegroups.png)
![AR - Create resource group](showcase-images/13-ar-create-resourcegroup.png)

By clicking the **i** button, the user can view additional details of a resource group. From here, the user can manage resources of the resource group. Resources are not exclusive to a resource group, making it possible to add existing resources from one to another resource group.
![AR - Resource group details](showcase-images/14-ar-resourcegroup-details.png)
![AR - Create resource group resource](showcase-images/15-ar-create-resourcegroup-resource.png)

#### 10.3.3 Policies
The user can create a new policy using the **New Policy** button. The *Issuer*, *Actor*, and *Service Provider* fields in the *New Policy* form each contain a dropdown menu with organizations from the Organization Register. This means that a policy cannot be created if there are no organizations in the Organization Register. By clicking the label button, the user can manage named properties of a policy. The user can delete a policy by clicking the trash can button, or view additional details by clicking the **i** button.
![AR - Policies](showcase-images/16-ar-policies.png)
![AR - Create policy](showcase-images/17-ar-create-policy.png)