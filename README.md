# Destiny

Destiny is a C# based open-source server emulation software for 2D MMORPG game MapleStory. Currently it supports only version v0.83 global edition.

### Requirements
- MapleStory V0.83, global + Localhost
- SQL server, pref. MySQL or WampServer
- Visual Studio, pref. 2015 or 2017

### Setup Guide
1. **Get Destiny**: Clone the Git repository to a local folder on your machine.
2. **Download and install Visual Studio**: The community version is offered for free on Microsoft website.
3. **Download and install MySQL**: Install a SQL server of your choice (pref. WampServer).
4. **Check LIB references for project**: Check that MoonSharp and MySQL .net DB connector are referenced rightly, if not use NutGet manager to install packages. Right click references -> manage NuGet packages, MySql.Data version 6.10.6.0 and MoonSharp.Interpreter version 2.0.0.0.  
4. **Build the Destiny solution provided with Visual Studio**
5. **Run Destiny**: Execute the servers in order: WvsCenter -> WvsLogin -> WvsGame(s).

Each server will guide you through the process of configuring it automatically.

**Warning: Two auto-setup options currently dont function as intented.**
- If login for first time with autoregister function set you will have to login for 2 times consequtively. On the fist time you wont be recognized and thefore on second attemp you will be autoregisterted and loged in.

- The number of channels selected on login server setup is not yet implemented. So even if you select 16 channels there will be only 1
