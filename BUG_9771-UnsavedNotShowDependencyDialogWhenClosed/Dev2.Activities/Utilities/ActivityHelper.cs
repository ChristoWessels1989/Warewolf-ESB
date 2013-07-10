﻿using System;
using System.Activities.Presentation.Model;
using Dev2.Common;
using Dev2.Data.SystemTemplates.Models;

namespace Dev2.Utilities
{
    public static class ActivityHelper
    {
        public static void InjectExpression(Dev2Switch ds, ModelProperty activityExpression)
        {
            if(ds == null) return;

            // FetchSwitchData
            string expressionToInject = string.Join("", GlobalConstants.InjectedSwitchDataFetch,
                                                    "(\"", ds.SwitchVariable, "\",",
                                                    GlobalConstants.InjectedDecisionDataListVariable,
                                                    ")");
            if(activityExpression != null)
            {
                activityExpression.SetValue(expressionToInject);
            }
        }

        public static void InjectExpression(Dev2DecisionStack ds, ModelProperty activityExpression)
        {
            if(ds == null) return;

            string modelData = ds.ToVBPersistableModel();
            string expressionToInject = string.Join("", GlobalConstants.InjectedDecisionHandler, "(\"",
                                                    modelData, "\",",
                                                    GlobalConstants.InjectedDecisionDataListVariable, ")");

            if(activityExpression != null)
            {
                activityExpression.SetValue(expressionToInject);
            }
        }

        public static string ExtractData(string val)
        {
            if(val.IndexOf(GlobalConstants.InjectedSwitchDataFetch, StringComparison.Ordinal) >= 0)
            {
                // Time to extract the data
                int start = val.IndexOf("(", StringComparison.Ordinal);
                if(start > 0)
                {
                    int end = val.IndexOf(@""",AmbientData", StringComparison.Ordinal);

                    if(end > start)
                    {
                        start += 2;
                        val = val.Substring(start, (end - start));

                        // Convert back for usage ;)
                        val = Dev2DecisionStack.FromVBPersitableModelToJSON(val);
                    }
                }
            }
            return val;
        }
        public static void SetSwitchKeyProperty(Dev2Switch ds, ModelItem switchCase)
        {
            if(ds != null)
            {
                ModelProperty keyProperty = switchCase.Properties["Key"];

                if(keyProperty != null)
                {
                    keyProperty.SetValue(ds.SwitchVariable);

                }
            }
        }

        public static void SetArmTextDefaults(Dev2DecisionStack dds)
        {
            if(string.IsNullOrEmpty(dds.TrueArmText.Trim()))
            {
                dds.TrueArmText = GlobalConstants.DefaultTrueArmText;
            }

            if(string.IsNullOrEmpty(dds.FalseArmText.Trim()))
            {
                dds.FalseArmText = GlobalConstants.DefaultFalseArmText;
            }
        }

        public static void SetArmText(ModelItem decisionActivity, Dev2DecisionStack dds)
        {
            SetArmText(decisionActivity, GlobalConstants.TrueArmPropertyText, dds.TrueArmText);
            SetArmText(decisionActivity, GlobalConstants.FalseArmPropertyText, dds.FalseArmText);
        }

        public static void SetArmText(ModelItem decisionActivity, string armType, string val)
        {
            ModelProperty tArm = decisionActivity.Properties[armType];

            if(tArm != null)
            {
                tArm.SetValue(val);
            }
        }

        public static ModelItem GetActivityFromWrapper<T>(Tuple<ModelItem, T> wrapper,
                                                string expressionProperty)
        {

            ModelItem activity = wrapper.Item1;
            if(activity == null) return null;
            ModelProperty property = activity.Properties[expressionProperty];
            return property == null ? null : property.Value;
        }

        public static ModelItem GetRootActivityFromWrapper<T>(Tuple<ModelItem, T> wrapper)
        {

            ModelItem activity = wrapper.Item1;
            if(activity == null) return null;
            return activity;
        }


        #region SetDisplayName

        // PBI 9220 - 2013.04.29 - TWR

        public static void SetDisplayName(ModelItem modelItem, IDev2FlowModel flow)
        {
            var displayName = modelItem.Properties[GlobalConstants.DisplayNamePropertyText];
            if(displayName != null)
            {
                displayName.SetValue(flow.DisplayText);
            }
        }

        #endregion
    }
}
