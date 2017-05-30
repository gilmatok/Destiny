# Destiny

Destiny is an open-source MMORPG server emulation software for the client of MapleStory v0.83.

### Background

I've been a part of the MapleStory development scene for almost 10 years now, and I've developed several server emulators in the past. However, I have yet to take on a massive project such as this and expose it to the community. As I'm making several changes in my life, I'd like to make changes in my software development habbits and this is the proof.

### Setup Guide
1. **Get Destiny**
Clone the Git repoistory to a local folder on your machine.
2. **Download and install Visual Studio 2015**
...
3. **Download and install MySQL**
...
4. **Run the Destiny SQL script**
...
5. **Modify Destiny.xml to your preference**
...
6. **Build the Destiny solution provided with Visual Studio 2015**
...
7. **Run WZ2BIN [MapleStory directory] [Data directory]**
WZ2BIN will convert MapleStory WZ files to custom data binary files that Destiny uses.
8. **Run Destiny**

The server should be running at this point.

### Design

* [**Server Architecture**] - Destiny unifies all server types under one assembly to ensure stability and ease of use.
* [**Game Data**] - Destiny uses WZ2BIN, an application that converts MapleStory WZ data files into custom binary data files.
* [**Database Managment**] - Destiny uses MySQL for database managment along with the .NET MySQL connector.
* [**Scripting Interface**] - Yet to be decided.
