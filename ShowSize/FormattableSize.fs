namespace ShowSizeModule

type SizeUnit =
    | Terabytes of double
    | Gigabytes of double
    | Megabytes of double
    | Kilobytes of double
    | Bytes of int64

[<Struct>]
type FormattableSize(sizeInBytes: int64) =
    member _.SizeUnit =
        if sizeInBytes >= (1024L * 1024L * 1024L * 1024L) then
            Terabytes ((double sizeInBytes) / (1024.0 * 1024.0 * 1024.0 * 1024.0))
        else if sizeInBytes >= (1024L * 1024L * 1024L) then
            Gigabytes ((double sizeInBytes) / (1024.0 * 1024.0 * 1024.0))
        else if sizeInBytes >= (1024L * 1024L) then
            Megabytes ((double sizeInBytes) / (1024.0 * 1024.0))
        else if sizeInBytes >= (1024L) then
            Kilobytes ((double sizeInBytes) / 1024.0)
        else 
            Bytes sizeInBytes

    member this.ToColoredString() =
        match this.SizeUnit with
        | Terabytes tb -> sprintf "\x1b[1;91m%.2fT\x1b[0m" tb
        | Gigabytes gb -> sprintf "\x1b[1;95m%.2fG\x1b[0m" gb
        | Megabytes mb -> sprintf "\x1b[1;93m%.2fM\x1b[0m" mb
        | Kilobytes kb -> sprintf "\x1b[1;92m%.2fK\x1b[0m" kb
        | Bytes b -> sprintf "\x1b[1;97m%iB\x1b[0m" b
        
    override this.ToString() =
        match this.SizeUnit with
        | Terabytes tb -> sprintf "%.2fT" tb
        | Gigabytes gb -> sprintf "%.2fG" gb
        | Megabytes mb -> sprintf "%.2fM" mb
        | Kilobytes kb -> sprintf "%.2fK" kb
        | Bytes b -> sprintf "%iB" b

