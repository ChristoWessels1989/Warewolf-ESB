﻿using System;
using System.Windows.Forms;
using Dev2.CodedUI.Tests.TabManagerUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.DocManagerUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.ExplorerUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.PluginServiceWizardUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.RibbonUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.ToolboxUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.WorkflowDesignerUIMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.DatabaseServiceWizardUIMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.DatabaseSourceUIMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.DebugUIMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.EmailSourceWizardUIMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.OutputUIMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.PluginSourceMapClasses;
using Dev2.Studio.UI.Tests.UIMaps.SaveDialogUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev2.Studio.UI.Tests
{
    /// <summary>
    ///     These are UI tests based on using a remote server
    /// </summary>
    [CodedUITest]
    public class RemoteServerUiTests
    {
        #region Fields

        const string RemoteServerName = "RemoteConnection";
        const string LocalHostServerName = "localhost";
        const string ExplorerTab = "Explorer";
        DatabaseServiceWizardUIMap _databaseServiceWizardUiMap;
        DatabaseSourceUIMap _databaseSourceUiMap;
        DebugUIMap _debugUiMap;
        DocManagerUIMap _docManagerMap;
        EmailSourceWizardUIMap _emailSourceWizardUiMap;
        ExplorerUIMap _explorerUiMap;
        PluginServiceWizardUIMap _pluginServiceWizardUiMap;
        PluginSourceMap _pluginSourceMap;
        RibbonUIMap _ribbonUiMap;
        SaveDialogUIMap _saveDialogUiMap;
        TabManagerUIMap _tabManagerDesignerUiMap;
        ToolboxUIMap _toolboxUiMap;
        WorkflowDesignerUIMap _workflowDesignerUiMap;
        OutputUIMap _outputUiMap;

        #endregion

        #region Properties

        ExplorerUIMap ExplorerUiMap { get { return _explorerUiMap ?? (_explorerUiMap = new ExplorerUIMap()); } }
        public TabManagerUIMap TabManagerUiMap { get { return _tabManagerDesignerUiMap ?? (_tabManagerDesignerUiMap = new TabManagerUIMap()); } }
        public DocManagerUIMap DocManagerUiMap { get { return _docManagerMap ?? (_docManagerMap = new DocManagerUIMap()); } }
        public WorkflowDesignerUIMap WorkflowDesignerUiMap { get { return _workflowDesignerUiMap ?? (_workflowDesignerUiMap = new WorkflowDesignerUIMap()); } }
        public DatabaseSourceUIMap DatabaseSourceUiMap { get { return _databaseSourceUiMap ?? (_databaseSourceUiMap = new DatabaseSourceUIMap()); } }
        public SaveDialogUIMap SaveDialogUiMap { get { return _saveDialogUiMap ?? (_saveDialogUiMap = new SaveDialogUIMap()); } }
        public EmailSourceWizardUIMap EmailSourceWizardUiMap { get { return _emailSourceWizardUiMap ?? (_emailSourceWizardUiMap = new EmailSourceWizardUIMap()); } }
        public PluginSourceMap PluginSourceMap { get { return _pluginSourceMap ?? (_pluginSourceMap = new PluginSourceMap()); } }
        public DatabaseServiceWizardUIMap DatabaseServiceWizardUiMap { get { return _databaseServiceWizardUiMap ?? (_databaseServiceWizardUiMap = new DatabaseServiceWizardUIMap()); } }
        public PluginServiceWizardUIMap PluginServiceWizardUiMap { get { return _pluginServiceWizardUiMap ?? (_pluginServiceWizardUiMap = new PluginServiceWizardUIMap()); } }
        public ToolboxUIMap ToolboxUiMap { get { return _toolboxUiMap ?? (_toolboxUiMap = new ToolboxUIMap()); } }
        public DebugUIMap DebugUiMap { get { return _debugUiMap ?? (_debugUiMap = new DebugUIMap()); } }
        public RibbonUIMap RibbonUiMap { get { return _ribbonUiMap ?? (_ribbonUiMap = new RibbonUIMap()); } }
        public OutputUIMap OutputUiMap { get { return _outputUiMap ?? (_outputUiMap = new OutputUIMap()); } }
        
        #endregion

        #region Test Methods

        [TestCleanup]
        public void TestCleanup()
        {
            TabManagerUiMap.CloseAllTabs();
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_ConnectToRemoteServerFromExplorer_RemoteServerConnected()
        {
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClickServerInServerDDL(RemoteServerName);
            var selectedSeverName = ExplorerUiMap.SelectedSeverName();
            Assert.AreEqual(RemoteServerName, selectedSeverName);
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_CreateRemoteWorkFlow_WorkflowIsCreated()
        {
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClickServerInServerDDL(RemoteServerName);
            CreateWorkflow();
            var activeTabName = TabManagerUiMap.GetActiveTabName();
            Assert.IsTrue(activeTabName.Contains("Unsaved"));
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_EditRemoteWorkFlow_WorkflowIsEdited()
        {
            const string TextToSearchWith = "Find Records";
            OpenWorkFlow(RemoteServerName, "WORKFLOWS", "TESTS", TextToSearchWith);
            var uiControl = WorkflowDesignerUiMap.FindControlByAutomationId(TabManagerUiMap.GetActiveTab(), "Assign");
            var p = WorkflowDesignerUiMap.GetPointUnderControl(uiControl);
            ToolboxUiMap.DragControlToWorkflowDesigner("MultiAssign", p);
            var activeTabName = TabManagerUiMap.GetActiveTabName();
            Assert.IsTrue(activeTabName.Contains("Find Records - RemoteConnection"));
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_ViewRemoteWorkFlowInBrowser_WorkflowIsExecuted()
        {
            const string TextToSearchWith = "Find Records";
            OpenWorkFlow(RemoteServerName, "WORKFLOWS", "TESTS", TextToSearchWith);
            OpenMenuItem("View in Browser");
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_DragAndDropWorkflowFromRemoteServerOnALocalHostCreatedWorkflow_WorkFlowIsDropped()
        {
            const string TextToSearchWith = "Internal Recursive Copy";
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            //Ensure that we're in localhost
            ExplorerUiMap.ClickServerInServerDDL(LocalHostServerName);
            //Create a workfliow
            RibbonUiMap.CreateNewWorkflow();
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClickServerInServerDDL(RemoteServerName);

            var point = WorkflowDesignerUiMap.GetStartNodeBottomAutoConnectorPoint();
            ExplorerUiMap.ClearExplorerSearchText();

            ExplorerUiMap.EnterExplorerSearchText(TextToSearchWith);
            ExplorerUiMap.DragControlToWorkflowDesigner(RemoteServerName, "WORKFLOWS", "UTILITY", TextToSearchWith, point);

            OpenMenuItem("Debug");
            SendKeys.SendWait("{F5}");

            Assert.IsFalse(OutputUIMap.IsAnyStepsInError());
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_DragAndDropWorkflowFromALocalServerOnARemoteServerCreatedWorkflow_WorkFlowIsDropped()
        {
            const string TextToSearchWith = "Utility - Assign";
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClickServerInServerDDL(RemoteServerName);
            //Create a workfliow
            RibbonUiMap.CreateNewWorkflow();
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            //Connect to local server
            ExplorerUiMap.ClickServerInServerDDL(LocalHostServerName);

            var point = WorkflowDesignerUiMap.GetStartNodeBottomAutoConnectorPoint();
            ExplorerUiMap.ClearExplorerSearchText();

            ExplorerUiMap.EnterExplorerSearchText(TextToSearchWith);
            ExplorerUiMap.DragControlToWorkflowDesigner(LocalHostServerName, "WORKFLOWS", "EXAMPLES", TextToSearchWith, point);

            OpenMenuItem("Debug");
            SendKeys.SendWait("{F5}");

            Assert.IsFalse(OutputUIMap.IsAnyStepsInError());
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_OpenWorkflowOnRemoteServerAndOpenWorkflowWithSameNameOnLocalHost_WorkflowIsOpened()
        {
            const string TextToSearchWith = "Find Records";

            OpenWorkFlow(RemoteServerName, "WORKFLOWS", "TESTS", TextToSearchWith);
            var remoteTab = TabManagerUiMap.FindTabByName(TextToSearchWith + " - " + RemoteServerName);
            Assert.IsNotNull(remoteTab);

            OpenWorkFlow(LocalHostServerName, "WORKFLOWS", "TESTS", TextToSearchWith);
            var localHostTab = TabManagerUiMap.FindTabByName(TextToSearchWith);
            Assert.IsNotNull(localHostTab);
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_DebugARemoteWorkflowWhenLocalWorkflowWithSameNameIsOpen_WorkflowIsExecuted()
        {
            const string TextToSearchWith = "Find Records";

            OpenWorkFlow(LocalHostServerName, "WORKFLOWS", "TESTS", TextToSearchWith);
            var localHostTab = TabManagerUiMap.FindTabByName(TextToSearchWith);
            Assert.IsNotNull(localHostTab);

            OpenWorkFlow(RemoteServerName, "WORKFLOWS", "TESTS", TextToSearchWith);

            RibbonUiMap.Debug();

            var remoteTab = TabManagerUiMap.FindTabByName(TextToSearchWith + " - " + RemoteServerName);
            Assert.IsNotNull(remoteTab);
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_EditRemoteDbSource_DbSourceIsEdited()
        {
            const string TextToSearchWith = "DBSource";
            OpenWorkFlow(RemoteServerName, "SOURCES", "REMOTETESTS", TextToSearchWith);
            DatabaseSourceUiMap.ClickSaveConnection();
            SaveDialogUiMap.ClickSave();
            //Not sure how to assert here
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_EditRemoteDbService_DbServiceIsEdited()
        {
            const string TextToSearchWith = "DBService";
            OpenWorkFlow(RemoteServerName, "SERVICES", "REMOTEUITESTS", TextToSearchWith);
            DatabaseServiceWizardUiMap.KeyboardOK();
            SaveDialogUiMap.ClickSave();
            //Not sure how to assert here
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_EditRemoteEmailSource_EmailSourceIsEdited()
        {
            const string TextToSearchWith = "EmailSource";
            OpenWorkFlow(RemoteServerName, "SOURCES", "REMOTETESTS", TextToSearchWith);
            EmailSourceWizardUiMap.ClickTestConnection();
            EmailSourceWizardUiMap.EnterEmailAddressAndSend();
            EmailSourceWizardUiMap.ClickSaveEmailSource();
            SaveDialogUiMap.ClickSave();
            //Not sure how to assert here
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_EditRemotePluginSource_PluginSourceIsEdited()
        {
            const string TextToSearchWith = "PluginSource";
            OpenWorkFlow(RemoteServerName, "SOURCES", "REMOTETESTS", TextToSearchWith);
            PluginSourceMap.ClickSavePlugin();
            SaveDialogUiMap.ClickSave();
            //Not sure how to assert here
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_EditRemotePluginService_PluginServiceIsEdited()
        {
            const string TextToSearchWith = "PluginService";
            OpenWorkFlow(RemoteServerName, "SERVICES", "REMOTEUITESTS", TextToSearchWith);
            PluginServiceWizardUiMap.ClickTestAndOk();
            SaveDialogUiMap.ClickSave();
            //Not sure how to assert here
        }

        [TestMethod]
        [Owner("Tshepo Ntlhokoa")]
        [TestCategory("RemoteServerUITests")]
        public void RemoteServerUITests_AddExecuteRenameAndDeleteALocalWorlFlow_ProcessCompletesSuccessfully()
        {
            ProcessAWorkflow(LocalHostServerName, "WORKFLOWS", "Unassigned");
            ProcessAWorkflow(RemoteServerName, "WORKFLOWS", "Unassigned");
        }

        void ProcessAWorkflow(string serverName, string serviceType, string folderName)
        {
            //CREATE A WORKFLOW
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClickServerInServerDDL(serverName);
            CreateWorkflow();
            var point = WorkflowDesignerUiMap.GetStartNodeBottomAutoConnectorPoint();
            ToolboxUiMap.DragControlToWorkflowDesigner("MultiAssign", point);

            //SAVE A WORKFLOW
            OpenMenuItem("Save");
            const string InitialName = "Initial_Name_WF_1";
            EnternameAndSave(InitialName);

            //EXECUTE A WORKFLOW
            OpenMenuItem("Debug");
            SendKeys.SendWait("{F5}");
            TabManagerUiMap.CloseAllTabs();

            //OPEN AND RENAME A WORKFLOW
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClearExplorerSearchText();
            ExplorerUiMap.EnterExplorerSearchText(InitialName);
            ExplorerUiMap.RightClickRenameProject(serverName, serviceType, folderName, InitialName);
            const string RenameTo = "RenameTo_Name_WF_1";
            Keyboard.SendKeys(RenameTo + "{ENTER}");

            //DELETE A WORKFLOW
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClearExplorerSearchText();
            ExplorerUiMap.EnterExplorerSearchText(RenameTo);
            ExplorerUiMap.RightClickDeleteProject(serverName, serviceType, folderName, RenameTo);
            Keyboard.SendKeys("{ENTER}{ENTER}");
        }
        #endregion

        #region Utils

        static void EnternameAndSave(string tempName)
        {
            Keyboard.SendKeys("{TAB}{TAB}{TAB}{TAB}{TAB}{ENTER}");
            Keyboard.SendKeys(tempName);
            Keyboard.SendKeys("{TAB}{TAB}{ENTER}");
            Playback.Wait(6000);
        }

        void OpenMenuItem(string itemType)
        {
            RibbonUiMap.ClickRibbonMenuItem(itemType);
        }

        void OpenWorkFlow(string serverName, string serviceType, string foldername, string textToSearchWith)
        {
            DocManagerUIMap.ClickOpenTabPage(ExplorerTab);
            ExplorerUiMap.ClickServerInServerDDL(serverName);
            ExplorerUiMap.ClearExplorerSearchText();
            ExplorerUiMap.EnterExplorerSearchText(textToSearchWith);
            ExplorerUiMap.DoubleClickOpenProject(serverName, serviceType, foldername, textToSearchWith);
        }

        void CreateWorkflow()
        {
            Keyboard.SendKeys(DocManagerUiMap.UIBusinessDesignStudioWindow, "{CTRL}W");
        }

        #endregion
    }
}