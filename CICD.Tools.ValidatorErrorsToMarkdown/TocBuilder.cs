using System;
using System.IO;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    /// <summary>
    /// Builds the Toc file structure.
    /// </summary>
    public class TocBuilder
    {
        private readonly int maxRecurseDepth = 200;

        /// <summary>
        /// Creates a root <see cref="TocItem"/> from the given directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>A <see cref="TocItem"/> root.</returns>
        public TocItem CreateFromDirectory(string directory)
        {
            var Toc = new TocItem(directory);
            RecurseParseDirectory(directory, Toc);
            return Toc;
        }

        private void RecurseParseDirectory(string directory, TocItem itemToAddTo, int recurseDepthCheck = 0)
        {
            if (recurseDepthCheck == maxRecurseDepth) throw new InvalidOperationException("Recursing too deep. Max level: " + maxRecurseDepth);

            foreach (var dir in Directory.GetDirectories(directory))
            {
                var child = itemToAddTo.With(new TocItem(dir));
                RecurseParseDirectory(dir, child, recurseDepthCheck++);
            }
        }
    }
}
