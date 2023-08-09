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
            return template.Descendants("Format").FirstOrDefault().Value;
        }

        public XElement GetTemplateInputs(XAttribute templateId)
        {
            var template = GetElementById(templateId);
            return template.Descendants("InputParameters").FirstOrDefault();
        }

        private XElement GetElementById(XAttribute templateId)
        {
            return descriptionTemplates.Descendants("DescriptionTemplate").Where(template => template.Attribute("id").Value == templateId.Value).FirstOrDefault();
        }
    }
}
