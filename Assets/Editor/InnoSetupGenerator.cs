using System.Diagnostics;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO.Compression;
using Debug = UnityEngine.Debug;
 
namespace Ani_V.EditorTools {
    /// <summary>
    /// Automatically generates an installer build with the help of InnoSetup.
    /// </summary>
    public class InnoSetupGenerator : IPostprocessBuildWithReport {
        public int callbackOrder => 0;
        public void OnPostprocessBuild(BuildReport report) {
            if (report.summary.platform == BuildTarget.StandaloneWindows || report.summary.platform == BuildTarget.StandaloneWindows64) {
                string buildPath = Path.GetDirectoryName(report.summary.outputPath);
                string appName = Path.GetFileNameWithoutExtension(report.summary.outputPath);
                string appVersion = PlayerSettings.bundleVersion;
                string publisherName = "Publisher";
                string appURL = "https://www.example.com/";
                string iconPath = "C:\\Users\\user\\Downloads\\logo multiple star (1).ico";
                string installerOutputPath = Path.Combine(buildPath, "Installers");
 
                if (!Directory.Exists(installerOutputPath)) {
                    Directory.CreateDirectory(installerOutputPath);
                }
 
                string issFilePath = Path.Combine(installerOutputPath, $"{appName}_Installer.iss");
                GenerateInnoSetupScript(issFilePath, buildPath, appName, appVersion, publisherName, appURL, iconPath);
 
                Debug.Log($"Inno Setup script generated at: {issFilePath}");
 
                // Show a popup dialog to decide whether to build the installer or not
                bool buildInstaller = EditorUtility.DisplayDialog(
                    "Build Installer",
                    "Do you want to build the installer now?",
                    "Yes",
                    "No"
                );
 
                if (buildInstaller) {
                    EnsureCorrectFolderStructure(buildPath, appName);
                    BuildInstaller(issFilePath);
                } else {
                    Debug.Log("Installer build skipped. ISS file has been generated.");
                }
                bool createZip = EditorUtility.DisplayDialog(
                    "Create Zip File",
                    "Do you want to create a .zip file for the standalone build?",
                    "Yes",
                    "No"
                );
 
                if (createZip)
                    CreateZip(buildPath, appName);
                else
                    Debug.Log("Zip file creation skipped.");
            }
        }
 
        private void CreateZip(string buildPath, string appName) {
            string zipFilePath = Path.Combine(buildPath, $"{appName}_Standalone.zip");
 
            string[] directoriesToInclude = new string[] {
                Path.Combine(buildPath, "MonoBleedingEdge"),
                Path.Combine(buildPath, $"{appName}_Data"),
                Path.Combine(buildPath, "UnityCrashHandler64.exe"),
                Path.Combine(buildPath, "UnityPlayer.dll")
            };
 
            try {
                using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create)) {
                    foreach (string directory in directoriesToInclude) {
                        if (Directory.Exists(directory)) {
                            AddDirectoryToZip(zip, directory, Path.GetFileName(directory));
                        }
                        else if (File.Exists(directory)) {
                            // If it's a file (like UnityCrashHandler64.exe), add it directly
                            zip.CreateEntryFromFile(directory, Path.GetFileName(directory));
                        }
                        else {
                            Debug.LogWarning($"Directory or file not found: {directory}");
                        }
                    }
                }
                Debug.Log($"Zip file created at: {zipFilePath}");
            }
            catch (Exception ex) {
                Debug.LogError($"Error creating zip file: {ex.Message}");
            }
        }
 
        private void AddDirectoryToZip(ZipArchive zip, string directoryPath, string directoryNameInZip) {
            // Add directory to zip, including its files and subdirectories
            foreach (string file in Directory.GetFiles(directoryPath)) {
                zip.CreateEntryFromFile(file, Path.Combine(directoryNameInZip, Path.GetFileName(file)));
            }
 
            foreach (string subDir in Directory.GetDirectories(directoryPath)) {
                string subDirName = Path.Combine(directoryNameInZip, Path.GetFileName(subDir));
                AddDirectoryToZip(zip, subDir, subDirName); // Recursively add subdirectories
            }
        }
 
        private void GenerateInnoSetupScript(string issFilePath, string buildPath, string appName, string appVersion, string publisherName, string appURL, string iconPath) {
            using (StreamWriter writer = new StreamWriter(issFilePath)) {
                writer.WriteLine("; Script generated by the Inno Setup Script Wizard.");
                writer.WriteLine("; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!");
                writer.WriteLine($"#define MyAppName \"{appName}\"");
                writer.WriteLine($"#define MyAppVersion \"{appVersion}\"");
                writer.WriteLine($"#define MyAppPublisher \"{publisherName}\"");
                writer.WriteLine($"#define MyAppURL \"{appURL}\"");
                writer.WriteLine($"#define MyAppExeName \"{appName}.exe\"");
 
                // Adding the missing lines
                writer.WriteLine($"#define MyAppAssocName MyAppName + \"\"");
                writer.WriteLine("#define MyAppAssocExt \".exe\"");
                writer.WriteLine($"#define MyAppAssocKey StringChange(MyAppAssocName, \"\", \"\") + MyAppAssocExt");
                writer.WriteLine();
 
                writer.WriteLine("[Setup]");
                writer.WriteLine($"AppName={{#MyAppName}}");
                writer.WriteLine($"AppVersion={{#MyAppVersion}}");
                writer.WriteLine($"AppPublisher={{#MyAppPublisher}}");
                writer.WriteLine($"AppPublisherURL={{#MyAppURL}}");
                writer.WriteLine($"AppSupportURL={{#MyAppURL}}");
                writer.WriteLine($"AppUpdatesURL={{#MyAppURL}}");
                writer.WriteLine($"DefaultDirName={{autopf}}\\{{#MyAppName}}");
                writer.WriteLine("ArchitecturesAllowed=x64compatible");
                writer.WriteLine("ArchitecturesInstallIn64BitMode=x64compatible");
                writer.WriteLine("ChangesAssociations=yes");
                writer.WriteLine("DisableProgramGroupPage=yes");
                writer.WriteLine("PrivilegesRequired=lowest");
                writer.WriteLine("PrivilegesRequiredOverridesAllowed=dialog");
                writer.WriteLine($"OutputDir={buildPath}\\Installers");
                writer.WriteLine($"OutputBaseFilename={appName}_Installer");
                writer.WriteLine($"SetupIconFile={iconPath}");
                writer.WriteLine("Compression=lzma");
                writer.WriteLine("SolidCompression=yes");
                writer.WriteLine("WizardStyle=modern");
                writer.WriteLine();
 
                writer.WriteLine("[Languages]");
                writer.WriteLine("Name: \"english\"; MessagesFile: \"compiler:Default.isl\"");
 
                writer.WriteLine("[Tasks]");
                writer.WriteLine("Name: \"desktopicon\"; Description: \"{cm:CreateDesktopIcon}\"; GroupDescription: \"{cm:AdditionalIcons}\"; Flags: unchecked");
                // writer.WriteLine("Name: \"runadmin\"; Description: \"Set Run as Admin\"; GroupDescription: \"Permissions:\"");
 
                writer.WriteLine("[Files]");
                writer.WriteLine($"Source: \"{buildPath}\\{appName}.exe\"; DestDir: \"{{app}}\"; Flags: ignoreversion");
                writer.WriteLine($"Source: \"{buildPath}\\UnityCrashHandler64.exe\"; DestDir: \"{{app}}\"; Flags: ignoreversion");
                writer.WriteLine($"Source: \"{buildPath}\\UnityPlayer.dll\"; DestDir: \"{{app}}\"; Flags: ignoreversion");
                writer.WriteLine($"Source: \"{buildPath}\\MonoBleedingEdge\\*\"; DestDir: \"{{app}}\"; Flags: ignoreversion recursesubdirs createallsubdirs");
                writer.WriteLine($"Source: \"{buildPath}\\{appName}_Data\\*\"; DestDir: \"{{app}}\"; Flags: ignoreversion recursesubdirs createallsubdirs");
                writer.WriteLine();
 
                writer.WriteLine("[Registry]");
                // writer.WriteLine("Root: HKCU; Subkey: \"Software\\Classes\\.myp\"; ValueType: string; ValueName: \"\"; ValueData: \"MyAppAssoc\"; Flags: uninsdeletekey");
                // writer.WriteLine("Root: HKCU; Subkey: \"Software\\Classes\\MyAppAssoc\\DefaultIcon\"; ValueType: string; ValueName: \"\"; ValueData: \"{app}\\MyAppIcon.ico\"");
                writer.WriteLine("Root: HKA; Subkey: \"Software\\Classes\\{#MyAppAssocExt}\\OpenWithProgids\"; ValueType: string; ValueName: \"{#MyAppAssocKey}\"; ValueData: \"\"; Flags: uninsdeletevalue");
                writer.WriteLine("Root: HKA; Subkey: \"Software\\Classes\\{#MyAppAssocKey}\"; ValueType: string; ValueName: \"\"; ValueData: \"{#MyAppAssocName}\"; Flags: uninsdeletekey");
                writer.WriteLine("Root: HKA; Subkey: \"Software\\Classes\\{#MyAppAssocKey}\\DefaultIcon\"; ValueType: string; ValueName: \"\"; ValueData: \"{app}\\{#MyAppExeName},0\"");
                writer.WriteLine("Root: HKA; Subkey: \"Software\\Classes\\{#MyAppAssocKey}\\shell\\open\\command\"; ValueType: string; ValueName: \"\"; ValueData: \"\"\"{app}\\{#MyAppExeName}\"\" \"\"%1\"\"\"");
                // writer.WriteLine("Root: HKA; Subkey: \"Software\\Classes\\Applications\\{#MyAppExeName}\\SupportedTypes\"; ValueType: string; ValueName: \".myp\"; ValueData: \"\"");
                // writer.WriteLine("Root: \"HKLM\"; Subkey: \"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\\"; \\");
                // writer.WriteLine($"ValueType: String; ValueName: \"{{app}}\\{appName}.exe\"; ValueData: \"RUNASADMIN\"; \\");
                // writer.WriteLine("Flags: uninsdeletekeyifempty uninsdeletevalue; MinVersion: 0,6.1; Tasks: \"runadmin\"");
 
                writer.WriteLine("[Icons]");
                writer.WriteLine($"Name: \"{{autoprograms}}\\{{#MyAppName}}\"; Filename: \"{{app}}\\{{#MyAppExeName}}\"");
                writer.WriteLine($"Name: \"{{autodesktop}}\\{{#MyAppName}}\"; Filename: \"{{app}}\\{{#MyAppExeName}}\"; Tasks: desktopicon");
 
                writer.WriteLine("[Run]");
                writer.WriteLine($"Filename: \"{{app}}\\{{#MyAppExeName}}\"; Description: \"Launch {{#MyAppName}}\"; Flags: nowait postinstall");
            }
        }
 
        private void EnsureCorrectFolderStructure(string buildPath, string appName) {
        // Path for MonoBleedingEdge and Appname_Data
            string monoBleedingEdgeFolder = Path.Combine(buildPath, "MonoBleedingEdge");
            string appDataFolder = Path.Combine(buildPath, $"{appName}_Data");
 
            // Ensure correct folder structure for MonoBleedingEdge
            if (Directory.Exists(monoBleedingEdgeFolder)) {
                string targetMonoBleedingEdgeFolder = Path.Combine(monoBleedingEdgeFolder, "MonoBleedingEdge");
 
            // Create the folder if it doesn't exist
            if (!Directory.Exists(targetMonoBleedingEdgeFolder)) {
                Directory.CreateDirectory(targetMonoBleedingEdgeFolder);
            }
 
            // Move files from MonoBleedingEdge folder to MonoBleedingEdge\MonoBleedingEdge
            foreach (string file in Directory.GetFiles(monoBleedingEdgeFolder)) {
                try {
                    string targetFilePath = Path.Combine(targetMonoBleedingEdgeFolder, Path.GetFileName(file));
                    // Check if the file already exists at the destination
                    if (!File.Exists(targetFilePath)) {
                        File.Move(file, targetFilePath);
                    }
                } catch (IOException ex) {
                    UnityEngine.Debug.LogError($"Error moving file {file}: {ex.Message}");
                }
            }
 
            // Move subdirectories to MonoBleedingEdge\MonoBleedingEdge
            foreach (string dir in Directory.GetDirectories(monoBleedingEdgeFolder)) {
                try {
                    string targetDirPath = Path.Combine(targetMonoBleedingEdgeFolder, Path.GetFileName(dir));
                    // Check if the directory already exists at the destination
                    if (!Directory.Exists(targetDirPath)) {
                        Directory.Move(dir, targetDirPath);
                    }
                } catch (IOException ex) {
                    UnityEngine.Debug.LogError($"Error moving directory {dir}: {ex.Message}");
                }
            }
        }
 
        // Ensure correct folder structure for Appname_Data
        if (Directory.Exists(appDataFolder)) {
            string targetAppDataFolder = Path.Combine(appDataFolder, $"{appName}_Data");
 
            // Create the folder if it doesn't exist
            if (!Directory.Exists(targetAppDataFolder)) {
                Directory.CreateDirectory(targetAppDataFolder);
            }
 
            // Move files from Appname_Data folder to Appname_Data\Appname_Data
            foreach (string file in Directory.GetFiles(appDataFolder)) {
                try {
                    string targetFilePath = Path.Combine(targetAppDataFolder, Path.GetFileName(file));
                    // Check if the file already exists at the destination
                    if (!File.Exists(targetFilePath)) {
                        File.Move(file, targetFilePath);
                    }
                } catch (IOException ex) {
                    UnityEngine.Debug.LogError($"Error moving file {file}: {ex.Message}");
                }
            }
 
            // Move subdirectories to Appname_Data\Appname_Data
            foreach (string dir in Directory.GetDirectories(appDataFolder)) {
                try {
                    string targetDirPath = Path.Combine(targetAppDataFolder, Path.GetFileName(dir));
                    // Check if the directory already exists at the destination
                    if (!Directory.Exists(targetDirPath)) {
                        Directory.Move(dir, targetDirPath);
                    }
                } catch (IOException ex) {
                    UnityEngine.Debug.LogError($"Error moving directory {dir}: {ex.Message}");
                }
            }
        }
    }
 
 
        private void BuildInstaller(string issFilePath) {
            string innoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\ISCC.exe";
 
            if (!File.Exists(innoSetupCompilerPath)) {
                Debug.LogError("Inno Setup Compiler not found. Ensure it is installed and the path is correct.");
                return;
            }
 
            try {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = innoSetupCompilerPath,
                    Arguments = $"\"{issFilePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
 
                using (Process process = new Process { StartInfo = startInfo }) {
                    process.OutputDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data)) {
                            Debug.Log($"[Inno Setup] {e.Data}");
                        }
                    };
 
                    process.ErrorDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data)) {
                            Debug.LogError($"[Inno Setup Error] {e.Data}");
                        }
                    };
 
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
 
                    if (process.ExitCode == 0)
                        Debug.Log("Installer built successfully.");
                    else 
                        Debug.LogError("Failed to build the installer. Check the Inno Setup Compiler logs for details.");
                }
            }
            catch (Exception ex) {
                Debug.LogError($"An error occurred while building the installer: {ex.Message}");
            }
        }
    }
}