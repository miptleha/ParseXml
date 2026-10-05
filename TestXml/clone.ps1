$sample = "C:\3\xml\usage.xml"
1..10000 | ForEach-Object {
    $target = "test_{0}.xml" -f $_
    if ($_ % 10 -eq 0) {
        $text = [IO.File]::ReadAllText($sample, [Text.Encoding]::UTF8)
        $text = $text -replace 'IPS\.ASG\.007', 'IPS.ASG.999'
        [IO.File]::WriteAllText($target, $text, [Text.Encoding]::UTF8)
    } else {
        Copy-Item $sample $target
    }
}
