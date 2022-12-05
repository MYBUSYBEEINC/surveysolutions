﻿using System;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Com.Google.Android.Exoplayer2.Video;
using Java.IO;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding;
using MvvmCross.WeakSubscription;
using WB.Core.SharedKernels.Enumerator.ViewModels.InterviewDetails.Questions.State;
using WB.UI.Shared.Enumerator.CustomBindings.Models;
using Uri = Android.Net.Uri;

namespace WB.UI.Shared.Enumerator.CustomBindings
{
    public class ExoPlayerBinding : BaseBinding<PlayerView, IMediaAttachment>
    {
        public ExoPlayerBinding(PlayerView view) : base(view)
        {
            
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        private IDisposable metadataEventSubscription;

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.metadataEventSubscription?.Dispose();
                this.metadataEventSubscription = null;
                
                if (Target?.Player != null)
                {
                    Target.Player.Release();
                    Target.Player.Dispose();
                    Target.Player = null;
                }
            }
            
            base.Dispose(isDisposing);
        }

        protected override void SetValueToView(PlayerView view, IMediaAttachment media)
        {
            // exit if there is no content path of file not exists
            if (media == null 
                || string.IsNullOrWhiteSpace(media.ContentPath) 
                || !System.IO.File.Exists(media.ContentPath))
            {
                return;
            }
            
            // we don't want to rebind same player on same view
            var mediaId = view.Player?.CurrentMediaItem?.MediaId;
            if (mediaId != null && media.ContentPath == mediaId) return;

            var dataSourceFactory = new DefaultDataSourceFactory(
                view.Context, Util.GetUserAgent(view.Context, "ExoPlayerInfo")
            );

            var uri = Uri.FromFile(new File(media.ContentPath));
            var mediaItem = MediaItem.FromUri(uri);
            mediaItem.MediaId = media.ContentPath;
            var mediaSourceFactory = new ProgressiveMediaSource.Factory(dataSourceFactory, new DefaultExtractorsFactory());
            var mediaSource = mediaSourceFactory.CreateMediaSource(mediaItem);

            SimpleExoPlayer simpleExoPlayer = view.Player as SimpleExoPlayer;
            
            if (simpleExoPlayer == null)
            {
                metadataEventSubscription?.Dispose();
                metadataEventSubscription = null;
                
                SimpleExoPlayer.Builder exoPlayerBuilder = new SimpleExoPlayer.Builder(view.Context);
                simpleExoPlayer = exoPlayerBuilder.Build();
                
                view.Player = simpleExoPlayer;
                
                if (media is MediaAttachment mediaAttachment)
                    mediaAttachment.Player = simpleExoPlayer;
            }
            else
            {
                //simpleExoPlayer.Stop();
                //simpleExoPlayer.Release();
                
                var currentMediaItem = simpleExoPlayer.CurrentMediaItem;
                simpleExoPlayer.RemoveMediaItem(0);
                currentMediaItem?.Dispose();
            }

            simpleExoPlayer.SetMediaSource(mediaSource);
            simpleExoPlayer.Prepare();

            // adjust video view height so that video take all horizontal space
            metadataEventSubscription = simpleExoPlayer.WeakSubscribe<SimpleExoPlayer, VideoFrameMetadataEventArgs>(
                nameof(simpleExoPlayer.VideoFrameMetadata),
                this.HandleVideoFrameMetadata);

            simpleExoPlayer.SeekTo(1);
        }

        private async void HandleVideoFrameMetadata(object sender, VideoFrameMetadataEventArgs args)
        {
            var mainThreadDispatcher = Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>();

            await mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                var view = Target;
                var ratio = (float)args.P2.Height / (float)args.P2.Width / (float)args.P2.PixelWidthHeightRatio;
                view.SetMinimumHeight((int)(view.Width * ratio));
                view.HideController();
                
                this.metadataEventSubscription?.Dispose();
                this.metadataEventSubscription = null;
            });
        }
    }

    public class ExoPlayerAudioAttachmentBinding : ExoPlayerBinding
    {
        public ExoPlayerAudioAttachmentBinding(PlayerView view) : base(view)
        {

        }

        protected override void SetValueToView(PlayerView view, IMediaAttachment value)
        {
            base.SetValueToView(view, value);
            view.ControllerShowTimeoutMs = 0;
            view.ControllerHideOnTouch = false;
            view.ShowController();
        }
    }
}
