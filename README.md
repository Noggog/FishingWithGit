# FishingWithGit
A git wrapper with the intention of extending hook functionality

This is a project aimed at offering a full suite of git hooks that can be leveraged by users for other projects.  It adds a few features over standard git:
* Adds hooks for most (eventually hopefully all) commands.
* Adds both sides (pre and post) to every hook.
* Allows hooks to be put inside the working directory, and thus committed if desired.
* Allows EXEs as hooks.  (pre-commit.exe)
* Allows an EXE or script to be called on every hook event.  These programs can then decide which hooks to act on.

This project was developed initially to support development for [HarmonizeGit](https://github.com/Noggog/HarmonizeGit).

# Currently Supported Hooks
* Branch creation/deletion
* Checkout
* Cherry Pick
* Commit
* CommitMsg
* Merge
* Pull
* Rebase
* Reset
* Status
* Take (a new command type.  Reset and checkout commands that aim to simply remove a list of modified files in the working directory funnel into this)

All commands except CommitMsg support pre and post firing.

# Creating a program to receive all hook events
Any exe that is not named after a hook (pre-commit.exe) will be treated as a common receiver.  It will be invoked and started on every hook
event, and passed information on what hook is being called, as well as any specialized args.

# Install
The install is not perfect at this point in time.  Currently it is an exe wrapper for git.exe.  It is intended to be called instead of 
git.exe, so it can call the appropriate hooks before/after git's normal operations.

My current install consists of inserting it in place of git.exe, to function as a wrapper for anyone looking to call git.exe.  
The original git.exe setup is moved into a subfolder, while the FishingWithGit.exe is renamed and put in git.exe's place.
The configuration for FishingWithGit then needs to be modified to match the subfolder the actual git.exe is located in.

With this in place, any caller looking to use git will instead call Fishing With Git, which will call the appropriate hooks in addition to the 
normal git calls.  This means any GUI you may use needs to be switched off embedded mode, so that it uses the installed git.exe (and thus
FishingWithGit).

# Installing In The Future
I hope to figure out a way to not have to literally boot git.exe out of its place.  This might come from environment variable modifications.
I also hope to create an installer when things get stable enough.

# Other Disclaimers
Overall, this project is still in a highly experimental state, and is not guarenteed to work with any setup.  Right now it works well with
my current setup involving SourceTree on windows.
