﻿using System;
using System.Activities;
using System.Collections.Generic;
using Dev2.Activities.Debug;
using Dev2.Data.Factories;
using Dev2.DataList.Contract;
using Dev2.DataList.Contract.Binary_Objects;
using Dev2.DataList.Contract.Builders;
using Dev2.DataList.Contract.Value_Objects;
using Dev2.Diagnostics;
using Dev2.Util;
using Unlimited.Applications.BusinessDesignStudio.Activities;
using Unlimited.Applications.BusinessDesignStudio.Activities.Utilities;

// ReSharper disable CheckNamespace
namespace Dev2.Activities
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Activity used for executing JavaScript through a tool
    /// </summary>
    public class DsfScriptingJavaScriptActivity : DsfActivityAbstract<string>
    {
        #region Fields

        new List<DebugItem> _debugInputs;
        new List<DebugItem> _debugOutputs;

        #endregion

        #region Properties

        [FindMissing]
        [Inputs("Script")]
        public string Script { get; set; }

        [FindMissing]
        [Outputs("Result")]
        public new string Result { get; set; }

        #endregion

        #region Ctor

        public DsfScriptingJavaScriptActivity()
            : base("JavaScript")
        {
            Script = string.Empty;
            Result = string.Empty;
        }

        #endregion

        #region Overrides of DsfNativeActivity<string>

        /// <summary>
        /// When overridden runs the activity's execution logic 
        /// </summary>
        /// <param name="context">The context to be used.</param>
        protected override void OnExecute(NativeActivityContext context)
        {
            _debugInputs = new List<DebugItem>();
            _debugOutputs = new List<DebugItem>();
            IDSFDataObject dataObject = context.GetExtension<IDSFDataObject>();

            IDataListCompiler compiler = DataListFactory.CreateDataListCompiler();

            Guid dlID = dataObject.DataListID;
            ErrorResultTO allErrors = new ErrorResultTO();
            ErrorResultTO errorResultTO = new ErrorResultTO();
            Guid executionId = dlID;
            allErrors.MergeErrors(errorResultTO);


            try
            {
                if(!errorResultTO.HasErrors())
                {
                    IDev2DataListUpsertPayloadBuilder<string> toUpsert = Dev2DataListBuilderFactory.CreateStringDataListUpsertBuilder(true);
                    IDev2IteratorCollection colItr = Dev2ValueObjectFactory.CreateIteratorCollection();

                    if(allErrors.HasErrors())
                    {
                        return;
                    }
                    IBinaryDataListEntry scriptEntry = compiler.Evaluate(executionId, enActionType.User, Script, false, out errorResultTO);
                    allErrors.MergeErrors(errorResultTO);
                    if(allErrors.HasErrors())
                    {
                        return;
                    }

                    if(dataObject.IsDebugMode())
                    {
                        AddDebugInputItem(Script, scriptEntry, executionId);
                    }

                    int iterationCounter = 0;

                    while(colItr.HasMoreData())
                    {

                        dynamic value = null;

                        //2013.06.03: Ashley Lewis for bug 9498 - handle multiple regions in result
                        foreach(var region in DataListCleaningUtils.SplitIntoRegions(Result))
                        {
                            toUpsert.Add(region, null);
                            toUpsert.FlushIterationFrame();

                            if(dataObject.IsDebugMode())
                            {
                                // ReSharper disable ExpressionIsAlwaysNull
                                AddDebugOutputItem(new DebugOutputParams(region, value, executionId, iterationCounter));
                                // ReSharper restore ExpressionIsAlwaysNull
                            }
                        }

                        iterationCounter++;
                    }

                    compiler.Upsert(executionId, toUpsert, out errorResultTO);
                    allErrors.MergeErrors(errorResultTO);
                }
            }
            catch(Exception e)
            {
                if(e.GetType() == typeof(NullReferenceException))
                {
                    allErrors.AddError("There was an error when returning a value from the javascript, remember to use the 'Return' keyword when returning the result");
                }
                else
                {
                    allErrors.AddError(e.Message);
                }
            }
            finally
            {
                // Handle Errors
                var hasErrors = allErrors.HasErrors();
                if(hasErrors)
                {
                    DisplayAndWriteError("DsfScriptingJavaScriptActivity", allErrors);
                    compiler.UpsertSystemTag(dataObject.DataListID, enSystemTag.Dev2Error, allErrors.MakeDataListReady(), out errorResultTO);
                }

                if(dataObject.IsDebugMode())
                {
                    if(hasErrors)
                    {
                        AddDebugOutputItem(new DebugItemStaticDataParams("", Result, ""));
                    }

                    DispatchDebugState(context, StateType.Before);
                    DispatchDebugState(context, StateType.After);
                }
            }

        }

        public override void UpdateForEachInputs(IList<Tuple<string, string>> updates, NativeActivityContext context)
        {
            foreach(Tuple<string, string> t in updates)
            {

                if(t.Item1 == Script)
                {
                    Script = t.Item2;
                }
            }
        }

        public override void UpdateForEachOutputs(IList<Tuple<string, string>> updates, NativeActivityContext context)
        {
            if(updates.Count == 1)
            {
                Result = updates[0].Item2;
            }
        }

        #endregion

        #region Private Methods


        private void AddDebugInputItem(string scriptExpression, IBinaryDataListEntry scriptEntry, Guid executionId)
        {
            AddDebugInputItem(new DebugItemVariableParams(scriptExpression, "Script", scriptEntry, executionId));
        }
        
        #endregion

        #region Get Debug Inputs/Outputs

        public override List<DebugItem> GetDebugInputs(IBinaryDataList dataList)
        {
            foreach(IDebugItem debugInput in _debugInputs)
            {
                debugInput.FlushStringBuilder();
            }
            return _debugInputs;
        }

        public override List<DebugItem> GetDebugOutputs(IBinaryDataList dataList)
        {
            foreach(IDebugItem debugOutput in _debugOutputs)
            {
                debugOutput.FlushStringBuilder();
            }
            return _debugOutputs;
        }

        #endregion Get Inputs/Outputs

        #region GetForEachInputs/Outputs

        public override IList<DsfForEachItem> GetForEachInputs()
        {
            return GetForEachItems(Script);
        }

        public override IList<DsfForEachItem> GetForEachOutputs()
        {
            return GetForEachItems(Result);
        }

        #endregion
    }
}

