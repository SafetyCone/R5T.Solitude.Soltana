using System;
using System.Collections.Generic;

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

        public void AddProjectFile(string solutionFilePath, string projectFilePath, Guid projectTypeGuid)
        {
            this.ModfifySolutionFile(solutionFilePath, (solutionFile) =>
            {
                this.InMemorySolutionFileOperator.AddProjectFile(solutionFile, solutionFilePath, projectFilePath, projectTypeGuid);
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

            var hasProjectFile = this.InMemorySolutionFileOperator.HasProjectFile(solutionFile, solutionFilePath, projectFilePath);
            return hasProjectFile;
        }

        public IEnumerable<string> ListProjectReferenceFilePaths(string solutionFilePath)
        {
            var solutionFile = this.InMemorySolutionFileOperator.CreateNewSolutionFile();

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
    }
}
