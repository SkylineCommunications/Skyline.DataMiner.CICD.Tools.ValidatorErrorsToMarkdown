using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown.Tests
{
    [TestClass()]
    public class XDocCheckHelperTests
    {
        [TestMethod()]
        public void GetCheckDescriptionTest_NoTemplateNoOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description>
                                                <Format>Invalid prefix '{1}' in 'Protocol/Name' tag. Current value '{0}'.</Format>
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagValue</InputParameter>
                                                    <InputParameter id=""1"">invalidPrefix</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Invalid prefix '{invalidPrefix}' in 'Protocol/Name' tag. Current value '{tagValue}'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_NoTemplateOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description>
                                                <Format>Invalid prefix '{1}' in 'Protocol/Name' tag. Current value '{0}'.</Format>
                                                <InputParameters>
                                                    <InputParameter id=""0"" value=""Protocol"">tagValue</InputParameter>
                                                    <InputParameter id=""1"">invalidPrefix</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Invalid prefix '{invalidPrefix}' in 'Protocol/Name' tag. Current value 'Protocol'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_TemplateOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"" value=""Protocol"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            string xmlTemplateContent = @"  <DescriptionTemplates>
                                                <DescriptionTemplate id=""1000"">
                                                    <Name>MissingTag</Name>
                                                    <Format>Missing tag '{0}'.</Format>
                                                    <InputParameters>
                                                        <InputParameter id=""0"">tagName</InputParameter>
                                                    </InputParameters>
                                                </DescriptionTemplate>
                                            </DescriptionTemplates>";
            XElement element = XElement.Parse(xmlContent);
            XElement template = XElement.Parse(xmlTemplateContent);
            DescriptionTemplates templates = new DescriptionTemplates(template);
            XDocCheckHelper helper = new XDocCheckHelper(element, templates);
            
            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Missing tag 'Protocol'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_TemplateNoOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            string xmlTemplateContent = @"  <DescriptionTemplates>
                                                <DescriptionTemplate id=""1000"">
                                                    <Name>MissingTag</Name>
                                                    <Format>Missing tag '{0}'.</Format>
                                                    <InputParameters>
                                                        <InputParameter id=""0"">tagName</InputParameter>
                                                    </InputParameters>
                                                </DescriptionTemplate>
                                            </DescriptionTemplates>";
            XElement element = XElement.Parse(xmlContent);
            XElement template = XElement.Parse(xmlTemplateContent);
            DescriptionTemplates templates = new DescriptionTemplates(template);
            XDocCheckHelper helper = new XDocCheckHelper(element, templates);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Missing tag '{tagName}'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_TemplateNoOverrideOnlyInputTemplate()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000""></Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            string xmlTemplateContent = @"  <DescriptionTemplates>
                                                <DescriptionTemplate id=""1000"">
                                                    <Name>MissingTag</Name>
                                                    <Format>Missing tag '{0}'.</Format>
                                                    <InputParameters>
                                                        <InputParameter id=""0"">tagName</InputParameter>
                                                    </InputParameters>
                                                </DescriptionTemplate>
                                            </DescriptionTemplates>";
            XElement element = XElement.Parse(xmlContent);
            XElement template = XElement.Parse(xmlTemplateContent);
            DescriptionTemplates templates = new DescriptionTemplates(template);
            XDocCheckHelper helper = new XDocCheckHelper(element, templates);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Missing tag '{tagName}'.", result);
        }

        [TestMethod()]
        public void GetCheckNameTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckName();

            // Assert
            Assert.AreEqual("CheckProtocolTag", result);
        }

        [TestMethod()]
        public void GetCheckIdTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckId();

            // Assert
            Assert.AreEqual("1", result);
        }

        [TestMethod()]
        public void GetCheckSeverityTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckSeverity(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Critical", result);
        }

        [TestMethod()]
        public void GetCheckCertaintyTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckCertainty(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Certain", result);
        }

        [TestMethod()]
        public void GetCheckErrorMessageIdTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckErrorMessageId(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("5", result);
        }

        [TestMethod()]
        public void GetCheckErrorMessageNameTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new XDocCheckHelper(element, null);

            // Act
            var result = helper.GetCheckErrorMessageName(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("InvalidPrefix", result);
        }
    }
}