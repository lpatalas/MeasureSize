---
external help file: MeasureSize.dll-Help.xml
Module Name: MeasureSize
online version:
schema: 2.0.0
---

# Measure-ItemSize

## SYNOPSIS

Computes the size of the file system items.

## SYNTAX

```
Measure-ItemSize [[-Path] <String[]>] [<CommonParameters>]
```

## DESCRIPTION

Computes the size of the file system items.

## EXAMPLES

### Example 1
```powershell
PS C:\> Measure-ItemSize
```

Compute size of all folders and files in current directory

### Example 2
```powershell
PS C:\> Get-ChildItem -Recurse -Include Debug,*.pdb | Measure-ItemSize
```

Compute sizes for items returned by other cmdlet

### Example 3
```powershell
PS C:\> Measure-ItemSize | where Size -gt 1KB
```

Filter items by size

## PARAMETERS

### -Path

Path to the file or directory. Can be a wildcard. If not specified
then the default value of "*" is used - it is it will compute the size
of each item in current directory. If the path points to something that
is not file system item, like variable or registry key then error will
be written to the error stream.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: *
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### MeasureSizeModule.ItemSize

Properties:
* `Item` - [FileInfo](https://docs.microsoft.com/en-us/dotnet/api/system.io.fileinfo) or [DirectoryInfo](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo) for given item
* `Size` - size of the item in bytes

## NOTES

## RELATED LINKS
