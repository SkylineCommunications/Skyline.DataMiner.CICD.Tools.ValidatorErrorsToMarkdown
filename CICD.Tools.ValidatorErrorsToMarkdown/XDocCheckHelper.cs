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

        public string GetCheckName()
        {
            string checkName = check.Descendants("Name").FirstOrDefault().Value;
            return checkName;
        }

        public string GetCheckDescription(XElement errorMessage)
        {
            string format = String.Empty;
            XElement templateInputs = null;
            var templateId = errorMessage.Descendants("Description").FirstOrDefault().Attribute("templateId");
            var inputParameters = errorMessage.Descendants("InputParameters").FirstOrDefault();
            if (templateId != null)
            {
                format = descriptionTemplates.GetFormat(templateId);
                templateInputs = descriptionTemplates.GetTemplateInputs(templateId);
            }
            else
            {
                 format = errorMessage.Descendants("Format").FirstOrDefault().Value;                
            }

            var input = GetInputParams(inputParameters, templateInputs);          
            return string.Format(format, input);
        }

        private static string[] GetInputParams(XElement inputParameters, XElement templateInput)
        {
            string[] inputParamsArray;
            IEnumerable<XElement> inputParams = null;
            if (templateInput != null)
            {
                IEnumerable<XElement> templateInputs = templateInput.Descendants("InputParameter");
                inputParamsArray = new string[templateInputs.Count()];
                inputParamsArray = GetCorrectInput(templateInputs, inputParamsArray);
            }
            else
            {
                inputParams = inputParameters.Descendants("InputParameter");
                inputParamsArray = new string[inputParams.Count()];
            }

            inputParams = inputParameters.Descendants("InputParameter");
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
                    inputParamsArray[index] = "{" + parameter.Value + "}";
                }
                else
                {
                    inputParamsArray[index] = parameter.Attribute("value").Value;
                }

                index++;
            }

            return inputParamsArray;
        }
    }
}
