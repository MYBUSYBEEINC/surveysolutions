﻿using System;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.UI;
using WB.Core.SharedKernels.Enumerator.ViewModels.InterviewDetails.Questions.State;

namespace WB.UI.Shared.Enumerator.CustomBindings.Models
{
    public class MediaAttachment : IMediaAttachment
    {
        public string ContentPath { get; set; }
        
        public SimpleExoPlayer Player { get; set; }

        public void Dispose()
        {
            Player?.Release();
            Player?.Dispose();
            Player = null;
        }
    }
}
