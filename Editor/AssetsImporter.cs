using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ProjectSetupTools.Editor
{
    public static class AssetsImporter
    {
        [MenuItem("Setup Tools/Import Assets")]
        public static void ImportAssets()
        {
            Assets.ImportAssets("Odin Inspector 3.3.1.11.unitypackage", "Sirenix/Editor ExtensionSystem");
            Assets.ImportAssets("Selection History.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
            Assets.ImportAssets("Extenject Dependency Injection IOC.unitypackage", "Mathijs Bakker/Editor ExtensionsUtilities");
        }

        [MenuItem("Setup Tools/Import Packages")]
        public static void ImportPackages()
        {
            Packages.InstallPackages(new[]
            {
                "https://github.com/DaniilCoolUkraine/Favourites.git",
                "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
                "https://github.com/DaniilCoolUkraine/SimpleEventBus.git"
            });
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

        public class Packages
        {
            private static AddRequest request;
            static Queue<string> packagesToInstall = new Queue<string>();

            public static void InstallPackages(string[] packages)
            {
                foreach (var package in packages)
                    packagesToInstall.Enqueue(package);

                if (packagesToInstall.Count > 0)
                {
                    StartNextPackageInstall();
                }
            }

            private static async void StartNextPackageInstall()
            {
                request = Client.Add(packagesToInstall.Dequeue());
                while (!request.IsCompleted) await Task.Delay(10);

                if (request.Status == StatusCode.Success)
                    Debug.Log($"Installed {request.Result.packageId}");
                else
                    Debug.LogError($"{request.Error.message}");

                if (packagesToInstall.Count > 0)
                {
                    await Task.Delay(1000);
                    StartNextPackageInstall();
                }
            }
        }

        public class Folders
        {
        }
    }
}