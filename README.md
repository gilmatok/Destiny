# Destiny

Destiny is an open-source MMORPG server emulation software for the client of MapleStory v0.83.

### Background

I've been a part of the MapleStory development scene for almost 10 years now, and I've developed several server emulators in the past. However, I have yet to take on a massive project such as this and expose it to the community. As I'm making several changes in my life, I'd like to make changes in my software development habbits and this is the proof.

### Setup Guide
1. **Get Destiny**
Clone the Git repoistory to a local folder on your machine.
2. **Download and install Visual Studio 2015**
3. **Download and install MySQL**
4. **Build the Destiny solution provided with Visual Studio 2015**
5. **Run Destiny**

The server will guide you through the process of configuring Destiny automatically.

### Design

* [**Server Architecture**] - Destiny unifies all server types under one assembly to ensure stability and ease of use.
* [**Game Data**] - Destiny uses MCDB for MapleStory related data storage access.
* [**Database Managment**] - Destiny uses MySQL for database managment along with the .NET MySQL connector.
* [**Scripting Interface**] - Yet to be decided.
