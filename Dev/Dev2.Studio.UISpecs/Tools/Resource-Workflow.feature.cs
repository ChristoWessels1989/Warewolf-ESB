﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.34209
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Dev2.Studio.UI.Specs.Tools
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute()]
    public partial class Resource_ServiceFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Resource-Workflow.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Resource-Service", "In order to be able to use Service\r\nAs a warewolf user\r\nI want to be able to Drag" +
                    " Service Tool onto design surface", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "Resource-Service")))
            {
                Dev2.Studio.UI.Specs.Tools.Resource_ServiceFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Drag Workflow to design surface and checking services are not showing in workflow" +
            " resource picker")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Resource-Service")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("Workflow")]
        public virtual void DragWorkflowToDesignSurfaceAndCheckingServicesAreNotShowingInWorkflowResourcePicker()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Drag Workflow to design surface and checking services are not showing in workflow" +
                    " resource picker", new string[] {
                        "Workflow"});
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
    testRunner.Given("I have Warewolf running", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.Given("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.Given("I click \"EXPLORERFILTERCLEARBUTTON\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 10
 testRunner.Given("I click \"EXPLORERCONNECTCONTROL\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.Given("I click \"U_UI_ExplorerServerCbx_AutoID_localhost\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 13
 testRunner.Given("I click new \"Workflow\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 15
 testRunner.When("I send \"workflow\" to \"TOOLBOX,PART_SearchBox\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 16
 testRunner.Given("I drag \"TOOLWORKFLOW\" onto \"ACTIVETAB,UI_WorkflowDesigner_AutoID,UserControl_1,sc" +
                    "rollViewer,ActivityTypeDesigner,WorkflowItemPresenter,StartSymbol\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 18
    testRunner.And("I send \"Control Flow - Decision\" to \"UI_SelectServiceWindow_AutoID,UI_NavigationV" +
                    "iewUserControl_AutoID,UI_DatalistFilterTextBox_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 19
 testRunner.Then("\"RESOURCEPICKERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Decision_AutoID\" is v" +
                    "isible", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 20
 testRunner.Given("I click \"TOOLWORKFLOWRISOURCEPICKFILTER\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 22
    testRunner.And("I send \"DBSERVICES\" to \"UI_SelectServiceWindow_AutoID,UI_NavigationViewUserContro" +
                    "l_AutoID,UI_DatalistFilterTextBox_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
 testRunner.Given("\"RESOURCEPICKERFOLDERS\" is invisible within \"2\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 24
 testRunner.And("I send \"{ESC}\" to \"\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 25
 testRunner.And("I drag \"TOOLWORKFLOW\" onto \"ACTIVETAB,UI_WorkflowDesigner_AutoID,UserControl_1,sc" +
                    "rollViewer,ActivityTypeDesigner,WorkflowItemPresenter,StartSymbol\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 27
 testRunner.And("I send \"Decision Testing\" to \"UI_SelectServiceWindow_AutoID,UI_NavigationViewUser" +
                    "Control_AutoID,UI_DatalistFilterTextBox_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
 testRunner.And("I double click \"RESOURCEPICKERFOLDERS,UI_Acceptance Testing Resources_AutoID,UI_D" +
                    "ecision Testing_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
 testRunner.And("I click \"UI_SelectServiceWindow_AutoID,UI_SelectServiceOKButton_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
 testRunner.And("\"WORKSURFACE,Acceptance Testing Resources\\Decision Testing(ServiceDesigner)\" is v" +
                    "isible within \"2\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
