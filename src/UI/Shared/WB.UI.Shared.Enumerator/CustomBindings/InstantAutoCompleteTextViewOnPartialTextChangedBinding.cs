﻿using System;
using System.Windows.Input;
using MvvmCross.Binding;
using MvvmCross.WeakSubscription;
using WB.UI.Shared.Enumerator.CustomControls;

namespace WB.UI.Shared.Enumerator.CustomBindings
{
    public class InstantAutoCompleteTextViewOnPartialTextChangedBinding : BaseBinding<InstantAutoCompleteTextView, ICommand>
    {
        private WeakReference<ICommand> command;
        private IDisposable subscription;

        public InstantAutoCompleteTextViewOnPartialTextChangedBinding(InstantAutoCompleteTextView androidControl) : base(androidControl)
        {
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        protected override void SetValueToView(InstantAutoCompleteTextView control, ICommand value)
        {
            if (this.Target == null)
                return;

            this.command = new WeakReference<ICommand>(value);
        }

        private void AutoCompleteOnPartialTextChanged(object sender, string partialText)
        {
            if (this.command != null && command.TryGetTarget(out var com))
                com.Execute(partialText);
        }

        public override void SubscribeToEvents()
        {
            var autoComplete = this.Target;
            if (autoComplete == null)
                return;

            this.subscription = autoComplete.WeakSubscribe<InstantAutoCompleteTextView, string>(
                nameof(autoComplete.PartialTextChanged),
                AutoCompleteOnPartialTextChanged);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
                return;

            if (isDisposing)
            {
                this.subscription?.Dispose();
                this.subscription = null;
                this.command = null;
            }
            base.Dispose(isDisposing);
        }
    }
}
