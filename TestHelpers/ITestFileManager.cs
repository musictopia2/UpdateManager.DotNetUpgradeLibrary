﻿namespace UpdateManager.DotNetUpgradeLibrary.TestHelpers;
public interface ITestFileManager
{
    bool FileExists(string filePath);
    void CopyFile(string sourcePath, string destinationPath);
    void DeleteFile(string filePath);
}