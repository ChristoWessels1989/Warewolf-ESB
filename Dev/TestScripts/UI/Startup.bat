REM ********************************************************************************************************************
REM * Hi-Jack the Auto Build Variables by QtAgent since this is injected after it has REM * setup
REM * Open the autogenerated qtREM * setup in the test run location of
REM * C:\Users\IntegrationTester\AppData\Local\VSEQT\QTAgent\...
REM * For example:
REM * set DeploymentDirectory=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1\DEPLOY~1
REM * set TestRunDirectory=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1
REM * set TestRunResultsDirectory=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1\Results\RSAKLF~1
REM * set TotalAgents=5
REM * set AgentWeighting=100
REM * set AgentLoadDistributor=Microsoft.VisualStudio.TestTools.Execution.AgentLoadDistributor
REM * set AgentId=1
REM * set TestDir=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1
REM * set ResultsDirectory=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1\Results
REM * set DataCollectionEnvironmentContext=Microsoft.VisualStudio.TestTools.Execution.DataCollectionEnvironmentContext
REM * set TestLogsDir=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1\Results\RSAKLF~1
REM * set ControllerName=rsaklfsvrtfsbld:6901
REM * set TestDeploymentDir=C:\Users\INTEGR~1\AppData\Local\VSEQT\QTAgent\54371B~1\RSAKLF~1\DEPLOY~1
REM * set AgentName=RSAKLFTST7X64-3
REM ********************************************************************************************************************

REM ** Kill The Warewolf ;) **
sc stop "Warewolf Server"
taskkill /im "Warewolf Server.exe"
taskkill /im "Warewolf Studio.exe"

REM  Wait 10 seconds ;)
ping -n 10 127.0.0.1 > nul

REM ** Start Warewolf server from deployed binaries built in debug config**
START "%DeploymentDirectory%\ServerbinDebug\Warewolf Server.exe" /D %DeploymentDirectory%\ServerbinDebug "Warewolf Server.exe"

REM  Wait 10 seconds ;)
ping -n 10 127.0.0.1 > nul

REM ** Start Warewolf studio from deployed binaries built in debug config**
START "%DeploymentDirectory%\StudiobinDebug\Warewolf Studio.exe" /D %DeploymentDirectory%\StudiobinDebug "Warewolf Studio.exe"

REM  Wait 30 seconds ;)
ping -n 30 127.0.0.1 > nul

exit 0