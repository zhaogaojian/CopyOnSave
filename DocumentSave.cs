using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ZGJ_CopyOnSave
{
    internal class DocumentSave : IVsRunningDocTableEvents3
    {
        private readonly DTE _dte;
        private readonly RunningDocumentTable _runningDocumentTable;
        public DocumentSave(DTE dte, RunningDocumentTable runningDocumentTable)
        {
            _runningDocumentTable = runningDocumentTable;
            _dte = dte; 
        }

        public int OnBeforeSave(uint docCookie)
        {
            //var document = FindDocument(docCookie);

            //if (document == null)
            //    return VSConstants.S_OK;

            //_documentFormatter.FormatDocument(document);
            //OnAfterSave(docCookie);
            return VSConstants.S_OK;
        }

        private Document FindDocument(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var documentInfo = _runningDocumentTable.GetDocumentInfo(docCookie);
            var documentPath = documentInfo.Moniker;

            return _dte.Documents.Cast<Document>().FirstOrDefault(doc => doc.FullName == documentPath);
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var document = FindDocument(docCookie);

                if (document == null)
                    return VSConstants.S_OK;


                string solutionPath = Directory.GetParent(document.DTE.Solution.FullName).FullName;

                List<string> fileFilter = new List<string>();
                List<string> destDirectory = new List<string>();
                List<string> fromDirectory = new List<string>();
                string cfgFilePath = solutionPath + "\\SaveCopy.cfg";
                if (!File.Exists(cfgFilePath)) return VSConstants.S_OK;
                // 创建一个 StreamReader 的实例来读取文件 
                using (StreamReader sr = new StreamReader(cfgFilePath))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("ExtensionFilter:"))
                        {
                            fileFilter.AddRange(line.Replace("ExtensionFilter:", "").Replace(@"""", "").Split(',').ToList());
                        }
                        if (line.StartsWith("CopyToDirectory:"))
                        {
                            destDirectory.AddRange(line.Replace("CopyToDirectory:", "").Replace(@"""", "").Split(',').ToList());
                        }
                        if (line.StartsWith("FromDirectory:"))
                        {
                            fromDirectory.AddRange(line.Replace("FromDirectory:", "").Replace(@"""", "").Split(',').ToList());
                        }
                    }
                }
                if (fileFilter.Count == 0 || destDirectory.Count == 0)
                {
                    return VSConstants.S_OK;

                }
                //int projectCount = document.DTE.Solution.Projects.Count;
                string srcFile = document.FullName;
                bool bNeedCopy = false;
                foreach (var item in fileFilter)
                {
                    if (srcFile.EndsWith(item))
                    {
                        bNeedCopy = true;
                    }
                }
                if (bNeedCopy)
                {

                    if (fromDirectory.Count > 0)
                    {
                        foreach (string projectPath in fromDirectory)
                        {
                            string projectFolder = projectPath;
                            if (srcFile.StartsWith(projectFolder))
                            {
                                string relativePath = srcFile.Replace(projectFolder, "");

                                foreach (string destPath in destDirectory)
                                {
                                    string destFile = destPath + relativePath;
                                    if (destFile == srcFile) continue;//目标目录中修改自己不拷贝
                                    string destFileDirectory = Path.GetDirectoryName(destFile);
                                    if (!Directory.Exists(destFileDirectory))
                                    {
                                        Directory.CreateDirectory(destFileDirectory);
                                    }
                                    File.Copy(srcFile, destFile, true);
                                }

                            }
                        }
                    }
                    else
                    {
                        foreach (Project projectItem in document.DTE.Solution.Projects)
                        {
                            //var projectItem = document.DTE.Solution.Projects.Item(i + 1);
                            if (projectItem.FullName == "") continue;
                            string projectFolder = Directory.GetParent(projectItem.FullName).FullName + "\\";
                            if (srcFile.StartsWith(projectFolder))
                            {
                                string relativePath = srcFile.Replace(projectFolder, "");

                                foreach (string destPath in destDirectory)
                                {
                                    string destFile = destPath + relativePath;
                                    if (destFile == srcFile) continue;//目标目录中修改自己不拷贝
                                    string destFileDirectory = Path.GetDirectoryName(destFile);
                                    if (!Directory.Exists(destFileDirectory))
                                    {
                                        Directory.CreateDirectory(destFileDirectory);
                                    }
                                    File.Copy(srcFile, destFile, true);
                                }

                            }

                        }
                    }



                }
            }
            catch (System.Exception ex)
            {

                return VSConstants.S_OK;
            }
            
            
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents3.OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld,
            string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents2.OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld,
            string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }
    }
}