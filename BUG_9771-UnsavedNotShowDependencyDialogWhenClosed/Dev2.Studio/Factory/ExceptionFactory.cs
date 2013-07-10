﻿using System;
using System.Diagnostics;
using System.Text;
using Dev2.Composition;
using Dev2.Studio.Core.Helpers;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Model;
using Dev2.Studio.ViewModels.Diagnostics;

namespace Dev2.Studio.Factory
{
    /// <summary>
    /// Factory used to create a wrapper around exceptions
    /// </summary>
    /// <author>jurie.smit</author>
    /// <date>2013/01/15</date>
    public static class ExceptionFactory
    {
        /// <summary>
        /// Creates a UI wrapper the specified exception (recursively).
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        /// <author>jurie.smit</author>
        /// <date>2013/01/15</date>
        public static ExceptionUIModel Create(Exception exception)
        {
            var uiModel = new ExceptionUIModel { Message = exception.Message };

            if(exception.InnerException != null)
            {
                uiModel.Exception.Add(Create(exception.InnerException));
            }

            return uiModel;
        }

        /// <summary>
        /// Creates the string value (recursively).
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="builder">The builder to use - null if not recursive.</param>
        /// <returns></returns>
        /// <author>jurie.smit</author>
        /// <date>2013/01/15</date>
        public static StringBuilder CreateStringValue(Exception exception, StringBuilder builder = null)
        {
            var appendStackTrace = false;
            if(builder == null)
            {
                builder = new StringBuilder();
                appendStackTrace = true;
            }

            builder.AppendLine("Exception: " + exception.Message);

            if(exception.InnerException != null)
            {
                CreateStringValue(exception.InnerException, builder);
            }

            if(appendStackTrace)
            {
                builder.AppendLine("StackTrace:");
                builder.AppendLine(exception.StackTrace);

                // 14th Feb 2013
                // Added by Michael to assist with debugging
                string fullStackTrace = Environment.NewLine + Environment.NewLine + "Additional Trace Info" + Environment.NewLine + Environment.NewLine;
                StackTrace theStackTrace = new StackTrace();
                for(int j = theStackTrace.FrameCount - 1; j >= 0; j--)
                {
                    string module = theStackTrace.GetFrame(j).GetMethod().Module.ToString();
                    if(module != "WindowsBase.dll" && module != "CommonLanguageRuntimeLibrary")
                    {

                        fullStackTrace += "--> " + theStackTrace.GetFrame(j).GetMethod().Name + " (" + theStackTrace.GetFrame(j).GetMethod().Module + ")";
                    }
                }
                builder.Append(fullStackTrace);
            }


            return builder;
        }

        /// <summary>
        /// Creates the exception view model.
        /// </summary>
        /// <param name="e">The exception for this viewmodel.</param>
        /// <param name="environmentModel">The environment model.</param>
        /// <returns></returns>
        /// <date>2013/01/16</date>
        /// <author>
        /// Jurie.smit
        /// </author>
        public static IExceptionViewModel CreateViewModel(Exception e, IEnvironmentModel environmentModel)
        {
            // PBI 9598 - 2013.06.10 - TWR : added environmentModel parameter
            var vm = new ExceptionViewModel
                {
                    OutputText = CreateStringValue(e).ToString(),
                    StackTrace = e.StackTrace,
                    OutputPath = FileHelper.GetUniqueOutputPath(".txt"),
                    ServerLogTempPath = FileHelper.GetServerLogTempPath(environmentModel)
                };

            string attachmentPath = vm.OutputPath + ";" + vm.ServerLogTempPath;
            vm.FeedbackAction = FeedbackFactory.CreateEmailFeedbackAction(attachmentPath);
            ImportService.SatisfyImports(vm);
            vm.Exception.Clear();
            vm.Exception.Add(Create(e));
            return vm;
        }
    }
}
