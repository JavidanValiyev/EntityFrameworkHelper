# EfCore Helper Context

This library created for helping to converting .net entity framework projects to multi tenant entity framework project

## Installation

 Not ready yet 
 
## Usage

Inherit from EfCoreHelperContext

![RemoteImage](https://camo.githubusercontent.com/13ae804923aa0e7d3db0f4c6f5b5d325c7e5560e98658993aecde42d63cd4205/68747470733a2f2f692e6962622e636f2f6733766446546a2f646f776e6c6f61642e706e67)

And implement the contract which you want to use
![EntityUsage](https://camo.githubusercontent.com/4d98749f9602b63161c3f92089ff19aeb5d1f48f9c4b87f81f7c97517b7e88fd/68747470733a2f2f692e6962622e636f2f7730547a336e562f646f776e6c6f61642e706e67)
After that, your queries converted to this 
![Query](https://camo.githubusercontent.com/2541eac1308fd464a0a89f56b39a2a9636a4eb28c745d7c29552dbd0672a6ae6/68747470733a2f2f692e6962622e636f2f78437a706342522f646f776e6c6f61642e706e67)
Current tenantId filter and !IsDeleted automatically added to query

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)