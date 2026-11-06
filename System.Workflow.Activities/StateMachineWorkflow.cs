namespace System.Workflow.Activities
{
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Reflection;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Workflow.ComponentModel.Compiler;

    [SRDescription(SR.StateMachineWorkflowActivityDescription)]
    // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
    [Designer("System.Workflow.Activities.StateMachineWorkflowDesigner, System.Workflow.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(IRootDesigner))]
    [Designer("System.Workflow.Activities.StateMachineWorkflowDesigner, System.Workflow.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(IDesigner))]
    [ToolboxItem(false)]
    [ActivityValidator(typeof(StateActivityValidator))]
    [SRCategory(SR.Standard)]
    [SRDisplayName(SR.StateMachineWorkflow)]
    [System.Runtime.InteropServices.ComVisible(false)]
    [Obsolete("The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*")]
    public class StateMachineWorkflowActivity : StateActivity
    {
        internal const string InitialStateNamePropertyName = "InitialStateName";
        internal const string CompletedStateNamePropertyName = "CompletedStateName";
        public const string SetStateQueueName = "SetStateQueue";

        //metadata properties
        public static readonly DependencyProperty InitialStateNameProperty = DependencyProperty.Register(StateMachineWorkflowActivity.InitialStateNamePropertyName, typeof(string), typeof(StateMachineWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static readonly DependencyProperty CompletedStateNameProperty = DependencyProperty.Register(StateMachineWorkflowActivity.CompletedStateNamePropertyName, typeof(string), typeof(StateMachineWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public StateMachineWorkflowActivity()
        {
        }


        public StateMachineWorkflowActivity(string name)
            : base(name)
        {
        }

        [SRDescription(SR.DynamicUpdateConditionDescr)]
        [SRCategory(SR.Conditions)]
        public ActivityCondition DynamicUpdateCondition
        {
            get
            {
                return WorkflowChanges.GetCondition(this) as ActivityCondition;
            }
            set
            {
                WorkflowChanges.SetCondition(this, value);
            }
        }


        [ValidationOption(ValidationOption.Optional)]
        [SRDescription(SR.InitialStateDescription)]
        // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
        [Editor("System.Workflow.Activities.StateDropDownEditor, System.Workflow.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [DefaultValue("")]
        public string InitialStateName
        {
            get
            {
                return (string)base.GetValue(InitialStateNameProperty);
            }
            set
            {
                base.SetValue(InitialStateNameProperty, value);
            }
        }

        [SRDescription(SR.CompletedStateDescription)]
        // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
        [Editor("System.Workflow.Activities.StateDropDownEditor, System.Workflow.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [DefaultValue("")]
        public string CompletedStateName
        {
            get
            {
                return (string)base.GetValue(CompletedStateNameProperty);
            }
            set
            {
                base.SetValue(CompletedStateNameProperty, value);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentStateName
        {
            get
            {
                StateMachineExecutionState executionState = this.ExecutionState;
                if (executionState == null)
                    return null;

                return executionState.CurrentStateName;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PreviousStateName
        {
            get
            {
                StateMachineExecutionState executionState = this.ExecutionState;
                if (executionState == null)
                    return null;

                return executionState.PreviousStateName;
            }
        }

        internal StateMachineExecutionState ExecutionState
        {
            get
            {
                return (StateMachineExecutionState)base.GetValue(StateMachineExecutionStateProperty);
            }
        }
    }
}
