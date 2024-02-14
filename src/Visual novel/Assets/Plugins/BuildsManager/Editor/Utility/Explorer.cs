using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace BuildsManager.Utility
{
    public static class Explorer
    {
        public static void ShowExplorer(string itemPath)
        {
            itemPath = itemPath.Replace(@"/", @"\"); // Explorer не любит косые черты спереди

            var findFile = false;

            var di = new DirectoryInfo(itemPath);

            foreach (var fi in di.GetFiles())
            {
                if (fi.Name is "." or ".." or "Thumbs.db")
                {
                    continue;
                }

                itemPath = fi.FullName;
                findFile = true;
                break;
            }

            if (!findFile)
            {
                foreach (var fi in di.GetDirectories())
                {
                    if (fi.Name is "." or ".." or "Thumbs.db")
                    {
                        continue;
                    }

                    itemPath = fi.FullName;
                    break;
                }
            }

            Process.Start("explorer.exe", "/select," + itemPath);
        }

        public static void CreateAllFoldersBeforePath(string path)
        {
            var dirs = ("Assets/" + path).Split('/');
            var allPath = dirs[0];
            for (var i = 1; i < dirs.Length - 1; ++i)
            {
                if (!AssetDatabase.IsValidFolder(allPath + "/" + dirs[i]))
                {
                    AssetDatabase.CreateFolder(allPath, dirs[i]);
                }

                allPath = allPath + "/" + dirs[i];
            }
        }
    }
}