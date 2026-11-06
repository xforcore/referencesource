namespace System.Workflow.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design.Serialization;
    using System.Workflow.ComponentModel.Design;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Serialization;

    [Browsable(true)]
    // workflowForCore: designer types by name, as .NET Framework records them; designers and toolbox bitmaps aren't compiled (docs/LIFT.md).
    [DesignerSerializer("System.Workflow.ComponentModel.Serialization.DependencyObjectCodeDomSerializer, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [Obsolete("The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*")]
    public sealed class WorkflowParameterBinding : DependencyObject
    {
        public static readonly DependencyProperty ParameterNameProperty = DependencyProperty.Register("ParameterName", typeof(string), typeof(WorkflowParameterBinding), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(WorkflowParameterBinding));

        public WorkflowParameterBinding()
        {
        }

        public WorkflowParameterBinding(string parameterName)
        {
            SetValue(ParameterNameProperty, parameterName);
        }

        [DefaultValue(null)]
        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public string ParameterName
        {
            get
            {
                return (string)GetValue(ParameterNameProperty);
            }
            set
            {
                SetValue(ParameterNameProperty, value);
            }
        }
    }

    [Serializable]
    [Obsolete("The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*")]
    public sealed class WorkflowParameterBindingCollection : KeyedCollection<string, WorkflowParameterBinding>
    {
        private Activity ownerActivity = null;

        public WorkflowParameterBindingCollection(Activity ownerActivity)
        {
            if (ownerActivity == null)
                throw new ArgumentNullException("ownerActivity");

            this.ownerActivity = ownerActivity;
        }

        public WorkflowParameterBinding GetItem(string key)
        {
            return this[key];
        }

        protected override string GetKeyForItem(WorkflowParameterBinding item)
        {
            return item.ParameterName;
        }
        protected override void ClearItems()
        {
            if (!this.ownerActivity.DesignMode)
                throw new InvalidOperationException(SR.GetString(SR.Error_CanNotChangeAtRuntime));

            base.ClearItems();
        }
        protected override void InsertItem(int index, WorkflowParameterBinding item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!this.ownerActivity.DesignMode)
                throw new InvalidOperationException(SR.GetString(SR.Error_CanNotChangeAtRuntime));

            if (Contains(item.ParameterName))
            {
                WorkflowParameterBinding oldItem = this[item.ParameterName];
                index = this.IndexOf(oldItem);
                RemoveItem(index);
            }

            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            if (!this.ownerActivity.DesignMode)
                throw new InvalidOperationException(SR.GetString(SR.Error_CanNotChangeAtRuntime));

            base.RemoveItem(index);
        }
        protected override void SetItem(int index, WorkflowParameterBinding item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!this.ownerActivity.DesignMode)
                throw new InvalidOperationException(SR.GetString(SR.Error_CanNotChangeAtRuntime));

            base.SetItem(index, item);
        }
    }
}
