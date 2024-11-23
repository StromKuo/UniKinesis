using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Text.RegularExpressions;

namespace UniKinesisSDK.Editor
{
    public class FileCopyPostProcessor : IPostprocessBuildWithReport
    {
        // Callback order, lower numbers execute earlier
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            // Execute only when the target platform is WebGL
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            // Define source directory path
            var sourceDir = Path.GetFullPath("Packages/com.strodio.unikinesis-sdk/Editor/aws-kinesis-npm-project~/dist");

            // Ensure the source directory exists
            if (!Directory.Exists(sourceDir))
            {
                Debug.LogError($"Source directory does not exist: {sourceDir}");
                return;
            }

            // Find all .js files in the source directory
            var jsFiles = Directory.GetFiles(sourceDir, "*.js");

            if (jsFiles.Length == 0)
            {
                Debug.LogError("No .js files found in the source directory.");
                return;
            }

            // Iterate over all found .js files
            foreach (var sourceFilePath in jsFiles)
            {
                // Get file name
                var jsFileName = Path.GetFileName(sourceFilePath);

                // Define destination file path
                var buildFolder = Path.Combine(report.summary.outputPath, "Build");
                var destFilePath = Path.Combine(buildFolder, jsFileName);

                // Copy .js file to Build folder
                File.Copy(sourceFilePath, destFilePath, true);
                Debug.Log($"Copied {sourceFilePath} to {destFilePath}");

                // Get path of index.html
                var indexPath = Path.Combine(report.summary.outputPath, "index.html");

                if (!File.Exists(indexPath))
                {
                    Debug.LogError($"index.html not found in the build output directory: {indexPath}");
                    return;
                }

                // Read index.html content
                var indexContent = File.ReadAllText(indexPath);

                // Use regex to match existing <script> tags, loosely matching version numbers
                var pattern = @"<script\s+src=""Build/(.+?)aws-kinesis-sdk\.js""></script>";

                // New <script> tag content
                var newScriptTag = $"<script src=\"Build/{jsFileName}\"></script>";

                // Check if reference already exists
                if (Regex.IsMatch(indexContent, pattern))
                {
                    // Replace existing <script> tag
                    indexContent = Regex.Replace(indexContent, pattern, newScriptTag);
                    Debug.Log("Replaced old <script> tag.");
                }
                else
                {
                    // Insert new <script> tag before </body>
                    var bodyCloseTag = "</body>";
                    var insertIndex = indexContent.LastIndexOf(bodyCloseTag);
                    if (insertIndex != -1)
                    {
                        indexContent = indexContent.Insert(insertIndex, $"{newScriptTag}\n");
                        Debug.Log("Inserted new <script> tag.");
                    }
                    else
                    {
                        Debug.LogError("Failed to find </body> tag in index.html.");
                        return;
                    }
                }

                // Write updated content back to index.html
                File.WriteAllText(indexPath, indexContent);
                Debug.Log("index.html updated.");
            }
        }
    }
}
