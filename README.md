# ![GitSwitch Logo](/gitswitch64x64.png) GitSwitch

This is a Windows utility that allows multiple users to share a pairing station while using their own
git `user.name`, `user.email` for commits and SSH key for interacting with GitHub.
It will update the `~/.gitconfig` and `~/.ssh/config` files on your machine.


## Running GitSwitch

When you run the `.exe`, GitSwitch will put an icon in the taskbar tray.
Right-click on it for options to:

* Edit (add / update / remove) user info.
* Select a user; this will configure the machine with their info.

For improved security, you can keep your SSH key on a removable drive.
If the drive letter changes (or if the SSH key file changes for any reason), GitSwitch will warn you about the problem.
You will need to use the "Edit Users..." option to update the path to your SSH key.

**Important Note:**
GitSwitch expects your unix home (`~`) folder to be the same as your Windows User Profile folder (e.g. `C:\Users\myuser`).
If this is not the case, GitSwitch may change config files which are not in use by git/ssh.


## Working on GitSwitch

Please remember to bump the version number (using [semantic versioning rules](http://semver.org/))
in the `AssemblyInfo.cs` file for issue tracking purposes.


## Q&A

* **Q:** Will GitSwitch clobber my `~/.gitconfig` or `~/.ssh/config` files?
* **A:** Maybe. But it isn't supposed to. You may want to backup your config files before using.
    Please create an issue with an example config file if you encounter this problem.

* **Q:** What's with that icon?
* **A:** Yeah, I know; I'm a programmer.
    It's supposed to be the [git (bash) logo](http://git-scm.com/) combined with a user icon.
