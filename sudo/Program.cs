using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace sudo
{
    internal class Program
    {
        private static readonly List<string> ShellCommands = new List<string>
        {
            "assoc",
            "attrib",
            "break",
            "bcdedit",
            "cacls",
            "call",
            "cd",
            "chcp",
            "chdir",
            "chkdsk",
            "chkntfs",
            "cls",
            "cmd",
            "color",
            "comp",
            "compact",
            "convert",
            "copy",
            "date",
            "del",
            "dir",
            "diskcomp",
            "diskcopy",
            "diskpart",
            "doskey",
            "driverquery",
            "echo",
            "endlocal",
            "erase",
            "exit",
            "fc",
            "find",
            "findstr",
            "for",
            "format",
            "fsutil",
            "ftype",
            "goto",
            "gpresult",
            "graftabl",
            "help",
            "icacls",
            "if",
            "label",
            "md",
            "mkdir",
            "mklink",
            "mode",
            "more",
            "move",
            "openfiles",
            "path",
            "pause",
            "popd",
            "print",
            "prompt",
            "pushd",
            "rd",
            "recover",
            "rem",
            "ren",
            "rename",
            "replace",
            "rmdir",
            "robocopy",
            "set",
            "setlocal",
            "sc",
            "schtasks",
            "shift",
            "shutdown",
            "sort",
            "start",
            "subst",
            "systeminfo",
            "tasklist",
            "taskkill",
            "time",
            "title",
            "tree",
            "type",
            "ver",
            "verify",
            "vol",
            "xcopy",
            "wmic"
        };


        private static void Main(string[] args)
        {
            var p = new Process();
            var si = new ProcessStartInfo();
            if (ShellCommands.Count(a => a.Equals(args[0], StringComparison.InvariantCultureIgnoreCase)) > 0)
            {
                si.FileName = "cmd.exe";
                si.Arguments = "/k " + args[0] + " " + args.ToArgumentString();
                si.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                var executablePath = PathHelpers.GetExecutablePath(args[0]);
                if (string.IsNullOrEmpty(executablePath))
                {
                    Console.WriteLine("The executable file for " + args[0] + " could not be found.");
                    return;
                }
                else
                {
                    si.FileName = executablePath;
                    si.Arguments = args.ToArgumentString();
                    si.WorkingDirectory = Path.GetDirectoryName(si.FileName);
                }
            }
            si.UseShellExecute = true;
            si.Verb = "runas";
            p.StartInfo = si;
            try
            {
                p.Start();
            }
            catch (Win32Exception e)
            {
                if (!e.Message.Contains("The operation was canceled by the user"))
                {
                    Console.WriteLine("");
                }
            }
        }
    }
}
