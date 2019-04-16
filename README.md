# WindowsAudioProfiles

Windows tool to remember different audio settings (volume, balance) on Windows and quicky switch from one to another.

## Visual Studio solution
The solution contains 3 projects:
- **WindowsAudioProfiles:** GUI tool for configuring profiles
- **WindowsAudioProfiles.Console:** Command line tool
- **WindowsAudioProfiles.Entity:** Library project

## How to configure
Run the GUI tool.

![](https://i.imgur.com/Zl0EbFk.png)

## How to switch profiles from the command line

`> WindowsAudioProfiles.CommandLine.exe set [profile name]`