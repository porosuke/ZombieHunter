日本語の説明は [README_JPN.md](./README_JPN.md) をご覧ください。

# Zombie_Hunter

![header](/header.png)

# Overview

C# shooting game co-produced with bitken1113.

# Requirement

Windows10 / 11

# Usage

To play, download the Game folders and launch shooting.exe.\
Do not rename or delete the text file highscore.txt.\
Reading and writing high scores will no longer be possible.

# Description
## Control
__[Attack]__\
Left mouse click

__[Move]__\
Player : WASD keys\
(up, down, left and right in relation to the screen)\
Aiming: Mouse cursor\
(The player faces the direction of the sight)

__[WeaponsSwitching]__\
Knife : Q\
Handgun : 1\
Shotgun : 2\
Assault rifle : 3\
Sniper rifles : 4\
(You cannot switch between these if you do not have one in your possession)

__[Reload]__\
Reload : R\
(Attack/weapon switching is not possible while reloading)\
(If you reload with ammunition remaining in the magazine, you will only be loaded short of the maximum number of rounds)

__[Debug]__\
Debug Mode : F1

## How to Play

Players are hunters, enemies are zombies.\
The game is about destroying all the zombies.\
Touch the green floor to advance to the next stage.\
If there are surviving zombies on the stage, they are shown in red and cannot be touched.\
Weapons and ammunition can be obtained by touching the icons on the stage.\
Ammunition is not reloaded automatically.Please perform the reload operation.\
Green zombies will attack with their claws if the hunter is within a certain distance.\
Zombies with red edges will fire blood bullets in addition to scratching.\
What is the boss of the final stage?You'll have to wait until you play to find out.\
The player's health is restored in stages in a certain amount of time after being hit.\
If the hunter stands still at this time, the recovery speed increases.\
Knives can slash multiple zombies in a single attack.\
The assault rifle can fire continuously by holding down the attack button.\
Sniper rifle bullets have the property of penetrating zombies.\
Good luck with the game! Aim for a high score!

## Program

__[Game]__
* shooting : Game executable files.
* Highscore : The fastest times are recorded in milliseconds.Strictly forbidden to tamper with!

__[Images]__
* player : Map chip used to draw the player.
* Weapon : Map chip with weapon and item icons.
* zombie_normal : Usually zombie map chip.
* zombie_special : Special zombie map chip.
* zombie_boss : Boss zombie map chip.
* stage : Stage map chip.

__[shooting]__
* shooting.sln : Load the files in the Data folder and create a stage.
Manage and move players, zombies, weapons and bullets.

# Update
Update on May 31, 2025.\
The updates include.

__[Zombie adjustments]__
* In a scratching attack, a decision of 7 damage was processed twice; this has been corrected to a decision of 15 damage once.
* Fixed a problem where the temporarily lose the ability to act while searching for a path.
* The hit detection of a scratching attack is changed to a small circle in front of the zombie instead of a circle centred on the zombie's coordinates.
* Range radius of scratching attacks changed from 40 to 25.
* Individual differences added to movement speed.
* Randomised angles during generation.
* Animation changed to be smoother.
* Additional landmarks on boss zombies.
* Boss zombie behaviour changed to a phase system.

__[Weapon Adjustments]__

_[method]_
* Knife hits are changed to a small circle in front of the player instead of a circle centred on the player's coordinates.
* Knives now hit more than one zombie at a time.
* Shotgun reloads changed from magazine type to one shot at a time (only for effect, firing while reloading is still not possible).
* Changed so that the reload time is reduced according to the number of shotgun rounds loaded.
* Fixed an issue where sniper rifle rounds would duplicate damage inflicted when penetrating an enemy.
* Sniper rifle reloads changed from magazine type to one shot at a time (only for effect, firing while reloading is still not possible).
* Changed so that the reload time is reduced according to the number of sniper rifle rounds loaded.
* Aiming cursor for each weapon changed to be easier to see.
* Aiming cursor changed to lock cursor during weapon change, reloading and firing cool time.

_[Damage, judgement, time, etc.]_

[Knife]
* Attack cooldown time changed to 1.4 seconds.
* Weapon switching animation time changed to 0.25 seconds.
* Attack range radius changed to 25.
* Damage changed to 30.

[Handgun]
* Gunfire cooldown time changed to 0.7 seconds.
* Weapon switching animation time changed to 0.3 seconds.
* Initial ammunition carried changed to 14.
* Initial loadout changed to 6.
* Maximum loadout changed to 8.
* Damage per bullet changed to 10.
* Reload time changed to 2.0 seconds.

[Shotgun]
* Gunfire cooldown time changed to 1.8 seconds.
* Weapon switching animation time changed to 0.3 seconds.
* Initial ammunition carried changed to 6.
* Initial loadout changed to 3.
* Maximum loadout changed to 4.
* Damage per bullet changed to 10.
* Reload time per shot changed to 0.6 seconds.

[AssaultRifle]
* Gunfire cooldown time changed to 0.5 seconds.
* Weapon switching animation time changed to 0.4 seconds.
* Initial ammunition carried changed to 20.
* Initial loadout changed to 12.
* Maximum loadout changed to 20.
* Damage per bullet changed to 14.
* Reload time changed to 3.0 seconds.

[SniperRifle]
* Gunfire cooldown time changed to 2.0 seconds.
* Weapon switching animation time changed to 0.5 seconds.
* Initial ammunition carried changed to 10.
* Initial loadout changed to 2.
* Maximum loadout changed to 5.
* Damage per bullet changed to 20.
* Reload time per shot changed to 0.8 seconds.

__[System tweaks]__
* Improved the accuracy of the coordinate table used for pathfinding.
* Faster generation of pathfinding tables.
* Some processes have been reviewed to reduce the load.
* Changes made to accurately manage game behaviour in terms of elapsed time and maintain 60 FPS.
* Changes made so that game behaviour is not influenced by FPS.
* Additional images for stage floors and walls.
* Recovery items added to stages.
* Recreate all textures.
* Changed to display the amount of change when ammunition is obtained or health is restored on stage.
* Change the amount of ammunition on stage.
* Added the ability to play multiple sounds at the same time.
* Some sound effects added and changed.
* Fixed a problem with the UI display shifting in some environments.
* Switching to debug mode changed to toggle instead of during pressing.
* Added functions such as enemy path information to the debug mode.

# Author

Porosuke\
bitKen1113

# Licence

This project is licensed under the MIT License, see the LICENSE file for details.
