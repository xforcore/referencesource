namespace System.Workflow.ComponentModel
{
    #region Imports

    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Design;
    using System.Workflow.ComponentModel.Serialization;
    using System.Workflow.ComponentModel.Compiler;

    #endregion

    [SRDescription(SR.SynchronizationScopeActivityDescription)]
    // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
    [ToolboxItem("System.Workflow.ComponentModel.Design.ActivityToolboxItem, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    [SupportsSynchronization]
    [Designer("System.Workflow.ComponentModel.Design.SequenceDesigner, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(IDesigner))]
    [Obsolete("The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*")]
    public sealed class SynchronizationScopeActivity : CompositeActivity, IActivityEventListener<ActivityExecutionStatusChangedEventArgs>
    {

        public SynchronizationScopeActivity()
        {
        }

        public SynchronizationScopeActivity(string name)
            : base(name)
        {
        }

        [SRDisplayName(SR.SynchronizationHandles)]
        [SRDescription(SR.SynchronizationHandlesDesc)]
        [TypeConverter(typeof(SynchronizationHandlesTypeConverter))]
        // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
        [EditorAttribute("System.Workflow.ComponentModel.SynchronizationHandlesEditor, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public ICollection<String> SynchronizationHandles
        {
            get
            {
                return this.GetValue(SynchronizationHandlesProperty) as ICollection<String>;
            }
            set
            {
                this.SetValue(SynchronizationHandlesProperty, value);
            }
        }

        protected internal override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            return SequenceHelper.Execute(this, executionContext);
        }

        protected internal override ActivityExecutionStatus Cancel(ActivityExecutionContext executionContext)
        {
            return SequenceHelper.Cancel(this, executionContext);
        }

        void IActivityEventListener<ActivityExecutionStatusChangedEventArgs>.OnEvent(Object sender, ActivityExecutionStatusChangedEventArgs e)
        {
            SequenceHelper.OnEvent(this, sender, e);
        }

        protected internal override void OnActivityChangeRemove(ActivityExecutionContext executionContext, Activity removedActivity)
        {
            SequenceHelper.OnActivityChangeRemove(this, executionContext, removedActivity);
        }

        protected internal override void OnWorkflowChangesCompleted(ActivityExecutionContext executionContext)
        {
            SequenceHelper.OnWorkflowChangesCompleted(this, executionContext);
        }
    }

    #region Class SynchronizationHandlesTypeConverter
    internal sealed class SynchronizationHandlesTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is ICollection<String>)
                return Stringify(value as ICollection<String>);

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return UnStringify(value as string);

            return base.ConvertFrom(context, culture, value);
        }

        internal static string Stringify(ICollection<String> synchronizationHandles)
        {
            string stringifiedValue = string.Empty;
            if (synchronizationHandles == null)
                return stringifiedValue;

            foreach (string handle in synchronizationHandles)
            {
                if (handle == null)
                    continue;
                if (stringifiedValue != string.Empty)
                    stringifiedValue += ", ";
                stringifiedValue += handle.Replace(",", "\\,");
            }

            return stringifiedValue;
        }

        internal static ICollection<String> UnStringify(string stringifiedValue)
        {
            ICollection<String> synchronizationHandles = new List<String>();
            stringifiedValue = stringifiedValue.Replace("\\,", ">");
            foreach (string handle in stringifiedValue.Split(new char[] { ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string realHandle = handle.Trim().Replace('>', ',');
                if (realHandle != string.Empty && !synchronizationHandles.Contains(realHandle))
                    synchronizationHandles.Add(realHandle);
            }

            return synchronizationHandles;
        }
    }
    #endregion

#if !WORKFLOWFORCORE // workflowForCore: a property editor for Visual Studio's designer (docs/LIFT.md).
    internal sealed class SynchronizationHandlesEditor : UITypeEditor
    {
        private MultilineStringEditor stringEditor = new MultilineStringEditor();

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string stringValue = SynchronizationHandlesTypeConverter.Stringify(value as ICollection<string>);
            stringValue = stringEditor.EditValue(context, provider, stringValue) as string;
            value = SynchronizationHandlesTypeConverter.UnStringify(stringValue);

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return stringEditor.GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return stringEditor.GetPaintValueSupported(context);
        }
    }
#endif
}
