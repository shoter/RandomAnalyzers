## Installation

RequiredMember can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio.

**Install using the command line:**
```bash
Install-Package RandomAnalyzers.RequiredMember
```

## Description

RequiredMember library provides you new attribute called `RequiredMemberAttribute`. All members of the class/struct that are marked with this attribute will require to be initialized inside object initializer. Otherwise the `RequiredMember.Analyzer` will sygnalize a warning that members were not initialized and you are required to do so.

## Code

```csharp
using RandomAnalyzers.RequiredMember;

public class Test
{
	[RequiredMember]
	public int property { get; set; }
}

// no warnings/errors

var variable = new Test() 
{
	property = 123
};

// warning
var variable = new Test();
var variable = new Test() {};

```


## UI Example

![Screenshot showing warnings for RequiredMember.Analyzer](https://github.com/shoter/RandomAnalyzers/raw/master/RandomAnalyzers.RequiredProperty/imgs/Example.png)