namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Grynwald.MarkdownGenerator;

    /// <summary>
    /// Converts the xml file to a tree structure with markDown files
    /// </summary>
    internal class ParseXmlToMarkDown
    {
        private readonly XDocument xml;
        private readonly string outputDirectoryPath;
        DescriptionTemplates descriptionTemplates;

        /// <summary>
        /// Creates an instance of class <see cref="ParseXmlToMarkDown"/>
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="outputDirectoryPath"></param>
        public ParseXmlToMarkDown(XDocument xml, string outputDirectoryPath)
        {
            this.xml = xml;
            this.outputDirectoryPath = outputDirectoryPath;
            descriptionTemplates = new DescriptionTemplates(xml.Descendants("DescriptionTemplates").FirstOrDefault());
        }

        /// <summary>
        /// Creates the layout of the markdown files
        /// </summary>
        public void ConvertToMarkDown()
        {
            var categories = xml?.Element("Validator")?.Element("ValidationChecks")?.Element("Categories")?.Elements("Category");
            foreach (var category in categories)
            {
                string catagoryId = category?.Attribute("id")?.Value;
                string catagoryName = category?.Element("Name")?.Value;

                var checks = category?.Element("Checks")?.Elements("Check");
                foreach (var check in checks)
                {
                    XDocCheckHelper helper = new(check, descriptionTemplates);
                    string checkName = helper.GetCheckName();
                    string checkId = helper.GetCheckId();

                    var errorMessages = check.Descendants("ErrorMessage");
                    foreach (var errorMessage in errorMessages)
                    {
                        string fullId = $"{catagoryId}.{checkId}.{XDocCheckHelper.GetCheckErrorMessageId(errorMessage)}";
                        string uid = $"Validator_{catagoryId}_{checkId}_{XDocCheckHelper.GetCheckErrorMessageId(errorMessage)}";
                        MdDocument doc = new();
                        // uid
                        doc.Root.Add(new MdParagraph(new MdRawMarkdownSpan($"---\r\nuid: {uid}\r\n---")));
                        // check name
                        doc.Root.Add(new MdHeading(checkName, 1));
                        // error message name
                        doc.Root.Add(new MdHeading(XDocCheckHelper.GetCheckErrorMessageName(errorMessage), 2));
                        // description
                        doc.Root.Add(new MdHeading("Description", 3));
                        doc.Root.Add(new MdParagraph(helper.GetCheckDescription(errorMessage)));
                        // properties table
                        doc.Root.Add(new MdHeading("Properties", 3));
                        doc.Root.Add(CreateTable(errorMessage, catagoryName, fullId));

                        string howToFix = XDocCheckHelper.GetCheckHowToFix(errorMessage);
                        if (howToFix is not "")
                        {
                            doc.Root.Add(new MdHeading("How to fix", 3));
                            doc.Root.Add(new MdParagraph(howToFix));
                        }

                        string exampleCode = XDocCheckHelper.GetCheckExampleCode(errorMessage);
                        if (exampleCode is not "")
                        {
                            doc.Root.Add(new MdHeading("Example code", 3));
                            doc.Root.Add(new MdCodeBlock(exampleCode, "xml"));
                        }

                        string details = XDocCheckHelper.GetCheckDetails(errorMessage);
                        if (details is not "")
                        {
                            doc.Root.Add(new MdHeading("Details", 3));
                            doc.Root.Add(new MdParagraph(details));
                        }

                        List<string> autofixWarnings = XDocCheckHelper.GetCheckAutoFixWarnings(errorMessage);
                        if (autofixWarnings is not null)
                        {
                            MdParagraph warnings = new();
                            warnings.Add(new MdRawMarkdownSpan($"[!WARNING]\r\n"));
                            for (int i = 0; i < autofixWarnings.Count; i++)
                            {
                                warnings.Add(new MdRawMarkdownSpan($"{autofixWarnings[i]}\r\n"));
                            }
                        }

                        string source = XDocCheckHelper.GetCheckSource(errorMessage);
                        Directory.CreateDirectory($@"{outputDirectoryPath}\DIS\{source}\{catagoryName}\{checkName}");

                        doc.Save($@"{outputDirectoryPath}\DIS\{source}\{catagoryName}\{checkName}\{uid}.md");
                    }
                }
            }
        }

        /// <summary>
        /// Creates a table with all the properties 
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="categoryName"></param>
        /// <param name="fullId"></param>
        /// <returns>A <see cref="MdTable"/> table</returns>
        private static MdTable CreateTable(XElement errorMessage, string categoryName, string fullId)
        {
            MdSpan[] headingCells = { "Name", "Value" };
            MdTableRow tableRowHeading = new(headingCells);

            MdSpan[] categoryCells = { "Category", categoryName };
            MdTableRow tableRowCategory = new(categoryCells);

            MdSpan[] fullIdCells = { "Full Id", fullId };
            MdTableRow tableRowfullId = new(fullIdCells);

            MdSpan[] severityCells = { "Severity", XDocCheckHelper.GetCheckSeverity(errorMessage) ?? "Variable" };
            MdTableRow tableRowSeverity = new(severityCells);

            MdSpan[] certaintyCells = { "Certainty", XDocCheckHelper.GetCheckCertainty(errorMessage) ?? "Variable" };
            MdTableRow tableRowCertainty = new(certaintyCells);

            MdSpan[] sourceCells = { "Source", XDocCheckHelper.GetCheckSource(errorMessage) };
            MdTableRow tableRowSource = new(sourceCells);

            MdSpan[] fixImpactCells = { "Fix Impact", XDocCheckHelper.GetCheckFixImpact(errorMessage) };
            MdTableRow tableRowFixImpact = new(fixImpactCells);

            MdSpan[] hasCodeFixCells = { "Has Code Fix", XDocCheckHelper.HasCheckErrorMessageCodeFix(errorMessage).ToString() };
            MdTableRow tableRowHasCodeFix = new(hasCodeFixCells);

            MdTableRow[] tableRows = { tableRowCategory, tableRowfullId, tableRowSeverity, tableRowCertainty, tableRowSource, tableRowFixImpact, tableRowHasCodeFix };
            MdTable table = new(tableRowHeading, tableRows);

            string groupDescription = XDocCheckHelper.GetCheckGroupDescription(errorMessage);
            if (groupDescription != "")
            {
                MdSpan[] groupDescriptionCells = { "Group Description", groupDescription };
                MdTableRow tableRowDescriptionGroup = new(groupDescriptionCells);
                table.Add(tableRowDescriptionGroup);
            }

            return table;
        }
    }
}