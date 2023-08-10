using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    public class XDocCheckHelper
    {
        private readonly XElement check;
        private readonly DescriptionTemplates descriptionTemplates;

        public XDocCheckHelper(XElement check, DescriptionTemplates descriptionTemplates)
        {
            this.check = check;
            this.descriptionTemplates = descriptionTemplates;
        }

        public string GetCheckId() => check?.Attribute("id")?.Value;

        public string GetCheckName() => check?.Element("Name")?.Value;

        public string GetCheckDescription(XElement errorMessage)
        {
            string format = String.Empty;
            XElement templateInputs = null;
            var templateId = errorMessage?.Element("Description")?.Attribute("templateId");
            var inputParameters = errorMessage?.Element("Description")?.Element("InputParameters");
            if (templateId != null)
            {
                format = descriptionTemplates.GetFormat(templateId);
                templateInputs = descriptionTemplates.GetTemplateInputs(templateId);
            }
            else
            {
                 format = errorMessage?.Element("Description")?.Element("Format")?.Value;                
            }

            var input = GetInputParams(inputParameters, templateInputs);          
            return string.Format(format, input);
        }

        public string GetCheckSeverity(XElement errorMessage) => errorMessage?.Element("Severity")?.Value;

        public string GetCheckCertainty(XElement errorMessage) => errorMessage?.Element("Certainty")?.Value;

        public string GetCheckErrorMessageId(XElement errorMessage) => errorMessage?.Attribute("id")?.Value;

        public string GetCheckErrorMessageName(XElement errorMessage) => errorMessage?.Element("Name")?.Value;

        private static string[] GetInputParams(XElement inputParameters, XElement templateInput)
        {
            string[] inputParamsArray;
            IEnumerable<XElement> inputParams = null;
            if (templateInput is not null)
            {
                IEnumerable<XElement> templateInputs = templateInput?.Elements("InputParameter");
                inputParamsArray = new string[templateInputs.Count()];
                inputParamsArray = GetCorrectInput(templateInputs, inputParamsArray);
            }
            else
            {
                inputParamsArray = new string[inputParameters.Elements("InputParameter").Count()];
            }

            inputParams = inputParameters?.Elements("InputParameter");
            if (inputParams is not null)
                inputParamsArray = GetCorrectInput(inputParams, inputParamsArray);

            return inputParamsArray;
        }

        private static string[] GetCorrectInput(IEnumerable<XElement> inputs, string[] inputParamsArray)
        {            
            int index = 0;
            foreach (var parameter in inputs)
            {
                if (parameter.Attribute("value") == null)
                {
                    inputParamsArray[index] = "{" + parameter?.Value + "}";
                }
                else
                {
                    inputParamsArray[index] = parameter?.Attribute("value")?.Value;
                }

                index++;
            }

            return inputParamsArray;
        }
    }
}
