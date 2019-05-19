function SetVersion ($file, $fileVersion, $packageVersion)
{
    $file = Resolve-Path $file

    Write-Output "Updating $file"
    Write-Output "Setting AssemblyVersion to $fileVersion"
    Write-Output "Setting AssemblyFileVersion to $fileVersion"
    Write-Output "Setting AssemblyInformationalVersion to $packageVersion"

    $sr = new-object System.IO.StreamReader( $file, [System.Text.Encoding]::GetEncoding("utf-8") )
    $content = $sr.ReadToEnd()
    $sr.Close()

    $content = [Regex]::Replace($content, "AssemblyVersion\(""[\d\.]+""\)", "AssemblyVersion(""$fileVersion"")");
    $content = [Regex]::Replace($content, "AssemblyFileVersion\(""[\d\.]+""\)", "AssemblyFileVersion(""$fileVersion"")");
    $content = [Regex]::Replace($content, "AssemblyInformationalVersion\(""[\d\.]+""\)", "AssemblyInformationalVersion(""$packageVersion"")");

    $sw = new-object System.IO.StreamWriter( $file, $false, [System.Text.Encoding]::GetEncoding("utf-8") )
    $sw.Write( $content )
    $sw.Close()
}

SetVersion "Distancify.Migrations.Litium\Properties\AssemblyInfo.cs" $Env:FILEVERSION $Env:PACKAGEVERSION
