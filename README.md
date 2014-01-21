# GateDiff

A minimalistic file comparison program and shell extension that delegates the
task of comparing files to external programs based on file extension. It
also offers basic file comparison that is geared towards binary files,
with some attention paid to images.


### Why create yet another diffing program?

This project was motivated by [DiffMerge](http://sourcegear.com/diffmerge/), which
is a great program for comparing text files, yet has some annoying flaws. This is
my attempt to hide those flaws.


### Installing shell extension

* Build the solution (you may need to get SharpShell using NuGet)
* Run `/delivery.bat` with wanted program directory as a parameter, e.g. `delivery.bat C:\bin\GateDiff`
* Execute `install.bat` from program directory as _Administrator_, e.g. `C:\bin\GateDiff\install.bat`


### Acknowledgments

* Icons are based on Wikipedia's [Nakayama Torii.svg](http://en.wikipedia.org/wiki/File:Nakayama_Torii.svg)
* Uses [SharpShell](https://github.com/dwmkerr/sharpshell), licensed under
[The Code Project Open License](http://www.codeproject.com/info/cpol10.aspx)
* Config file parsing derived from [Jon Rista's article on Code Project]
(http://www.codeproject.com/Articles/16466/Unraveling-the-Mysteries-of-NET-2-0-Configuration)
* Thanks to David Rickard for [explaining how to save window position in WPF]
(http://blogs.msdn.com/b/davidrickard/archive/2010/03/09/saving-window-size-and-location-in-wpf-and-winforms.aspx)

---
Licensed under [cc by-sa](http://creativecommons.org/licenses/by-sa/3.0/)
