namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Grynwald.MarkdownGenerator;

    /// <summary>
    /// 
    /// </summary>
    internal class ParseXmlToMarkDown
	{
        private readonly XDocument xml;
        private MdDocument doc;
        DescriptionTemplates descriptionTemplates;

        /// <summary>
        /// 
        /// </summary>
        public ParseXmlToMarkDown(XDocument xml)
        {
            this.xml = xml;
            doc = new MdDocument();
            descriptionTemplates = new DescriptionTemplates(xml.Descendants("DescriptionTemplates").FirstOrDefault());
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConvertToMarkDown()
        {
            var Checks = xml.Descendants("Checks").Descendants("Check");
            foreach (var check in Checks)
            {
                XDocCheckHelper helper = new XDocCheckHelper(check, descriptionTemplates);
                doc.Root.Add(new MdHeading(helper.GetCheckName(), 1));

                var errorMessages = check.Descendants("ErrorMessage");
                foreach (var errorMessage in errorMessages)
                {
                    doc.Root.Add(new MdHeading(helper.GetCheckDescription(errorMessage), 3));
                }

            }

            doc.Save(@"C:\temp\example.md");
        }
	}
}