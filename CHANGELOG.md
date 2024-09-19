- Unreleased
  - Removed dependency on InputUtils from manifest.

- 2.0.0
  - Implemented in-world clickable buttons.
  - Removed keybinds.
  - Fixed a bug where the rack wouldn't get updated properly if a suit was bought from the terminal.
  - Fixed centering of label.
  - Changed font of label to VGA 437.
  - The label now properly reacts to lighting and disappears when lights are turned off.
  - Refactored and simplified code internals.
  - Removed dependency on MoreSuit's `MakeSuitsFitOnRack` setting.
    The gap between suits is now calculated exclusively using our `SuitsPerPage` setting.

- 1.1.2
  - Fixed a bug which spammed the console with NullReferenceException's. (Verity)
  - Made LobbyCompatibility a soft dependency. (1A3) (https://github.com/1A3Dev) (https://thunderstore.io/c/lethal-company/p/Dev1A3/)

- 1.1.1
  - Fixed a bug where equipping a suit wouldn't get networked if other players hadn't visited all pages prior to equipping.
  - Fixed a bug where the text above the rack would appear at the landing location before the ship finished landing.
  - Fixed a bug where the text above the rack would stay at the landing location after taking off.
  - Removed assembly reference on MoreSuits (now accessed via reflection).
  - Added support for LobbyCompatibility
  - All changes listed were done by peelz (https://github.com/notpeelz) (https://thunderstore.io/c/lethal-company/p/notpeelz/)

- 1.1.0
  - Complete recoding and reorganization of the mod, making it easier to read from a developer's standpoint and more efficient and of higher quality for the user.
  - You no longer need the suitselect asset bundle as its now an embedded resource in the dll.

- 1.0.9
  - Fixed issue where closing a lobby on a suit rack page other than the first could cause loading problems in new lobbies; should resolve broken suit rack on subsequent sessions without needing to restart the game.

- 1.0.8
  - Added some changes suggested by RT0405.
  - Added a keybind to manually refresh the suit rack, its default key is K. (this could be automated but I am lazy)

- 1.0.7
  - Added back suit caching which was removed in 1.0.6 causing some performance issues for certain people.
  - Removed text component foreach loop to prevent possible performance issues.
  - Made it so that the text is only being written to when you change page, and not all the time (whoops).

- 1.0.6
  - Re-created the asset bundle for the page counter, removing the rectangle which had sizing issues. (this should make the text readable from any resolution also.)
  - Updated the More Suits version to 1.4.3!
  - VR Support was added 2 Months ago by someone else! You can download it here (Download: https://thunderstore.io/c/lethal-company/p/RT0405/TooManySuitsInWorldButtons/) (Source: https://github.com/RT0405/TooManySuitsInWorldButtons)

- 1.0.5
  - Visual bug reported by and fixed by Lordfirespeed (off-by-one error)
  - Game breaking bug caused by page number not being reset when disconnecting or being kicked, reported by Clark (https://thunderstore.io/c/lethal-company/p/TeamClark/TheMostLethalCompany/)

- 1.0.4
  - Bug fixes

- 1.0.3
  - Editable Text-Scale in the config.

- 1.0.2
  - Text is now hovering above the clothing rack, instead of being a UI element.

- 1.0.1
  - Fixed text, it now disappears when exiting the ship.
  - Text is now moved to a more convenient place.

- 1.0.0
  - Initial release
