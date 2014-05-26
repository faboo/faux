About
=====

Faux is a project manager, but not of the usual sort. In Faux, you manage the
files of a single project at a time. You can think of it like the Workspace pane
of Eclipse or the Solution Explorer of Visual Studio.

The main window hosts a file tree. The folders in the tree virtual (faux)
folders, but the files they contain are files that exist on disk under the
folder that the current project is in. These faux folders and the files within
them can be nested and arranged arbitrarily without affecting the actual file
structure on disk.

By default, double clicking a file or launching it from the right-click menu,
will launch the file in the default program for that file type. Additionally,
Faux file associations can be made from the Types Editor, opened from the File
menue "Edit Types". In the Types Editor, new file types can be created to change
the program that is launched by default for that type.


Building
--------

You will need Visual Studio 2010 to build Faux. I use the Express version, but
any of the versions with C# support should work fine.

There is no installer at the moment, so you will need to copy the contents of
the Project\bin\Release directory to an appropriate place.


Running
-------

I recommend setting Faux as the default program for .project files, but you can
also start Faux from Explorer or the Start Menu and drag a project file into it.

You'll note that Faux has no "new project" command at the moment. To create a
new project, simply create a new, empty file, and open it in Faux. Faux will
take care of the rest.


Project Files and Other Files
-----------------------------

Every project (even empty ones) has two folders called "Project Files and "Other
Files".

The Project Files folder contains all the files and faux folders you've added to
your project.

The Other Files folder contains all of the files not currently in other faux
folders. Other files or sub-folders can be moved to a faux folder (such as the
Project Files folder) by dragging them around the file tree.

Files moved into the Other Files folder will be forgotten, and no longer managed
as part of the project.

Additionally, files may be dragged into Faux directly from Explorer.


Types
-----

Types created in the Types Editor have attributes: their Name, defining Pattern,
associated Launcher program, the Arguments to the launcher, and an Alternate
Icon.

The Name attribute identifies the file type.

The Pattern attribute is a regular expression that should match the names of
files of this type. For instance, a pattern of "\.jpe?g$" (without the quotes)
would match Jpeg files.

The Launcher attribute is the full path to the program that will be used to
handle the Launch command or double clicking.

The Arguments attribute is the commandline arguments to be passed to the
launcher. This should probably at least include the {FilePath} macro (click the
question mark to see the other macros).

The Alternate Icon attribute is the path to the icon file to use to represent
files of this type. If left unset, the default is used (usually the launcher's
icon).


Planned Features
----------------

* Multi-select in the file tree

* More uniform icon set

* Integration with an icon web service for choosing appropriate icons
 - DONE!

* Renaming the application and executable to "Faux"
