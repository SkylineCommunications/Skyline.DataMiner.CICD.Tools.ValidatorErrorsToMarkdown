using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

using Skyline.DataMiner.CICD.FileSystem;

namespace CICD.Tools.ValidatorErrorsToMarkdown
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var fileLocation = new Option<string>(
                name: "--fileLocation",
                description: "File containing the ErrorMessages.")
            {
                IsRequired = true
            };
            var resultLocation

            var rootCommand = new RootCommand("Returns MarkDown from xml file.")
            {
                fileLocation,
            };


            rootCommand.SetHandler(Process, fileLocation);

            await rootCommand.InvokeAsync(args);

            return 0;
        }

        private static async Task Process(string fileLocation)
        {
            XDocument xml = XDocument.Load(fileLocation);
        }
    }
}
