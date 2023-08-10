using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    public class DescriptionTemplates
    {
        private XElement descriptionTemplates;

        public DescriptionTemplates(XElement descriptionTemplates)
        {
            this.descriptionTemplates = descriptionTemplates;
        }

        public string GetFormat(XAttribute templateId)
        {
            var template = GetElementById(templateId);
            return template?.Element("Format")?.Value;
        }

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
