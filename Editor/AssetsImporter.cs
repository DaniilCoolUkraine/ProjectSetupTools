using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ProjectSetupTools.Editor
{
    public static class AssetsImporter
    {
        [MenuItem("SetupTools/Import Assets")]
        public static void ImportAssets()
        {
            Assets.ImportAssets("Odin Inspector 3.3.1.11.unitypackage", "Sirenix/Editor ExtensionSystem");
            Assets.ImportAssets("Selection History.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
            Assets.ImportAssets("Extenject Dependency Injection IOC.unitypackage", "Mathijs Bakker/Editor ExtensionsUtilities");
        }

        [MenuItem("SetupTools/Import Packages")]
        public static void ImportPackages()
        {
            Packages.InstallPackages(new[]
            {
                "https://github.com/DaniilCoolUkraine/Favourites.git",
                "https://github.com/DaniilCoolUkraine/SimpleEventBus.git",
                "https://github.com/DaniilCoolUkraine/ExtendedHierarchy.git",
                "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
                "https://github.com/DaniilCoolUkraine/SimpleStateMachine.git",
            });
        }

        [MenuItem("SetupTools/Set Root Namespace")]
        public static void SetRootNamespace()
        {
            string projectName = PlayerSettings.productName.Replace(" ", "");

            if (string.IsNullOrEmpty(projectName))
            {
                Debug.LogWarning("Project name is empty. Please set it in PlayerSettings.");
                return;
            }

            EditorSettings.projectGenerationRootNamespace = projectName;

            Debug.Log($"Root Namespace set to \"{projectName}\".");
        }

        public class Assets
        {
            public static void ImportAssets(string asset, string folder)
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var assetsFolder = Path.Combine(basePath, "Unity/Asset Store-5.x");

                AssetDatabase.ImportPackage(Path.Combine(assetsFolder, folder, asset), false);
            }
        }

        [InitializeOnLoad]
        public class Packages
        {
            private const string QueueKey = "ProjectSetupTools_PackageQueue";

            private static AddRequest _request;
            private static Queue<string> _packagesToInstall;

            static Packages()
            {
                _packagesToInstall = LoadQueue();

                if (_packagesToInstall.Count > 0)
                {
                    EditorApplication.update += OnEditorUpdate;
                }
            }

            public static void InstallPackages(string[] packages)
            {
                _packagesToInstall = new Queue<string>(packages);
                SaveQueue();

                EditorApplication.update += OnEditorUpdate;
            }

            private static void OnEditorUpdate()
            {
                if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                    return;

                if (_request != null)
                {
                    if (!_request.IsCompleted)
                        return;

                    if (_request.Status == StatusCode.Success)
                        Debug.Log($"Installed {_request.Result.packageId}");
                    else
                        Debug.LogError($"Failed to install package: {_request.Error.message}");

                    _request = null;
                }

                if (_packagesToInstall.Count > 0)
                {
                    string package = _packagesToInstall.Dequeue();
                    SaveQueue();

                    Debug.Log($"Installing package: {package}");
                    _request = Client.Add(package);
                }
                else
                {
                    EditorApplication.update -= OnEditorUpdate;
                    ClearQueue();
                    Debug.Log("All packages installed.");
                }
            }

            private static void SaveQueue()
            {
                string joined = string.Join("|", _packagesToInstall);
                EditorPrefs.SetString(QueueKey, joined);
            }

            private static Queue<string> LoadQueue()
            {
                string saved = EditorPrefs.GetString(QueueKey, "");
                if (string.IsNullOrEmpty(saved))
                    return new Queue<string>();

                return new Queue<string>(saved.Split('|'));
            }

            private static void ClearQueue()
            {
                EditorPrefs.DeleteKey(QueueKey);
            }
        }

        public class Folders
        {
        }
    }
}