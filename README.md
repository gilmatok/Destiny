# Destiny

Destiny is a C# based open-source server emulation software for 2D MMORPG game MapleStory. Currently it supports only version v0.83 global edition.

### Requirements
- MapleStory V0.83, global + Localhost
- SQL server, pref. MySQL or WampServer
- Visual Studio, pref. 2017

### Setup Guide
1. **Get Destiny**: Clone the Git repository to a local folder on your machine.
2. **Download and install Visual Studio**: The community version is offered for free on Microsoft's website.
3. **Download and install MySQL**: Install a SQL server of your choice (pref. WampServer).
4. **Check LIB references for Common project**: Check that MoonSharp and MySQL .net DB connector are referenced correctly in the Common project. If not, use NuGet Package Manager to install packages. Right click references -> manage NuGet packages. MySql.Data version 6.10.8.0 and MoonSharp.Interpreter version 2.0.0.0. (Note: VS 2017 and beyond should fetch these automatically when attempting a build.)  
4. **Build the Destiny solution using Visual Studio**
5. **Run Destiny**: Execute the servers in order: WvsCenter -> WvsLogin -> WvsGame(s).

Each server will guide you through the process of configuring it automatically.
