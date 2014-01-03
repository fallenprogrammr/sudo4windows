sudo4windows
============

Attempt to emulate the Unix sudo command in Windows.


The examples of emulating sudo like functionality in windows that I had seen so far with my google / bing - fu were lacking at least for me.

This is my first take on having sudo like functionality in windows, currently it is running on .net framework 4.0, the solution is created using Visual Studio 2013.

This would need some rework to make it more stable, but as of now, it does what I need it to do i.e. gives me a way of running stuff from my command line using an "elevated" mode.

Usage is pretty simple - sudo [command / application].

The code matches the command first to see if it matches any existing shell commands on windows, if it does, then it executes that command using the cmd.exe with a /k option.

If it does not match, the application then attempts to find that application in the directories specified by the path variable [I need to verify if using shellexecute=true will make it unnecessary for not specifying the full path of the executable].

The application is then started using Process, StartInfo provides the required parameters to execute the application in an elevated mode using the verb "runas".

