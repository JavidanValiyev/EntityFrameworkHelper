# Armudu

If you've ever worked on projects with **multi-tenant**, **auditable**, or **soft-deletable** tables, you know the pain. You have to constantly add conditions like `&& TenantId=={someValue}` or `&& !IsDeleted` to *all* of your Entity Framework Core queries.

Managing all those extra conditions can be a huge headache, especially in large projects with tons of queries. If you or a teammate ever forgets to add one of these conditions, you could end up in a world of trouble! 😅

This little library is here to help! Take a look at how it works.

## Installation
Use the following commands to install the necessary packages:
```
dotnet add package Armudu.EntityFrameworkCore.Atom.Contract
dotnet add package Armudu.EntityFrameworkCore.Atom
 ```
## Usage
If your project uses an architecture like **CQRS**, and your Entity and DbContext classes are in different layers, you'll need to install the packages this way:
* Install `Armudu.EntityFrameworkCore.Atom.Contract` in the layer that holds your entities.
* Install `Armudu.EntityFrameworkCore.Atom` in the layer that contains your ApplicationDbContext.
### **1. Implement `IDynamicQueryFilterSource` interface on your DbContext**
In IDynamicQueryFilterSource interface you have to implement `FilterKey` object. 
This object should contain the key for the Entity Framework model caching mechanism.
You can set the tenant id or unique value for the current tenant, so the library can apply global filter for 
the current tenant. Take a look to photo.

![RemoteImage](https://github.com/JavidanValiyev/EntityFrameworkHelper/blob/main/workflows/Screenshot_1.png?raw=true)

### **2. Call AddGlobalFilters func**
Next, you need to call AddGlobalFilters with your TenantId value not FilterKey object.

> Note: Do not miss the FilterKey object and tenant id value should be used for different purposes.
> FilterKey object is used for a caching mechanism, and tenant id value is used for global filter.
> Of course, you can use the same value for both purposes.

### **3. Implement the Contract on Your Entities**
In next step, you have to implement the specific contracts you want to use on your entity classes (e.g., `IAuditableEntity`).

> You must also specify the data type (e.g., `Guid`, `int`) for the audit user's ID.
### **4 Call ConfigureStates func**
You have to call ConfigureStates in your SaveChanges method of your DbContext.
> take a look to photo..
> <img src="https://github.com/JavidanValiyev/EntityFrameworkHelper/blob/main/workflows/Screenshot_3.png?raw=true">
### **5. Replace ModelCacheKeyFactory with DynamicModelCacheKeyFactory**
Finally, you have to replace the default ModelCacheKeyFactory with DynamicModelCacheKeyFactory.
> ```options.ReplaceService<IModelCacheKeyFactory, DynamicFilterModelCacheKeyFactory>();``` use this line of code
> on where you write ```AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));```
> take a look to photo.
<img src="https://github.com/JavidanValiyev/EntityFrameworkHelper/blob/main/workflows/Screenshot_2.png?raw=true">
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