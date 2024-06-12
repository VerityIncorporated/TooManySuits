# TooManySuits

Installation
-

- Install BepInEx.
- Download and install More Suits.
- Download and install TooManySuits.

Default Controls
-

- Next Page: n
- Back Page: b

These are configurable in the config, located at Lethal Company\BepInEx\config\verity.TooManySuits.cfg

Notice
-

Please report any bugs to me in the Lethal Company Modding discord.

Changelogs
-

### Version 1.0.1
- Fixed text, it now disappears when exiting the ship.
- Text is now moved to a more convenient place.

### Version 1.0.2
- Text is now hovering above the clothing rack, instead of being a UI element.

### Version 1.0.3
- Editable Text-Scale in the config.

### Version 1.0.4
- Bug fixes

### Version 1.0.5
- Visual bug reported by and fixed by Lordfirespeed (off-by-one error)
- Game breaking bug caused by page number not being reset when disconnecting or being kicked, reported by Clark (https://thunderstore.io/c/lethal-company/p/TeamClark/TheMostLethalCompany/)

### Version 1.0.6
- Re-created the asset bundle for the page counter, removing the rectangle which had sizing issues. (this should make the text readable from any resolution also.)
- Updated the More Suits version to 1.4.3!
- VR Support was added 2 Months ago by someone else! You can download it here (Download: https://thunderstore.io/c/lethal-company/p/RT0405/TooManySuitsInWorldButtons/) (Source: https://github.com/RT0405/TooManySuitsInWorldButtons)

### Version 1.0.7
- Added back suit caching which was removed in 1.0.6 causing some performance issues for certain people.
- Removed text component foreach loop to prevent possible performance issues.
- Made it so that the text is only being written to when you change page, and not all the time (whoops).

### Version 1.0.8
- Added some changes suggested by RT0405.
- Added a keybind to manually refresh the suit rack, its default key is K. (this could be automated but I am lazy)

### Version 1.0.9
- Fixed issue where closing a lobby on a suit rack page other than the first could cause loading problems in new lobbies; should resolve broken suit rack on subsequent sessions without needing to restart the game.

### Version 1.1.0
- Complete recoding and reorganization of the mod, making it easier to read from a developer's standpoint and more efficient and of higher quality for the user.
- You no longer need the suitselect asset bundle as its now an embedded resource in the dll.