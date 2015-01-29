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


## Known Issues

### New drives not recognized by git bash shell

If you plug in a drive (e.g. USB) while your git bash shell is open, the drive may not be recognized.
When you do an SSH operation (such as `git pull`) you'll get an error message something like this:

```
no such identity: /F/ssh_keys/id_rsa: No such file or directory
Permission denied (publickey).
fatal: Could not read from remote repository.

Please make sure you have the correct access rights
and the repository exists.
```

You have to close **all** git bash windows then re-open for the new drive to be recognized.
(Per [this](http://stackoverflow.com/a/12082487/843431) and [this](http://stackoverflow.com/a/9833065/843431).)

### [New drives not recognized by ssh-agent](https://github.com/pluralsight/git-switch/issues/1)

While the `ssh-agent` is running it may not recognize drive changes,
and won't be able to find your SSH key (giving the same error as above).
You must end the `ssh-agent` process to get past this error.

GitSwitch has an option to "Kill ssh-agent on user change" which, when checked,
will kill all `ssh-agent` processes for you whenever you change users.
This option is not checked by default (but will be remembered if you check it).


## Tips

### Mapping your USB drive to a specific drive letter

If you map a USB drive to a specific drive letter, you may avoid some issues of getting a different drive letter assigned.
To do this:

1. In File Explorer, right-click on 'This-PC' in the left nav
2. Click 'Manage'
3. Select 'Disk Management' under 'Storage'
4. Click 'Change Drive Letter and Paths...'
5. Assign a drive letter and click 'OK'


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
