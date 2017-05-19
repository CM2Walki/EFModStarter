# Company of Heroes: Eastern Front Steam Launcher
### Utilised Libraries
1. [Steamworks.NET API (C#)](https://steamworks.github.io/)
2. [Newtonsoft JSON (C#)](http://www.newtonsoft.com/json)
3. [Lua-io library](https://www.lua.org/manual/5.3/manual.html#6.8)
4. [Lua JSON library](http://lua-users.org/wiki/JsonModules)
5. The latest steam_api.dll

(You can find a copy of those in the EF_Bin folder)

This project was created to implement and use the Steamworks API in a Company of Heroes (**1**) Steam mod. It functions as a communications tool between the game's scripting language SCAR (~Lua) and the actual achievements API. Furthermore, it uses a workaround to display the mod rather than the default Company of Heroes game as currently being played on Steamfriends. It also implements a link to a basic diagnotics DB, that allows us to derive the total amount of crashes per version and the amount of players currently online. 

To fully grasp the structure of the program, we first have to look at the way Steam used to display the currently played game:

d**Old** System (FCFS, the first game to launch is displayed as being played):
1. Start mod _.bat/.exe_ via Steam
2. Steam recognises _.bat/.exe_ as parent process
3. Steamfriends displays the mod

**New** System (the last game to launch is displayed as being played):
1. Start mod _.bat/.exe_ via Steam
2. Company of Heroes (New Steam Version), a Steam game, gets launched
3. Steamfriends displays Company of Heroes (New Steam Version)

e# The Workaround
To tackle this problem, a second .exe (_EFDaemon_) is introduced that restarts the _CoHEF.exe_ after a fixed delay, for it then to reclaim it's spot as the last game launched. In the end we end up with a structure like this:

1. Start _CoHEF.exe_
2. _CoHEF.exe_ launches _RelicCoH.exe_ (with parameters _-mod (...)_ and _-uniquetoken (later used)_ AND _EFDaemon.exe_.
3. _CoHEF.exe_ shuts down
4. Steamfriends displays Company of Heroes (New Steam Version)
5. After a ~30 seconds wait, the _EFDaemon.exe_ restarts the _CoHEF.exe_ with a different parameter, thus taking the place of last game launched.
6. Steamfriends will now display Company of Heroes: Eastern Front

# The Achievements
The launcher uses the [Steamworks.NET API library](https://steamworks.github.io/). This will obviously only work if your .exe is launched via Steam. The _CoHEF.exe_ itself is only responsible for writting changes to Steam that were read from the _pipeline.dat_. It does **not** read directly from the game! The _pipeline.dat_ is written to by SCAR, to make this work you will need both a pure Lua JSON library and the lua-io library (you can find those in the EF_Bin folder).

To activate achievements in a game, simply load the _achievements.scar_ in a gamemode's .scar file and then call _achievements_init()_. This comes in handy if you want to disallow achievements in certain scenarios:

```lua
import("achievements.scar")
function _initdata()
    [...]
    achievements_init()
    [...]
end
```

As stated earlier the .exe parses the JSON from the _pipeline.dat_ and then proceeds to iterate the table to find the status of each achievement. It only checks the file if is has been edited.

Note that you need to place the steam_api.dll in a seperate folder (EF uses EF_Bin). Do **NOT** replace the steam_api.dll from the original installation folder as it will break vCoH!

# Security
This approach is really unsafe, as the player can simply edit the associated .scar files or even the _pipeline.dat_ to quickly unlock all achievements. If one feels fancy, he might want to implement some encryption.

# The WebAPI 
In the C# sourcecode you will find some code regarding simple HTTP requests to our diagnostics database:

```C#
var webClient = new System.Net.WebClient();
string url = "http://me2stats.eu:5020/join?version=" + version_string + "&key=" + auth_key;
webClient.DownloadStringAsync(new Uri(url));
```

It gives us an approximated number of players online, as well as recorded game crashes (so that we can act fast if something breaks). The probability that you might need this code is very low, so just get rid off it. If you are really interested in the webAPI code, just drop me a private message.

### Donate
I did this in my spare time in the interest of all CoH1 mods. If one thinks I deserve a reward for this, then go ahead and follow this button:

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MQGHXU85FDUPW)
