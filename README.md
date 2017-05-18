# Company of Heroes: Eastern Front Steam Launcher
This project was created to implement and use the Steamworks API in a Company of Heroes (**1**) Steam mod. It functions as a communications tool between the game's scripting language SCAR (~Lua) and the actual achievements API. Furthermore, it uses a workaround to display the mod rather than the default Company of Heroes game as currently being played on Steamfriends.
To fully grasp the structure of the program, we first have to look at the way Steam used to display the currently played game:

**OLD** System (FCFS, the first game to launch is displayed as being played):
1. Start mod _.bat/.exe_ via Steam
2. Steam recognises _.bat/.exe_ as parent process
3. Steamfriends displays the mod

**New** System (the last game to launch is displayed as being played):
1. Start mod _.bat/.exe_ via Steam
2. Company of Heroes (New Steam Version), a Steam game gets launched
3. Steamfriends displays = Company of Heroes (New Steam Version)

# The Workaround
To tackle this problem, a second .exe (_EFDaemon_) is introduced that restarts the _CoHEF.exe_ after a fixed delay, for it then to reclaim it's spot as the last game launched. In the end we end up with a structure like this:

1. Start _CoHEF.exe_
2. _CoHEF.exe_ launches _RelicCoH.exe_ (with parameters _-mod (...)_ and _-uniquetoken (later used)_ AND _EFDaemon.exe_.
3. _CoHEF.exe_ shuts down
4. Steamfriends displays = Company of Heroes (New Steam Version)
5. After a ~30 seconds wait, the _EFDaemon.exe_ restarts _CoHEF.exe_ with a different parameter.
