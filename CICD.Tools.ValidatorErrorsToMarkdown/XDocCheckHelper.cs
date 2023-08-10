using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    /// <summary>
    /// A helper to get all the info in the element check.
    /// </summary>
    public class XDocCheckHelper
    {
        private readonly XElement check;
        private readonly DescriptionTemplates descriptionTemplates;

        /// <summary>
        /// constructor with as input the Check itself and all description templates.
        /// </summary>
        public XDocCheckHelper(XElement check, DescriptionTemplates descriptionTemplates)
        {
            this.check = check;
            this.descriptionTemplates = descriptionTemplates;
        }

        /// <summary>
        /// Gets the id of the Check.
        /// </summary>
        public string GetCheckId() => check?.Attribute("id")?.Value;

        /// <summary>
        /// Gets the Name of the Check.
        /// </summary>
        public string GetCheckName() => check?.Element("Name")?.Value;

        /// <summary>
        /// Gets the id of the error message.
        /// </summary>
        public string GetCheckErrorMessageId(XElement errorMessage) => errorMessage?.Attribute("id")?.Value;

        /// <summary>
        /// Gets the name of the error message.
        /// </summary>
        public string GetCheckErrorMessageName(XElement errorMessage) => errorMessage?.Element("Name")?.Value;

        /// <summary>
        /// Gets the group description of the error message.
        /// </summary>
        public string GetCheckGroupDescription(XElement errorMessage) => errorMessage?.Element("GroupDescription")?.Value;

        /// <summary>
        /// Gets the Description of the error message.
        /// </summary>
        public string GetCheckDescription(XElement errorMessage)
        {
            XElement templateInputs = null;
            var templateId = errorMessage?.Element("Description")?.Attribute("templateId");
            var inputParameters = errorMessage?.Element("Description")?.Element("InputParameters");
            string format;
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

        /// <summary>
        /// Gets the severity of the error message.
        /// </summary>
        public string GetCheckSeverity(XElement errorMessage) => errorMessage?.Element("Severity")?.Value;

        /// <summary>
        /// Gets the certainty of the error message.
        /// </summary>
        public string GetCheckCertainty(XElement errorMessage) => errorMessage?.Element("Certainty")?.Value;

        /// <summary>
        /// Gets the source of the error message.
        /// </summary>
        public string GetCheckSource(XElement errorMessage) => errorMessage?.Element("Source")?.Value;

        /// <summary>
        /// Gets the fix impact of the error message.
        /// </summary>
        public string GetCheckFixImpact(XElement errorMessage) => errorMessage?.Element("FixImpact")?.Value;

        /// <summary>
        /// Gets the code fix of the error message.
        /// </summary>
        public bool HasCheckErrorMessageCodeFix(XElement errorMessage) => errorMessage?.Element("HasCodeFix")?.Value is "True";

        /// <summary>
        /// Gets the how to fix of the error message.
        /// </summary>
        public string GetCheckHowToFix(XElement errorMessage) => SetNewLines(errorMessage?.Element("HowToFix")?.Value);

        /// <summary>
        /// Gets the example code of the error message.
        /// </summary>
        public string GetCheckExampleCode(XElement errorMessage) => SetNewLines(errorMessage?.Element("ExampleCode")?.Value);

        /// <summary>
        /// Gets the details of the error message.
        /// </summary>
        public string GetCheckDetails(XElement errorMessage) => SetNewLines(errorMessage?.Element("Details")?.Value);

        /// <summary>
        /// Gets the auto fix warnings of the error message.
        /// </summary>
        public string GetCheckAutoFixWarnings(XElement errorMessage)
        {
            var warnings = errorMessage?.Element("AutoFixWarnings").Elements("AutoFixWarning").ToList();
            return warnings[0].Value + Environment.NewLine + Environment.NewLine + warnings[1].Value;
        }

        private static string[] GetInputParams(XElement inputParameters, XElement templateInput)
        {
            IEnumerable<XElement> inputParams = inputParameters?.Elements("InputParameter");
            string[] inputParamsArray;
            if (templateInput is not null)
            {
                IEnumerable<XElement> templateInputs = templateInput?.Elements("InputParameter");
                inputParamsArray = new string[templateInputs.Count()];
                inputParamsArray = CheckValueOverrides(templateInputs, inputParamsArray);
            }
            else
            {
                inputParamsArray = new string[inputParameters.Elements("InputParameter").Count()];
            }

            
            if (inputParams is not null)
                inputParamsArray = CheckValueOverrides(inputParams, inputParamsArray);

            return inputParamsArray;
        }

        /// <summary>
        /// Checks if the value atribute overrides the value of the parameter and returns the array with the expected values
        /// </summary>
        private static string[] CheckValueOverrides(IEnumerable<XElement> inputs, string[] inputParamsArray)
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

        private static string SetNewLines(string data)
        {
            data = data.Replace("[NewLine]", Environment.NewLine);
            return data;
        }
    }
}
