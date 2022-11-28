$source = "C:\MeetingsSync\MeetingsSync\bin\Release"
$destination = "C:\MeetingsSync\MeetingsSyncRelease.zip"
Add-Type -assembly "system.io.compression.filesystem"
If(Test-path $destination) {Remove-item $destination}
[io.compression.zipfile]::CreateFromDirectory($source, $destination)
