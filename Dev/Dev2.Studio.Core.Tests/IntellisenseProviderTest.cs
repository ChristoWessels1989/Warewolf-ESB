﻿using Dev2.Composition;
using Dev2.DataList.Contract;
using Dev2.MathOperations;
using Dev2.Studio.Core;
using Dev2.Studio.Core.AppResources.Enums;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.Interfaces.DataList;
using Dev2.Studio.Core.Models;
using Dev2.Studio.InterfaceImplementors;
using Dev2.Studio.ViewModels.DataList;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Dev2.Core.Tests
{
    [TestClass]
    public class IntellisenseProviderTest
    {
        private IResourceModel _resourceModel;

        #region Test Initialization

        [TestInitialize]
        public void Init()
        {
            Monitor.Enter(DataListSingletonTest.DataListSingletonTestGuard);

            ImportService.CurrentContext = CompositionInitializer.InitializeForMeflessBaseViewModel();

            var testEnvironmentModel = new Mock<IEnvironmentModel>();
            testEnvironmentModel.Setup(model => model.DsfChannel.ExecuteCommand(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns("");

            _resourceModel = new ResourceModel(testEnvironmentModel.Object) { ResourceName = "test", ResourceType = ResourceType.Service, DataList = @"
            <DataList>
                    <Scalar/>
                    <Country/>
                    <State />
                    <City>
                        <Name/>
                        <GeoLocation />
                    </City>
             </DataList>
            " };

            IDataListViewModel setupDatalist = new DataListViewModel();
            DataListSingleton.SetDataList(setupDatalist);
            DataListSingleton.ActiveDataList.InitializeDataListViewModel(_resourceModel);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Monitor.Exit(DataListSingletonTest.DataListSingletonTestGuard);
        }

        #endregion Test Initialization

        #region DefaultIntellisenseProvider

        #region GetIntellisenseResults


        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_OpenRegion_Expected_AllVarsInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 2, 
                InputText = "[[", DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_OpenRegion_AndInRecSetIndex_Expected_AllVarsInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City([[", DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(3, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
        }

        //BUG 8755
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResultsWithOpenRegionAndStarIndexExpectedNoResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 8, 
                InputText = "[[City(*", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        // ReSharper disable InconsistentNaming
        [TestMethod]
        // ReSharper restore InconsistentNaming
        public void GetIntellisenseResultsWithOpenRegionAndOpenRegionStarIndexExpectedNoResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 10,
                InputText = "[[City([[*",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_OpenRegion_AndInRecSetIndex_AndNoParentRegion_Expected_AllVarsInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 7, 
                InputText = "City([[", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithOpenRegionAndStarIndexAndNoParentRegionExpectedNoResults()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 6, 
                InputText = "City(*", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_OpenRegion_AndInRecSetIndex_AndWithField_Expected_AllVarsInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City([[).Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());

        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithInRegionAndStarIndexAndWithFieldExpectedNoResults()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 8, 
                InputText = "[[City(*).Name]]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithInRegionAndNumberIndexAndWithFieldExpectedNoResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 9,
                InputText = "[[City(33).Name]]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_InRecSetIndex_AndWithField_Expected_AllVarsInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 7, 
                InputText = "City([[).Name", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithNoRegionAndStarIndexAndWithFieldExpectedNoResults()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 6, 
                InputText = "City(*).Name", 
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithNoRegionAndStarNumberAndWithFieldExpectedNoResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 6,
                InputText = "City(4).Name",
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_OpenRegion_AndInRecSetIndex_AndWithField_Expected_ScalarVarInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 12, 
                InputText = "[[City([[sca).Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
        }

        //BUG 8759, This test was incorect, it was testing a bug in the system.
        //          It has been ammended to break and a bug (8759) logged against it.
        //[TestMethod]
        //// ReSharper disable InconsistentNaming
        //public void GetIntellisenseResults_With_OpenRegion_AndAfterRecSetIndex_AndWithPartialField_Expected_ScalarVarInResults()
        //// ReSharper restore InconsistentNaming
        //{
        //    var context = new IntellisenseProviderContext 
        //    { 
        //        CaretPosition = 21, 
        //        InputText = "[[City([[Scalar]]).Na", 
        //        DesiredResultSet = IntellisenseDesiredResultSet.Default 
        //    };

        //    var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

        //    Assert.AreEqual(1, getResults.Count);
        //    Assert.AreEqual("[[City([[Scalar]]).Name]]", getResults[0].ToString());

        //    foreach (var result in getResults)
        //    {
        //        Assert.IsFalse(result.IsError, "An error occurent in one of the results");
        //    }
        //}

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithOpenRegionAndAfterStarIndexAndWithPartialFieldExpectedScalarVarInResults()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 12, 
                InputText = "[[City(*).Na", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual("[[City(*).Name]]", getResults[0].ToString());

        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithOpenRegionAndAfterNumberIndexAndWithPartialFieldExpectedScalarVarInResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 12,
                InputText = "[[City(6).Na",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual("[[City(6).Name]]", getResults[0].ToString());

        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_InRecSetIndex_AndWithField_Expected_ScalarVarInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 10, 
                InputText = "City([[sca).Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());

        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_OpenRegion_AndInRecSetIndex_AndWithField_Expected_RecSetVarInResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 12, 
                InputText = "[[City([[Cit).Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(5, getResults.Count);
            Assert.AreEqual("[[City(", getResults[0].ToString());
            Assert.AreEqual("[[City(*)]]", getResults[1].ToString());
            Assert.AreEqual("[[City()]]", getResults[2].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[3].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[4].ToString());

        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_Expression_Expected_NoResults()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 10, 
                InputText = "{{var r  = \"[[xpath('//result/node()')]]\";if(r.indexOf(\"success\") == -1){var s = \"No\";}else{var s =\"Yes\";}}}", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_CommaSeperatedRegions_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 13, 
                InputText = "[[Scalar]],[[", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());

        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_CommaSeperatedRegions_AndWithinIndex_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 20, 
                InputText = "[[Scalar]],[[City([[).Name]],[[Country]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithCommaSeperatedRegionsAndStarIndexExpectedNoResults()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 19, 
                InputText = "[[Scalar]],[[City(*).Name]],[[Country]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWithCommaSeperatedRegionsAndNumberIndexExpectedNoResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 19,
                InputText = "[[Scalar]],[[City(5).Name]],[[Country]]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_CommaSeperatedRegions_AndBeforeFirstComma_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 2, 
                InputText = "[[,[[City([[Scalar]]).Name]],[[Country]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_CommaSeperatedRegions_AndAfterLastComma_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 40, 
                InputText = "[[City([[Scalar]]).Name]],[[Country]],[[", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_Sum_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 6, 
                InputText = "Sum([[",
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_Sum_AndAfterComma_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 17, 
                InputText = "Sum([[Scalar]],[[", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };
            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_Sum_AndBeforeComma_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 6, 
                InputText = "Sum([[,[[Scalar]])", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void GetIntellisenseResults_With_Sum_AndWithinCommas_Expected_AllVarsInResults()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 16, 
                InputText = "Sum([[State]],[[,[[Scalar]])", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWithSumAndAfterCommaAndBeforeBraceExpectedAllVarsInResults()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 17, 
                InputText = "Sum([[Scalar]],[[)", 
                DesiredResultSet = IntellisenseDesiredResultSet.EntireSet 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWhereBracketOfRecordsetIsClosedExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 8, 
                InputText = "[[City()", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWhereBracketOfRecordsetIsClosedAndStarIndexExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City(*)", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWhereBracketOfRecordsetIsClosedAndNumberIndexExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City(77)", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWhereBracketOfRecordsetIsClosedAndThereIsAFieldAfterClosedBracketExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 8, 
                InputText = "[[City().Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWhereBracketOfRecordsetIsClosedAndThereIsAFieldAfterClosedBracketAndNumberIndexExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 10, 
                InputText = "[[City(44).Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8755
        [TestMethod]
        public void GetIntellisenseResultsWhereBracketOfRecordsetIsClosedAndThereIsAFieldAfterClosedBracketAndStarIndexExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City(*).Name]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWhereCommaEnteredForInfragisticsFunctonExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 15, 
                InputText = "Sum([[Scalar]],", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWithAdjacentRegionsInParamaterOfInfragisticsFunctionExpectedAllVarsInResults()
        {
            var context = new IntellisenseProviderContext 
            { CaretPosition = 27, 
                InputText = "Sum([[Scalar]],[[Scalar]][[", 
                DesiredResultSet = 
                IntellisenseDesiredResultSet.EntireSet 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWhereCarretPositionPastTheLengthOfTheInputTextExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 16, 
                InputText = "Sum([[Scalar]],", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWhereCarretPositionLessThanZeroExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = -1, 
                InputText = "Sum([[Scalar]],", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };
            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);
            Assert.AreEqual(0, getResults.Count);
        }

        //BUG 8736
        [TestMethod]
        public void GetIntellisenseResultsWhereInputTextContainsSpecialCharactersExpectedNoResultsAndException()
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 30, 
                InputText = "!@#$%^&*()_+[]{}\\|;:'\",./?><", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);
            Assert.AreEqual(0, getResults.Count);
        }

        //2013.04.16: Ashley Lewis - for Bug 6103
        [TestMethod]
        public void GetIntellisenseResultsWithInRecSetIndexAndWithFieldAndWithClosingSquareBraceExpectedCorrectErrorResult()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 13,
                InputText = "[[City([[sca]).Name]]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(StringResources.IntellisenseErrorMisMatchingBrackets, getResults[0].Description);
        }
        [TestMethod]
        public void GetIntellisenseResultsWithInRecSetIndexAndWithFieldAndWithBothClosingSquareBracesExpectedErrorResult()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 14,
                InputText = "[[City([[sca]]).Name]]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.IsTrue(getResults[0].IsError, "Intellisense did not recognize variable as an error");
            Assert.AreEqual("Missing Scalar", getResults[0].ToString(), "Intellisense did not throw unrecognized variable error");
        }
        [TestMethod]
        public void GetIntellisenseResultsWithClosingSquareBraceExpectedCorrectErrorResult()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 9,
                InputText = "[[scalar]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(StringResources.IntellisenseErrorMisMatchingBrackets, getResults[0].Description);
        }

        //2013.04.22: Ashley Lewis - for Bug 6103 QA Feedback
        [TestMethod]
        public void GetIntellisenseResultsWithOpenRegionAndInRecSetIndexAndWithFieldExpectedAllResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 9,
                InputText = "[[City([[).Name]]",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(6, getResults.Count);
            Assert.AreEqual("[[Scalar]]", getResults[0].ToString());
            Assert.AreEqual("[[Country]]", getResults[1].ToString());
            Assert.AreEqual("[[State]]", getResults[2].ToString());
            Assert.AreEqual("[[City()]]", getResults[3].ToString());
            Assert.AreEqual("[[City().Name]]", getResults[4].ToString());
            Assert.AreEqual("[[City().GeoLocation]]", getResults[5].ToString());
        }

        //2013.06.11: Ashley Lewis for bug 8759 - intellisense for partial field
        public void GetIntellisenseResultsWithPartialFieldAndScalarsInIndexExpectedVarInResultIndices()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 22,
                InputText = "[[City([[Scalar]]).Nam",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(2, getResults.Count);
            Assert.AreEqual("[[City([[Scalar]]).Name]]", getResults[0].ToString());
            Assert.AreEqual("Invalid Expression", getResults[1].ToString());
        }
        public void GetIntellisenseResultsWithPartialFieldAndScalarsInIndexExpectedNoVarInResultIndices()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 22,
                InputText = "[[City([[Scalar]]).Name]], [[City().Nam",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(2, getResults.Count);
            Assert.AreEqual("[[City().Name]]", getResults[0].ToString());
            Assert.AreEqual("Invalid Expression", getResults[1].ToString());
        }
        public void GetIntellisenseResultsWithTrailingCommaExpectedNoResults()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 27,
                InputText = "[[City([[Scalar]]).Name]], ",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(0, getResults.Count);
        }
        public void GetIntellisenseResultsWithTrailingCloseRegionExpectedCorrectResult()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = "[[City([[Scalar]]).Nam".Length,
                InputText = "[[City([[Scalar]]).Nam]], ",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual("[[City([[Scalar]]).Name]]", getResults[0]);
        }


        //2013.06.13: Ashley lewis for bug 7847 - numeric region error
        [TestMethod]
        public void NumericRegionExpectedReturnsError()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = "[[4]]".Length,
                InputText = "[[4]], ",
                DesiredResultSet = IntellisenseDesiredResultSet.Default
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);

            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual("Invalid syntax - You have started a variable name with a number", getResults[0].Description);
        }

        //2013.05.29: Ashley Lewis for bug 9472 - RecorsetsOnly filter tests
        [TestMethod]
        public void GetIntellisenseResultsWithRecordsetFilterAndNoRegionExpectedCompleteResult()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 3,
                InputText = "Cit",
                DesiredResultSet = IntellisenseDesiredResultSet.Default,
                State = true,
                FilterType = enIntellisensePartType.RecorsetsOnly
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);
            Assert.AreEqual("[[City()]]", getResults[0].ToString(), "Intellisense got recordset filtered results incorrectly");
        }
        [TestMethod]
        public void GetIntellisenseResultsWithRecordsetFilterExpectedCompleteResult()
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 3,
                InputText = "[[C",
                DesiredResultSet = IntellisenseDesiredResultSet.Default,
                State = true,
                FilterType = enIntellisensePartType.RecorsetsOnly
            };

            var getResults = new DefaultIntellisenseProvider().GetIntellisenseResults(context);
            Assert.AreEqual("[[City()]]", getResults[0].ToString(), "Intellisense got recordset filtered results incorrectly");
        }

        #endregion

        #region PerformResultInsertion

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_Expected_ResultReplacesText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 5, 
                InputText = "[[sca", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };
            
            Assert.AreEqual("[[scalar]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[scalar]]", context));
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialRecSet_AndRegion_Expected_ResultReplacesText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 5, 
                InputText = "[[rec", DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            Assert.AreEqual("[[recset().field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset().field]]", context));
        }

        //2013.01.24: Ashley Lewis - Bug 8105
        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialField_AndRegion_Expected_ResultReplacesText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 5, 
                InputText = "[[fie", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            Assert.AreEqual("[[recset().field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset().field]]", context));
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndNoRegion_Expected_ResultReplacesText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 4, 
                InputText = "scal", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default, 
                State = true 
            };

            Assert.AreEqual("[[scalar]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[scalar]]", context));
        }
        
        //Bug 8437
        [TestMethod]
// ReSharper disable InconsistentNaming
        public void NoFieldResultInsertion_AndMatchOnMiddleOfRecsetName_Expected_ResultReplacesText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 5, 
                InputText = "[[set", DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            Assert.AreEqual("[[recset()]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset()]]", context));
        }
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void NoFieldResultInsertion_Where_CaretPositionIsZero_Expected_DoesNotThrowException()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 0, 
                InputText = "", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            //The only reason this logic needs to run is to check that a zero caret position doesn't crash it!!!
            string actual = new DefaultIntellisenseProvider().PerformResultInsertion("", context);
            Assert.AreEqual("", actual);
        }
        [TestMethod]
// ReSharper disable InconsistentNaming
        public void NoFieldStarResultInsertion_AndMatchOnRecsetName_AndRegion_Expected_ResultReplacesText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 5, 
                InputText = "[[rec", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default, 
                State = true 
            };

            Assert.AreEqual("[[recset(*)]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset(*)]]", context));
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_Expected_ResultAppendsText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 14, 
                InputText = "[[recset([[sca", 
                DesiredResultSet = 0, 
                State = true
            };

            Assert.AreEqual("[[recset([[scalar]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[scalar]]", context));
        }
       
        //Bug 6103
        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_Expected_ResultInsertsText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 14, 
                InputText = "[[recset([[sca).field]]", 
                DesiredResultSet = 0, State = true 
            };

            Assert.AreEqual("[[recset([[scalar]]).field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[scalar]]", context));
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialRecset_AndRegion_Expected_ResultInsertsText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 14, 
                InputText = "[[recset([[ano).field]]", 
                DesiredResultSet = 0, 
                State = true 
            };
            
            Assert.AreEqual("[[recset([[anotherRecset().newfield]]).field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[anotherRecset().newfield]]", context));
        }

        [TestMethod]
// ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_AtDeepWithinExtaIndex_Expected_ResultInsertsText()
// ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 23,
                InputText = "[[recset([[recset([[sca).field]]).field]]",
                DesiredResultSet = 0,
                State = true
            };

            Assert.AreEqual("[[recset([[recset([[scalar]]).field]]).field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[scalar]]", context));
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_AndWithinPluses_Expected_ResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 32, 
                InputText = "[[recset().field]]+[[Scalar]]+[[+[[fail]]", 
                DesiredResultSet = 0 
            };

            Assert.AreEqual("[[recset().field]]+[[Scalar]]+[[Car]]+[[fail]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[Car]]", context));
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_AndAfterPluses_Expected_ResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 32, 
                InputText = "[[recset().field]]+[[Scalar]]+[[", 
                DesiredResultSet = 0 
            };

            Assert.AreEqual("[[recset().field]]+[[Scalar]]+[[Car]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[Car]]", context));
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialScalar_AndRegion_AndAfterSum_Expected_ResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "Sum([[Sca", 
                DesiredResultSet = 0 
            };

            Assert.AreEqual("Sum([[Scalar]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[Scalar]]", context));
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialField_AndRegion_AndAfterIndexed_Expected_ResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 15, 
                InputText = "[[recset(3).fie", 
                DesiredResultSet = 0 
            };
            
            Assert.AreEqual("[[recset(3).field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset(3).field]]", context));
        }

        // BUG 8755
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertionWithPartialFieldAndRegionAndAfterBlankIndexExpectedResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext
            {
                CaretPosition = 14,
                InputText = "[[recset().fie",
                DesiredResultSet = 0
            };

            Assert.AreEqual("[[recset().field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset().field]]", context));
        }

        // BUG 8755
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertionWithPartialFieldAndRegionAndAfterStarIndexExpectedResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 15, 
                InputText = "[[recset(*).fie", 
                DesiredResultSet = 0 
            };

            Assert.AreEqual("[[recset(*).field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset(*).field]]", context));
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void PerformResultInsertion_With_PartialField_AndAfterIndex_Expected_ResultInsertsText()
        // ReSharper restore InconsistentNaming
        {
            var context = new IntellisenseProviderContext 
            { 
                CaretPosition = 24, 
                InputText = "[[recset([[scalar]]).fie", 
                DesiredResultSet = 0, 
                State = true 
            };

            Assert.AreEqual("[[recset([[scalar]]).field]]", new DefaultIntellisenseProvider().PerformResultInsertion("[[recset([[scalar]]).field]]", context));
        }

        //Bug 8736
        [TestMethod]
        public void PerformResultInsertionWithPartialScalarAndFullRegionExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext 
            { 
                CaretPosition = 3, 
                InputText = "[[S]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default 
            };

            string exprected = "[[Scalar]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[Scalar]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8736
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetAndFullRegionExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext 
            { 
                CaretPosition = 7, 
                InputText = "[[City(]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default, 
                State = true
            };

            string exprected = "[[City().GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City().GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8736
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetAndPartialRegionExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City().", 
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch 
            };

            string exprected = "[[City().GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City().GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8736
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetWithIndexAndPartialRegionExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext 
            { 
                CaretPosition = 10, 
                InputText = "[[City(4).", 
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch 
            };

            string exprected = "[[City(4).GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City(4).GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8755
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetAndPartialRegionAndStarIndexExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext
            {
                CaretPosition = 10,
                InputText = "[[City(*).",
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            string exprected = "[[City(*).GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City(*).GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8736
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetWithClosedBracketsAndFullRegionExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext 
            { 
                CaretPosition = 9, 
                InputText = "[[City().]]", 
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch 
            };

            string exprected = "[[City().GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City().GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8755
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetWithClosedBracketsAndFullRegionAnNumberIndexExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext
            {
                CaretPosition = 11,
                InputText = "[[City(44).]]",
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            string exprected = "[[City(44).GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City(44).GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8755
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetWithClosedBracketsAndFullRegionAnStarIndexExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext
            {
                CaretPosition = 10,
                InputText = "[[City(*).]]",
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            string exprected = "[[City(*).GeoLocation]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City(*).GeoLocation]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //Bug 8736
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetExpectedResultInsertsText()
        {
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext 
            { 
                CaretPosition = 4, 
                InputText = "City", 
                DesiredResultSet = IntellisenseDesiredResultSet.Default, 
                State = true
            };

            string exprected = "[[City()]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[City()]]", intellisenseProviderContext);

            Assert.AreEqual(exprected, actual);
        }

        //2013.06.14: Ashley Lewis for 8760 - inserting recset to other regions
        [TestMethod]
        public void PerformResultInsertionWithPartialRecordsetAndExistingRegionExpectedResultInsertsText()
        {
            var inputText = "[[scalar]][[rec";
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext
            {
                CaretPosition = inputText.Length,
                InputText = inputText,
                DesiredResultSet = IntellisenseDesiredResultSet.Default,
                State = true
            };

            string expected = inputText + "set().field]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[recset().field]]", intellisenseProviderContext);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void PerformResultInsertionWithAlmostCompleteRecordsetAndExistingRegionExpectedResultInsertsText()
        {
            var inputText = "[[scalar]][[recset().fiel]]";
            DefaultIntellisenseProvider defaultIntellisenseProvider = new DefaultIntellisenseProvider();
            IntellisenseProviderContext intellisenseProviderContext = new IntellisenseProviderContext
            {
                CaretPosition = inputText.Length-2,
                InputText = inputText,
                DesiredResultSet = IntellisenseDesiredResultSet.Default,
                State = true
            };

            string expected = "[[scalar]][[recset().field]]";
            string actual = defaultIntellisenseProvider.PerformResultInsertion("[[recset().field]]", intellisenseProviderContext);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #endregion

        #region CalculateIntellisenseProvider Tests

        [TestMethod]
        public void GetIntellisenseResults_PartialMethodMatch_Expected_ClosestMatchesReturned()
        {
            IntellisenseProviderContext context = new IntellisenseProviderContext
            {
                CaretPosition = 2,
                InputText = "su",
                IsInCalculateMode = true,
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            var calculateIntellisenseProvider = new CalculateIntellisenseProvider();
            IList<IntellisenseProviderResult> results = calculateIntellisenseProvider.GetIntellisenseResults(context);

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void GetIntellisenseResults_FullMethodMatch_GivenMethodWithParams_Expected_AllAvailableFunctionsReturned()
        {
            string intellisenseText = "sum(10,20,30)";
            IntellisenseProviderContext context = new IntellisenseProviderContext
            {
                CaretPosition = intellisenseText.Length,
                InputText = intellisenseText,
                IsInCalculateMode = true,
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            var calculateIntellisenseProvider = new CalculateIntellisenseProvider();
            IList<IntellisenseProviderResult> results = calculateIntellisenseProvider.GetIntellisenseResults(context);
            // Create function repo as all the functions will be available here
            IFrameworkRepository<IFunction> functionRepo = MathOpsFactory.FunctionRepository();
            functionRepo.Load();
            Assert.AreEqual(functionRepo.All().Count, results.Count);
        }


        // BUG 7858
        [TestMethod]
        public void GetIntellisenseResult_MethodExpectingNoParams_Given_NoParams_Expected_NoErrorInResultSet()
        {
            string intellisenseText = "pi()";
            IntellisenseProviderContext context = new IntellisenseProviderContext
            {
                CaretPosition = intellisenseText.Length,
                InputText = intellisenseText,
                IsInCalculateMode = true,
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            var calculateIntellisenseProvider = new CalculateIntellisenseProvider();
            IList<IntellisenseProviderResult> results = calculateIntellisenseProvider.GetIntellisenseResults(context);
            Assert.IsFalse(results[0].IsError);
        }

        // BUG 7858
        [TestMethod]
        public void GetIntellisenseResult_MethodExpectingNoParams_Given_Params_Expected_ErrorInResultSet()
        {
            string intellisenseText = "pi(1)";
            IntellisenseProviderContext context = new IntellisenseProviderContext
            {
                CaretPosition = intellisenseText.Length,
                InputText = intellisenseText,
                IsInCalculateMode = true,
                DesiredResultSet = IntellisenseDesiredResultSet.ClosestMatch
            };

            var calculateIntellisenseProvider = new CalculateIntellisenseProvider();
            IList<IntellisenseProviderResult> results = calculateIntellisenseProvider.GetIntellisenseResults(context);
            Assert.IsTrue(results[0].IsError);
        }

        #endregion CalculateIntellisenseProvider Tests
    }
}
