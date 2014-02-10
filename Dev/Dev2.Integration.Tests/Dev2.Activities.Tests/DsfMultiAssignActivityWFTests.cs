﻿using System;
using System.Text.RegularExpressions;
using Dev2.Integration.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev2.Integration.Tests.Dev2.Activities.Tests
{
    /// <summary>
    /// Summary description for DsfMultiAssignActivityWFTests
    /// </summary>
    [TestClass]
    public class DsfMultiAssignActivityWFTests
    {
        readonly string WebserverURI = ServerSettings.WebserverURI;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region RecordSet Tests

        [TestMethod]
        public void MutiAssignUsingStarIntegrationTest()
        {
            string PostData = String.Format("{0}{1}", WebserverURI, "MutiAssignWithStarTestWorkFlow");
            string expected = "<testScalar>testScalarData</testScalar>";
            string expected2 = "<recset2>      <field2>world1</field2>    </recset2>    <recset2>      <field2>world2</field2>    </recset2>    <recset2>      <field2>world3</field2>    </recset2>    <recset2>      <field2>world4</field2>    </recset2>";
            string expected3 = "<recset1>      <rec>testScalarData</rec>      <field>world1</field>    </recset1>    <recset1>      <rec>hello1</rec>      <field>world2</field>    </recset1>    <recset1>      <rec>hello2</rec>      <field>world3</field>    </recset1>    <recset1>      <rec>hello3</rec>      <field>world4</field>    </recset1>    <recset1>      <rec>hello4</rec>      <field />    </recset1>";

            string ResponseData = TestHelper.PostDataToWebserver(PostData);


            Regex regex = new Regex(@">\s*<");

            expected = regex.Replace(expected, "><");
            expected2 = regex.Replace(expected, "><");
            expected3 = regex.Replace(expected, "><");
            ResponseData = regex.Replace(ResponseData, "><");

            StringAssert.Contains(ResponseData, expected);
            StringAssert.Contains(ResponseData, expected2);
            StringAssert.Contains(ResponseData, expected3);
        }

        // Test created by: Michael
        [TestMethod]
        public void MultiAssignUsingIndexIntegrationTest()
        {
            string PostData = String.Format("{0}{1}", WebserverURI, "MultiAssignUsingIndexIntegrationTest");
            string ResponseData = TestHelper.PostDataToWebserver(PostData);

            string expected = "<recSet><Name>1</Name><Surname>2</Surname></recSet><recSet><Name>3</Name><Surname>4</Surname></recSet>";
            StringAssert.Contains(ResponseData, expected);
        }

        // Test created by: Michael
        // Broken - Bug: 7836

        [TestMethod]
        public void MultiAssignUsingBlankIntegrationTest()
        {
            string PostData = String.Format("{0}{1}", WebserverURI, "MultiAssignUsingBlankIntegrationTest");
            string ResponseData = TestHelper.PostDataToWebserver(PostData);

            string expected1 = "<someRec><Name>NAME1</Name><Surname>SURNAME1</Surname></someRec>";
            string expected2 = "<someRec><Name>Name2</Name><Surname>Surname2</Surname></someRec>";
            string expected3 = "<someRec><Name>name3</Name><Surname>SURNAME3</Surname></someRec>";
            string expected4 = "<someRec><Name>Name4</Name><Surname></Surname></someRec>";
            string expected5 = "<someRec><Name>name5</Name><Surname>SURNAME5</Surname></someRec>";
            string expected6 = "<someRec><Name></Name><Surname>Surname6</Surname></someRec>";
            string expected7 = "<someRec><Name></Name><Surname>Surname7</Surname></someRec>";

            StringAssert.Contains(ResponseData, expected1);
            StringAssert.Contains(ResponseData, expected2);
            StringAssert.Contains(ResponseData, expected3);
            StringAssert.Contains(ResponseData, expected4);
            StringAssert.Contains(ResponseData, expected5);
            StringAssert.Contains(ResponseData, expected6);
            StringAssert.Contains(ResponseData, expected7);
        }

        #endregion RecordSet Tests

        #region Recursive Nature Tests

        [TestMethod]
        public void MutiAssignUsingRecursiveEvalutationIntergrationTest()
        {
            string PostData = String.Format("{0}{1}", WebserverURI, "MutiAssignRecursiveEvaluationTestWorkflow");
            string expected = @"<testScalar>hello2</testScalar><recset1><rec>testScalarData</rec><field>world1</field></recset1><recset1><rec>hello1</rec><field>world2</field></recset1><recset1><rec>hello2</rec><field>world3</field></recset1><recset1><rec>hello3</rec><field>world4</field></recset1><recset1><rec>hello4</rec><field></field></recset1><recset2><field2>world1</field2></recset2><recset2><field2>world2</field2></recset2><recset2><field2>world3</field2></recset2><recset2><field2>world4</field2></recset2><recsetName>recset1</recsetName><recsetFieldName>rec</recsetFieldName><recsetIndex>3</recsetIndex><five>se</five><six>ven</six><temp>7</temp><seven>7</seven><eight></eight>";

            string ResponseData = TestHelper.PostDataToWebserver(PostData);

            //Assert.IsTrue(XElement.DeepEquals(XElement.Parse(expected), XElement.Parse(ResponseData)));
            StringAssert.Contains(ResponseData, expected);
        }

        #endregion Recursive Nature Tests

        #region Calculation Mode Tests


        [TestMethod]
        public void MultiAssign_Calculate_NoCalculate_Comparison_Expected()
        {
            string postData = String.Format("{0}{1}?{2}", ServerSettings.WebserverURI, "MultiAssignCalculateNoCalculateComparisonTest", "Input=10");
            string responseData = TestHelper.PostDataToWebserver(postData);


            int index = responseData.IndexOf("<CalcResult>");
            string actualCalcResult = null;
            string actualNoCalcResult = null;

            if(index != -1)
            {
                int next = responseData.IndexOf("</CalcResult>", index + 1);

                if(next != -1)
                {
                    actualCalcResult = responseData.Substring(index + 12, next - (index + 12));
                }
            }

            index = responseData.IndexOf("<NoCalcResult>");

            if(index != -1)
            {
                int next = responseData.IndexOf("</NoCalcResult>", index + 1);

                if(next != -1)
                {
                    actualNoCalcResult = responseData.Substring(index + 14, next - (index + 14));
                }
            }

            Assert.AreEqual(actualCalcResult, "40");
            Assert.AreEqual(actualNoCalcResult, "sum(30,10)");
        }

        #endregion Calculation Mode Testss
    }
}

