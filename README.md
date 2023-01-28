# EfCore Helper Context

This library created for helping to converting .net entity framework projects to multi tenant entity framework project

## Installation
```
dotnet add package Armudu.EntityFrameworkCore.Atom
 ```
## Usage

Inherit from EfCoreHelperContext

![RemoteImage](https://camo.githubusercontent.com/13ae804923aa0e7d3db0f4c6f5b5d325c7e5560e98658993aecde42d63cd4205/68747470733a2f2f692e6962622e636f2f6733766446546a2f646f776e6c6f61642e706e67)

Implement User service

![UserService](https://camo.githubusercontent.com/08ce2de040df94b248b17422219a02a28a2b969897466d7b044992becc1cf3eb/68747470733a2f2f7777772e6c696e6b706963747572652e636f6d2f712f696e6469722d315f332e706e67)

And implement the contract which you want to use.
You have to set type of Id of audit user 
![EntityUsage](https://www.linkpicture.com/q/download_753.png)

After that, your queries automatically will become like this 
![Query](https://camo.githubusercontent.com/2541eac1308fd464a0a89f56b39a2a9636a4eb28c745d7c29552dbd0672a6ae6/68747470733a2f2f692e6962622e636f2f78437a706342522f646f776e6c6f61642e706e67)
Current tenantId filter and !IsDeleted added to query

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)