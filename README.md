# EfCore Helper Context

This library created for helping to converting .net entity framework projects to multi tenant entity framework project

## Installation

 Not ready yet 
 
## Usage

Inherit from EfCoreHelperContext

![RemoteImage](https://i.ibb.co/g3vdFTj/download.png)

And implement the contract which you want to use
![EntityUsage](https://i.ibb.co/w0Tz3nV/download.png)
After that, your queries converted to this 
![Query](https://i.ibb.co/xCzpcBR/download.png)
Current tenantId filter and !IsDeleted automatically added to query

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)