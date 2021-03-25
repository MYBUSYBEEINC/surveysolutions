﻿namespace WB.Core.SharedKernels.Enumerator.Services
{
    public class ChangePasswordDialogOkCallback
    {
        public ChangePasswordDialogResult DialogResult { get; set; }
        public bool NeedClose { get; set; } = true;
        public string OldPasswordError { get; set; }
        public string NewPasswordError { get; set; }
    }
}