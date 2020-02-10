using System;
using System.Collections.Generic;
using System.Linq;

using R5T.Cambridge.Types;
using R5T.Soltana.Extensions;
using R5T.Solutas;

using InMemorySolutionFileOperator = R5T.Soltana.IVisualStudioSolutionFileOperator;


namespace R5T.Solitude.Soltana
{
    public class VisualStudioSolutionFileOperator : IVisualStudioSolutionFileOperator
    {
        private InMemorySolutionFileOperator InMemorySolutionFileOperator { get; }
        private IVisualStudioSolutionFileSerializer VisualStudioSolutionFileSerializer { get; }


        public VisualStudioSolutionFileOperator(
            InMemorySolutionFileOperator inMemorySolutionFileOperator,
            IVisualStudioSolutionFileSerializer visualStudioSolutionFileSerializer)
        {
            this.InMemorySolutionFileOperator = inMemorySolutionFileOperator;
            this.VisualStudioSolutionFileSerializer = visualStudioSolutionFileSerializer;
        }

        private void ModfifySolutionFile(string solutionFilePath, Action<SolutionFile> solutionFileAction)
        {
            var solutionFile = this.VisualStudioSolutionFileSerializer.Deserialize(solutionFilePath);

            solutionFileAction(solutionFile);

            this.VisualStudioSolutionFileSerializer.Serialize(solutionFilePath, solutionFile);
        }

        private T ModfifySolutionFile<T>(string solutionFilePath, Func<SolutionFile, T> solutionFileFunction)
        {
            var solutionFile = this.VisualStudioSolutionFileSerializer.Deserialize(solutionFilePath);

            var output = solutionFileFunction(solutionFile);

            this.VisualStudioSolutionFileSerializer.Serialize(solutionFilePath, solutionFile);

            return output;
        }

        public void AddProjectFile(string solutionFilePath, string projectFilePath, Guid projectTypeGuid, Guid projectGuid)
        {
            this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                this.InMemorySolutionFileOperator.AddProjectFile(solutionFile, solutionFilePath, projectFilePath, projectTypeGuid, projectGuid);
            });
        }

        public void CreateNewSolutionFile(string solutionFilePath)
        {
            var solutionFile = this.InMemorySolutionFileOperator.CreateNewSolutionFile();

            this.VisualStudioSolutionFileSerializer.Serialize(solutionFilePath, solutionFile);
        }

        public bool HasProjectFile(string solutionFilePath, string projectFilePath)
        {
            var solutionFile = this.InMemorySolutionFileOperator.CreateNewSolutionFile();

            var hasProjectFile = this.InMemorySolutionFileOperator.HasProjectFile(solutionFile, solutionFilePath, projectFilePath, out _);
            return hasProjectFile;
        }

        public IEnumerable<string> ListProjectReferenceFilePaths(string solutionFilePath)
        {
            var solutionFile = this.VisualStudioSolutionFileSerializer.Deserialize(solutionFilePath);

            var projectReferenceFilePaths = this.InMemorySolutionFileOperator.ListProjectFilePaths(solutionFile, solutionFilePath);
            return projectReferenceFilePaths;
        }

        public bool RemoveProjectFile(string solutionFilePath, string projectFilePath)
        {
            var success = this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                var output = this.InMemorySolutionFileOperator.RemoveProjectFile(solutionFile, solutionFilePath, projectFilePath);
                return output;
            });

            return success;
        }

        public void AddSolutionFolder(string solutionFilePath, string solutionFolderPath)
        {
            this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                this.InMemorySolutionFileOperator.AddSolutionFolder(solutionFile, solutionFolderPath);
            });
        }

        public bool HasSolutionFolder(string solutionFilePath, string solutionFolderPath)
        {
            var hasSolutionFolder = this.ModfifySolutionFile(solutionFolderPath, (solutionFile) =>
            {
                var output = this.InMemorySolutionFileOperator.HasSolutionFolder(solutionFile, solutionFolderPath, out _);
                return output;
            });

            return hasSolutionFolder;
        }

        public bool RemoveSolutionFolderAndContents(string solutionFilePath, string solutionFolderPath)
        {
            var success = this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                var output = this.InMemorySolutionFileOperator.RemoveSolutionFolderAndContents(solutionFile, solutionFolderPath);
                return output;
            });

            return success;
        }

        public void MoveProjectFileIntoSolutionFolder(string solutionFilePath, string projectFilePath, string solutionFolderPath)
        {
            this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                this.InMemorySolutionFileOperator.MoveProjectFileIntoSolutionFolder(solutionFile, solutionFilePath, projectFilePath, solutionFolderPath);
            });
        }

        public void MoveProjectFileOutOfSolutionFolder(string solutionFilePath, string projectFilePath, string solutionFolderPath)
        {
            this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                this.InMemorySolutionFileOperator.MoveProjectFileOutOfSolutionFolder(solutionFile, solutionFilePath, projectFilePath, solutionFolderPath);
            });
        }

        public IEnumerable<string> ListSolutionFolderProjectFilePaths(string solutionFilePath, string solutionFolderPath)
        {
            var projectFiles = this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                var projectFileReferences = this.InMemorySolutionFileOperator.ListSolutionFolderProjectFiles(solutionFile, solutionFilePath, solutionFolderPath);

                var output = projectFileReferences.Select(x => x.ProjectFilePathValue).ToArray();
                return output;
            });

            return projectFiles;
        }

        public IEnumerable<string> ListSolutionFolderSolutionFolderNames(string solutionFilePath, string solutionFolderPath)
        {
            var solutionFolderNames = this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                var solutionFolderProjects = this.InMemorySolutionFileOperator.ListSolutionFolderSolutionFolders(solutionFile, solutionFolderPath);

                var output = solutionFolderProjects.Select(x => x.ProjectName);
                return output;
            });

            return solutionFolderNames;
        }

        public IEnumerable<string> ListRootSolutionFolderNames(string solutionFilePath)
        {
            var rootSolutionFolderNames = this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                var rootSolutionFolders = this.InMemorySolutionFileOperator.ListRootSolutionFolders(solutionFile);

                var output = rootSolutionFolders.Select(x => x.ProjectFileRelativePathValue);
                return output;
            });

            return rootSolutionFolderNames;
        }

        public IEnumerable<string> ListRootProjectFilePaths(string solutionFilePath)
        {
            var rootProjectFilePaths = this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                var rootProjectReferences = this.InMemorySolutionFileOperator.ListRootProjectFiles(solutionFile, solutionFilePath);

                var output = rootProjectReferences.Select(x => x.ProjectFilePathValue).ToArray();
                return output;
            });

            return rootProjectFilePaths;
        }
    }
}
