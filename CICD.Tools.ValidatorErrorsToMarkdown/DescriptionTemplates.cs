using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    /// <summary>
    /// Class to work with the description templates
    /// </summary>
    public class DescriptionTemplates
    {
        private XElement descriptionTemplates;

        /// <summary>
        /// constructor to get all the templates
        /// </summary>
        public DescriptionTemplates(XElement descriptionTemplates)
        {
            this.descriptionTemplates = descriptionTemplates;
        }

        /// <summary>
        /// Gets the format from the template with the the template id
        /// </summary>
        public string GetFormat(XAttribute templateId)
        {
            var template = GetElementById(templateId);
            return template?.Element("Format")?.Value;
        }

        /// <summary>
        /// Gets the Name of the template with the the template id
        /// </summary>
        public XElement GetTemplateInputs(XAttribute templateId)
        {
            var template = GetElementById(templateId);
            return template?.Element("InputParameters");
        }

        private XElement GetElementById(XAttribute templateId)
        {
            return descriptionTemplates?.Elements("DescriptionTemplate").Where(template => template?.Attribute("id")?.Value == templateId?.Value).FirstOrDefault();
        }
    }
}
