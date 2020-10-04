---
external help file: MeasureSize.dll-Help.xml
Module Name: MeasureSize
online version:
schema: 2.0.0
---

# Measure-DriveSize

## SYNOPSIS

Gets the used, free and total space for each drive.

## SYNTAX

```
Measure-DriveSize [<CommonParameters>]
```

## DESCRIPTION

Gets the used, free and total space for each drive.

## EXAMPLES

### Example 1
```powershell
PS C:\> Measure-DriveSize
```

Get space statistics for all drives

### Example 2
```powershell
PS C:\> Measure-DriveSize | where UsedPercentage -lt 0.7
```

Filter drives by used percentage


## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### MeasureSizeModule.DriveSize

Properties:
* `Drive` - [System.IO.DriveInfo](https://docs.microsoft.com/en-us/dotnet/api/system.io.driveinfo) for given drive
* `UsedSpace` - total used space in bytes
* `FreeSpace` - total free space in bytes
* `TotalSize` - total drive space in bytes (free + used)
* `UsedPercentage` - percentage of used disk space (`0.0` - `1.0`)

## NOTES

## RELATED LINKS
