using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Reflection;

namespace sudo
{
    public class PathHelpers
    {
        //This function is modified from the post on stackoverflow: http://stackoverflow.com/questions/1684392/is-there-a-managed-api-for-kernel32-searchpath
        private static string SearchPath(string pathToSearchIn, string fileNameToSearchFor, string extensionToSearchFor, bool isAnExecutable = false)
        {
            // pathToSearchIn [in, optional] 
            // The path to be searched for the file. 
            // If this parameter is NULL, the function searches for a matching file using a registry-dependent system search path.

            //fileNameToSearchFor [in] 
            //The name of the file for which to search.

            //extensionToSearchFor [in, optional] 
            //The extension to be added to the file name when searching for the file. The first character of the file name extension must be a period (.). The extension is added only if the specified file name does not end with an extension. 

            //If a file name extension is not required or if the file name contains an extension, this parameter can be NULL.

            //isAnExecutable [in, optional]
            //Flag for an executable file search, if the extensionToSearchFor argument is null / empty,

            //Return Value
            //The full path of the file.

            //If the function fails, the return value is null. 

            var pathsToSearch = new List<string>();
            var currentWorkingFolder = Environment.CurrentDirectory;
            var path = Environment.GetEnvironmentVariable("path");

            if (string.IsNullOrEmpty(pathToSearchIn))
            {
                var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager");


                object safeProcessSearchModeObject = key.GetValue("SafeProcessSearchMode");
                if (safeProcessSearchModeObject != null)
                {
                    int safeProcessSearchMode = (int)safeProcessSearchModeObject;
                    if (safeProcessSearchMode == 1)
                    {
                        // When the value of this registry key is set to "1", 
                        // SearchPath first searches the folders that are specified in the system path, 
                        // and then searches the current working folder. 
                        pathsToSearch.AddRange(path.Split(new char[] { Path.PathSeparator }, StringSplitOptions.None));
                        pathsToSearch.Add(currentWorkingFolder);
                    }
                    else
                    {
                        // When the value of this registry entry is set to "0", 
                        // the computer first searches the current working folder, 
                        // and then searches the folders that are specified in the system path. 
                        // The system default value for this registry key is "0".
                        pathsToSearch.Add(currentWorkingFolder);
                        pathsToSearch.AddRange(path.Split(new char[] { Path.PathSeparator }, StringSplitOptions.None));
                    }
                }
                else
                {
                    // Default 0 case
                    pathsToSearch.Add(currentWorkingFolder);
                    pathsToSearch.AddRange(path.Split(new char[] { Path.PathSeparator }, StringSplitOptions.None));
                }
            }
            else
            {
                // Path was provided, use it
                pathsToSearch.Add(pathToSearchIn);
            }

            var foundFile = SearchPath(pathsToSearch, extensionToSearchFor, fileNameToSearchFor, isAnExecutable);
            return foundFile != null ? Path.Combine(foundFile.DirectoryName, foundFile.Name) : null;


        }

        private static FileInfo SearchPath(List<string> paths, string extension, string fileNamePart, bool isAnExecutable)
        {
            var extensionsToSearch = new List<string>();
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    var dir = new DirectoryInfo(path);
                    if (string.IsNullOrEmpty(extension) && isAnExecutable)
                    {
                        AddExecutableExtensions(extensionsToSearch);
                    }
                    else
                    {
                        extensionsToSearch.Add(extension == null ? "" : (extension.StartsWith(".") ? extension : "." + extension));
                    }

                    var fileInfo = dir.GetFiles().Where(file => extensionsToSearch.Contains(file.Extension) && file.Name.Contains(fileNamePart));
                    if (fileInfo.Any())
                        return fileInfo.First();
                }
            }
            return null;
        }

        private static void AddExecutableExtensions(List<string> extensionsToSearch)
        {
            extensionsToSearch.Add(".exe");
            extensionsToSearch.Add(".com");
            extensionsToSearch.Add(".cmd");
            extensionsToSearch.Add(".bat");
            extensionsToSearch.Add(".vbs");
            extensionsToSearch.Add(".vbscript");
            extensionsToSearch.Add(".vbe");
            extensionsToSearch.Add(".vb");
            extensionsToSearch.Add(".bin");
            extensionsToSearch.Add(".cpl");
            extensionsToSearch.Add(".gadget");
            extensionsToSearch.Add(".ins");
            extensionsToSearch.Add(".inx");
            extensionsToSearch.Add(".isu");
            extensionsToSearch.Add(".job");
            extensionsToSearch.Add(".jse");
            extensionsToSearch.Add(".lnk");
            extensionsToSearch.Add(".msc");
            extensionsToSearch.Add(".msi");
            extensionsToSearch.Add(".msp");
            extensionsToSearch.Add(".mst");
            extensionsToSearch.Add(".paf");
            extensionsToSearch.Add(".pif");
            extensionsToSearch.Add(".ps1");
            extensionsToSearch.Add(".reg");
            extensionsToSearch.Add(".rgs");
            extensionsToSearch.Add(".sct");
            extensionsToSearch.Add(".shb");
            extensionsToSearch.Add(".shs");
            extensionsToSearch.Add(".u3p");
            extensionsToSearch.Add(".ws");
            extensionsToSearch.Add(".wsf");
        }

        public static string GetExecutablePath(string arg)
        {
            var path = Path.GetDirectoryName(arg);
            if (path != null && path == "")
            {
                var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
                if (currentDirectory.GetFiles(arg).Length == 0)
                {
                    path = SearchPath("", arg, null, true);
                }
                path = currentDirectory.FullName;
            }
            else
            {
                path = arg;
            }
            return path;
        }
    }
}
