﻿using System.Collections.Generic;

namespace Unlimited.Framework.Converters.Graph.Interfaces
{
    public interface IPath
    {
        string ActualPath { get; set; }
        string DisplayPath { get; set; }
        string SampleData { get; set; }
        string OutputExpression { get; set; }

        IEnumerable<IPathSegment> GetSegements();
        IPathSegment CreatePathSegment(string pathSegmentString);
    }
}
