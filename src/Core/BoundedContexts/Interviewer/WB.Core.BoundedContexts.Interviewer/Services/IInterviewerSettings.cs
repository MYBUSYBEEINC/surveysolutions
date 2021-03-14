using System.Collections.Generic;
using WB.Core.SharedKernels.DataCollection.Implementation.Entities;
using WB.Core.SharedKernels.Enumerator.Services;

namespace WB.Core.BoundedContexts.Interviewer.Services
{
    public interface IInterviewerSettings : IEnumeratorSettings, IDeviceSettings
    {
        void SetVibrateOnError(bool vibrate);
        void SetShowLocationOnMap(bool showLocationOnMap);
        void SetAllowSyncWithHq(bool allowSyncWithHq);
        bool AllowSyncWithHq { get; }
        bool IsOfflineSynchronizationDone { get; }
        void SetOfflineSynchronizationCompleted();

        bool PartialSynchronizationEnabled { get; }
        void SetPartialSynchronizationEnabled(bool enable);
        void SetQuestionnaireInWebMode(List<string> tabletSettingsQuestionnairesInWebMode);
        void SetWebInterviewUrlTemplate(string tabletSettingsWebInterviewUrlTemplate);
        List<QuestionnaireIdentity> QuestionnairesInWebMode { get; }
        string? WebInterviewUriTemplate { get; }
    }
}
