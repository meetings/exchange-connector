cd C:\MeetingsSync
del \\tsclient\rds\run
powershell "& ""C:\MeetingsSync\zipRelease.ps1"""
dir C:\MeetingsSync\MeetingsSyncRelease.zip
powershell "& ""date"""
copy C:\MeetingsSync\MeetingsSyncRelease.zip \\tsclient\rds\MeetingsSyncRelease.zip
echo 1 > \\tsclient\rds\run
pause
