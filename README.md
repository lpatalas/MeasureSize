# MeasureSize PowerShell module

## Installation

```
Install-Module MeasureSize -ScopeCurrent CurrentUser
```

## Usage

```
Import-Module MeasureSize
```

### Compute size of all folders and files in current directory

```
PS C:\Projects\MeasureSize> Measure-ItemSize

  Size Item
  ---- ----
10.32M C:\Projects\MeasureSize\.artifacts
96.00K C:\Projects\MeasureSize\.ionide
 2.15K C:\Projects\MeasureSize\.vscode
 3.80M C:\Projects\MeasureSize\src
  181B C:\Projects\MeasureSize\.editorconfig
   13B C:\Projects\MeasureSize\.gitattributes
   41B C:\Projects\MeasureSize\.gitignore
   98B C:\Projects\MeasureSize\global.json
 1.05K C:\Projects\MeasureSize\LICENSE.md
 1.69K C:\Projects\MeasureSize\MeasureSize.sln
 2.27K C:\Projects\MeasureSize\publish.ps1
  252B C:\Projects\MeasureSize\README.md
   87B C:\Projects\MeasureSize\todo.md
```

### Compute sizes for items returned by other cmdlet

```
PS C:\Projects\MeasureSize> Get-ChildItem -Recurse -Include Debug,*.pdb | Measure-ItemSize

   Size Item
   ---- ----
  1.85M C:\Projects\MeasureSize\src\bin\Debug
196.68K C:\Projects\MeasureSize\src\bin\Debug\netstandard2.0\MeasureSize.pdb
  1.91M C:\Projects\MeasureSize\src\obj\Debug
196.68K C:\Projects\MeasureSize\src\obj\Debug\netstandard2.0\MeasureSize.pdb
```

### Filter items by size

```
PS C:\Projects\MeasureSize> Measure-ItemSize | where Size -gt 1KB

  Size Item
  ---- ----
10.32M C:\Projects\MeasureSize\.artifacts
96.00K C:\Projects\MeasureSize\.ionide
 2.15K C:\Projects\MeasureSize\.vscode
 3.80M C:\Projects\MeasureSize\src
 1.05K C:\Projects\MeasureSize\LICENSE.md
 1.69K C:\Projects\MeasureSize\MeasureSize.sln
 2.27K C:\Projects\MeasureSize\publish.ps1
 1.27K C:\Projects\MeasureSize\README.md
```

### Get statistics for each drive

```
PS C:\Projects\MeasureSize> Measure-DriveSize

Drive          Used % FreeSpace UsedSpace TotalSize
-----          ------ --------- --------- ---------
C:\ (SYSTEM)    86,8%    29.27G   193.23G   222.51G
D:\ (DATA1)     61,1%   108.66G   170.79G   279.45G
E:\ (DATA2)     60,5%   155.72G   238.14G   393.86G
```

### Filter drives by used percentage

```
PS C:\Projects\MeasureSize> Measure-DriveSize | where UsedPercentage -lt 0.7

Drive          Used % FreeSpace UsedSpace TotalSize
-----          ------ --------- --------- ---------
D:\ (DATA1)     61,1%   108.66G   170.79G   279.45G
E:\ (DATA2)     60,5%   155.72G   238.14G   393.86G
```
