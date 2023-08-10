namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Grynwald.MarkdownGenerator;

    /// <summary>
    /// 
    /// </summary>
    internal class ParseXmlToMarkDown
	{
        private readonly XDocument xml;
        private DocumentSet<MdDocument> docSet;
        DescriptionTemplates descriptionTemplates;

        /// <summary>
        /// 
        /// </summary>
        public ParseXmlToMarkDown(XDocument xml)
        {
            this.xml = xml;
            descriptionTemplates = new DescriptionTemplates(xml.Descendants("DescriptionTemplates").FirstOrDefault());
            docSet = new DocumentSet<MdDocument>();
        }

        /// <summary>
        /// creating the MarkDown file.
        /// </summary>
        public void ConvertToMarkDown()
        {           
            var categories = xml?.Element("Validator")?.Element("ValidationChecks")?.Element("Categories")?.Elements("Category");
            foreach (var category in categories)
            {              
                string catagoryId = category?.Attribute("id")?.Value;
                string catagoryName = category?.Element("Name")?.Value;
                Directory.CreateDirectory($@"C:\Validator\{catagoryName}");

                var checks = category?.Elements("Check");
                foreach (var check in checks)
                {
                    XDocCheckHelper helper = new(check, descriptionTemplates);
                    string checkName = helper.GetCheckName();
                    string checkId = helper.GetCheckId();
                    Directory.CreateDirectory($@"C:\Validator\{catagoryName}\{checkName}");

                    var errorMessages = check.Descendants("ErrorMessage");
                    foreach (var errorMessage in errorMessages)
                    {
                        string uid = $"Validator_{catagoryId}_{checkId}_{helper.GetCheckErrorMessageId(errorMessage)}";
                        MdDocument doc = new();
                        doc.Root.Add(new MdParagraph(new MdRawMarkdownSpan($"---\r\nuid: {uid}\r\n---")));
                        doc.Root.Add(new MdHeading(checkName, 1));
                        doc.Root.Add(new MdHeading(helper.GetCheckErrorMessageName(errorMessage), 2));
                        doc.Root.Add(new MdParagraph(helper.GetCheckDescription(errorMessage)));
                        doc.Root.Add(new MdParagraph(catagoryName));
                        doc.Root.Add(new MdParagraph($"Severity: {helper.GetCheckSeverity(errorMessage)}"));
                        doc.Root.Add(new MdParagraph($"Certainty: {helper.GetCheckCertainty(errorMessage)}"));
                        doc.Save($@"C:\Validator\{catagoryName}\{checkName}\{uid}.md");
                    }
                }
            }                        
        }
	}
}