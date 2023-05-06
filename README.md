# EfCore Helper Context

If you are working on projects that contain multi-tenant, auditable or soft deletable tables,
So you had add  ``` && TenantId=={someValue} ``` or  ```&& !IsDeleted ``` conditions or something like that to all of your entity framework core queries.
Sometime this is hard to manage all of this queries, especially large project that contain big amount of query.
If you or your teammate forget to add these condition to queries. You may be in trouble. 🤣

This tiny library may help you. Take a look 

## Installation
```
dotnet add package Armudu.EntityFrameworkCore.Atom.Contract
dotnet add package Armudu.EntityFrameworkCore.Atom
 ```
## Usage
If your Entity and DbContext classes are placed on different layers due to the architecture (for example CQRS) you are using, 
you have to install package ```Armudu.EntityFrameworkCore.Atom.Contract``` to layer that contain entities. Also you have to install package 
```Armudu.EntityFrameworkCore.Atom``` to layer that contain your ApplicationDbContext.

Inherit from EfCoreHelperContext
![RemoteImage](https://camo.githubusercontent.com/13ae804923aa0e7d3db0f4c6f5b5d325c7e5560e98658993aecde42d63cd4205/68747470733a2f2f692e6962622e636f2f6733766446546a2f646f776e6c6f61642e706e67)

Implement User service.
UserId injected from constructor for testing. Don't get hung up on it,You can implement for your needs.

![UserService](https://www.linkpicture.com/q/download-1_169.png)

And implement the contract which you want to use.
You have to set type of Id of audit user 
![EntityUsage](https://www.linkpicture.com/q/download_753.png)

After that, your queries automatically will become like this 
![Query](https://camo.githubusercontent.com/2541eac1308fd464a0a89f56b39a2a9636a4eb28c745d7c29552dbd0672a6ae6/68747470733a2f2f692e6962622e636f2f78437a706342522f646f776e6c6f61642e706e67)
Current tenantId filter and !IsDeleted will be add to query

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)