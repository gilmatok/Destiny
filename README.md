# Destiny

Destiny is a C# based open-source server emulatation software for 2D MMORPG game Maplestory. Currently it supports only version v0.83 global edition.

### Requirements
- Maplestory V0.83, global + Localhost
- SQL server, pref. MySQL or WampServer
- Visual Studio, pref. 2015 or 2017

### Setup Guide
1. **Get Destiny**: Clone the Git repoistory to a local folder on your machine.
2. **Download and install Visual Studio**: The community version is offered for free on Microsoft website.
3. **Download and install MySQL**: Install a SQL server of your choice (pref. WampServer).
4. **Check LIB references for project**: Check that MoonSharp and MySQL .net DB connector are referenced rightly, if not use NutGet manager to install packages. Right click references -> manage NuGet packages, MySql.Data version 6.10.6.0 and MoonSharp.Interpreter version 2.0.0.0.  
4. **Build the Destiny solution provided with Visual Studio**
5. **Run Destiny**: Execute the servers in order: WvsCenter -> WvsLogin -> WvsGame(s).

Each server will guide you through the process of configuring it automatically.

**Warning: Two auto-setup options currently dont function well.**
- The autoregister function will get stuck you on login screen and after reset due to generation of wrong hash will claim wrong       password. Simply generate SHA512 for you pass and throw it manualy into DB as password or comment out the password check at all its  at LoginClient.cs.

- The number of channels selected on login server setup is not yet implemented. So even if you select 16 channels there will be only 1
