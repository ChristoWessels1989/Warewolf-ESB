﻿
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Dev2.Diagnostics
{
    // If you add/remove columns here then 
    // change DebugState.Serialize/Deserialize
    public interface IDebugItem : IList<IDebugItemResult>
    {
        [Obsolete]
        string Group { get; set; }
        string MoreText { get; set; }
        string MoreLink { get; set; }

        bool Contains(string filterText);
        XElement ToXml();
    }
}
