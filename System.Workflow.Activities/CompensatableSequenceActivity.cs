namespace System.Workflow.Activities
{
    #region Imports

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.CodeDom;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Design;
    using System.Collections.Generic;
    using System.Workflow.ComponentModel.Compiler;

    #endregion

    [SRDescription(SR.CompensatableSequenceActivityDescription)]
    // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
    [ToolboxItem("System.Workflow.ComponentModel.Design.ActivityToolboxItem, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    [Designer("System.Workflow.Activities.SequenceDesigner, System.Workflow.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(IDesigner))]
    [SRCategory(SR.Standard)]
    [Obsolete("The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*")]
    public sealed class CompensatableSequenceActivity : SequenceActivity, ICompensatableActivity
    {
        #region Constructors

        public CompensatableSequenceActivity()
        {
        }

        public CompensatableSequenceActivity(string name)
            : base(name)
        {
        }

        #endregion

        #region ICompensatableActivity Members
        ActivityExecutionStatus ICompensatableActivity.Compensate(ActivityExecutionContext executionContext)
        {
            return ActivityExecutionStatus.Closed;
        }
        #endregion
    }
}
