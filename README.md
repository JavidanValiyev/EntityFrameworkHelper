# EfCore Helper Context

If you've ever worked on projects with **multi-tenant**, **auditable**, or **soft-deletable** tables, you know the pain. You have to constantly add conditions like `&& TenantId=={someValue}` or `&& !IsDeleted` to *all* of your Entity Framework Core queries.

Managing all those extra conditions can be a huge headache, especially in large projects with tons of queries. If you or a teammate ever forgets to add one of these conditions, you could end up in a world of trouble! 😅

This little library is here to help! Take a look at how it works.

## Installation
Just use the following commands to install the necessary packages:
```
dotnet add package Armudu.EntityFrameworkCore.Atom.Contract
dotnet add package Armudu.EntityFrameworkCore.Atom
 ```
## Usage
If your project uses an architecture like **CQRS**, and your Entity and DbContext classes are in different layers, you'll need to install the packages this way:
* Install `Armudu.EntityFrameworkCore.Atom.Contract` in the layer that holds your entities.
* Install `Armudu.EntityFrameworkCore.Atom` in the layer that contains your ApplicationDbContext.
### **1. Inherit from `EfCoreHelperContext`**
You need to make your DbContext inherit from `EfCoreHelperContext`.

![RemoteImage](https://github.com/JavidanValiyev/EntityFrameworkHelper/blob/main/EntityFrameworkHelper.Contracts/Assets/Photo1.png?raw=true)

### **2. Implement the User Service**
Next, you need to implement a user service. This is how the library gets information like the current user's ID.

> Note: The example shows the `UserId` being injected via the constructor for testing. Don't worry about that specific implementation—you can design your service to fit your application's actual needs.

### **3. Implement the Contract on Your Entities**
Finally, you implement the specific contracts you want to use on your entity classes (e.g., `IAuditableEntity`).

> You must also specify the data type (e.g., `Guid`, `int`) for the audit user's ID.

---

After you've done this, the library automatically takes care of those boilerplate conditions!

Your queries will now automatically include filters for the **current tenant ID** and the `!IsDeleted` condition. No more manual filtering!

---

## Contributing
Pull requests are always welcome! If you have a major change in mind, please open an issue first so we can discuss it before you start coding.

Also, please make sure you update the tests as needed.

---

## License
[MIT](https://choosealicense.com/licenses/mit/)