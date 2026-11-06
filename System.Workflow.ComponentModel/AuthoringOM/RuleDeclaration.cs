namespace System.Workflow.ComponentModel
{
    using System;
    using System.CodeDom;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Workflow.ComponentModel.Design;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Serialization;
    using System.ComponentModel.Design.Serialization;
    using System.Reflection;

    //TODO: remove this inheritence, when event binding service line UseMethod and FreeMethod methods are fixed. 
    [DesignerSerializer(typeof(WorkflowMarkupSerializer), typeof(WorkflowMarkupSerializer))]
    // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
    [DesignerSerializer("System.Workflow.ComponentModel.Serialization.DependencyObjectCodeDomSerializer, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [TypeConverter(typeof(ConditionTypeConverter))]
    [ActivityValidator(typeof(ConditionValidator))]
    [MergableProperty(false)]
    [Browsable(true)]
    [Obsolete("The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*")]
    public abstract class ActivityCondition : DependencyObject
    {
        public abstract bool Evaluate(Activity activity, IServiceProvider provider);
    }
}
