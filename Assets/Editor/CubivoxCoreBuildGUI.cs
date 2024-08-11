using System.Diagnostics;
using System.IO;

using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

/// <summary>
/// Displays the "Core Build" button to build the CubivoxCore and copy it into the Assets folder on demand.
/// (Note: Windows only for now.)
/// 
/// This uses the unity-toolbar-extender library:
/// https://github.com/marijnz/unity-toolbar-extender
/// </summary>
[InitializeOnLoad]
public class CubivoxCoreBuildGUI
{
    static CubivoxCoreBuildGUI()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        var commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 11,
            fixedWidth = 75,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold
        };

        if (GUILayout.Button(new GUIContent("Build Core", "Build Cubivox Core"), commandButtonStyle))
        {
            OnBuildCore();
        }

        GUILayout.FlexibleSpace();
    }

    static void OnBuildCore()
    {
        var buildLocation = Path.Join(Application.dataPath, "CoreBuilds");
        var submodulePath = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "Submodules", "CubivoxCore");
        var buildPath = Path.Combine(submodulePath, "bin", "Debug", "netstandard2.1", "CubivoxCore.dll");

        if(!Directory.Exists(buildLocation))
        {
            Directory.CreateDirectory(buildLocation);
        }

        if(!Directory.Exists(submodulePath))
        {
            EditorUtility.DisplayDialog("Cubivox Core Build Error", "The CubivoxCore submodule directory could not be found! Make sure it really exists.", "Ok");
            UnityEngine.Debug.LogError("Could not find CubivoxCore submodule directory!");
            return;
        }

        if( File.Exists(buildPath) )
        {
            File.Delete(buildPath);
        }

        var buildProcess = new ProcessStartInfo();
        buildProcess.UseShellExecute = false;
        buildProcess.WorkingDirectory = submodulePath;
        buildProcess.FileName = @"C:\Windows\System32\cmd.exe";
        buildProcess.Arguments = "/c dotnet build";
        buildProcess.RedirectStandardOutput = true;

        var process = Process.Start(buildProcess);
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if( !File.Exists(buildPath) )
        {
            UnityEngine.Debug.LogError("Failed to build CubivoxCore!");
            UnityEngine.Debug.LogError(output);
            EditorUtility.DisplayDialog("Cubivox Core Build Error", "Cubivox Core has failed to compile! Check the console for detailed errors!", "Ok");
            return;
        }

        File.Copy(buildPath, Path.Combine(buildLocation, "CubivoxCore.dll"), true);
        UnityEngine.Debug.Log("Successfully built CubivoxCore!");
    }
}
