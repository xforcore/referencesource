//------------------------------------------------------------------------------
// <copyright file="FilterElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Configuration;
using System;

namespace System.Diagnostics {
    internal class FilterElement : TypedElement {
        public FilterElement() : base(typeof(TraceFilter)) {}

        public TraceFilter GetRuntimeObject() {
            TraceFilter newFilter = (TraceFilter) BaseGetRuntimeObject();
            // webForCore: TraceFilter.initializeData is internal to .NET's System.Diagnostics; webForCore keeps it
            // (xforcore/webForCore#213).
            TraceInitializeData.Set(newFilter, InitData);
            return newFilter;
        }

        internal TraceFilter RefreshRuntimeObject(TraceFilter filter) {
            // webForCore: as GetRuntimeObject finds the type, and the initializeData webForCore kept (xforcore/webForCore#213).
            if (TraceConfigurationTypes.GetType(TypeName) != filter.GetType() || InitData != TraceInitializeData.Get(filter)) {
                // type or initdata changed
                _runtimeObject = null;
                return GetRuntimeObject();
            }
            else {
                return filter;
            }
        }
    }
}


