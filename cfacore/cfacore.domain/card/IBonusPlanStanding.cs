using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.domain.card
{
    public interface IBonusPlanStanding
    {
        IAvailableSmartReward[] availableSmartReward { get; set; }
        string BPCredit { get; set; }
        DateTime BPExpireDate { get; set; }
        bool BPExpireDateSpecified { get; set; }
        int BPID { get; set; }
        bool BPIDSpecified { get; set; }
        DateTime BPMemberExpireDate { get; set; }
        bool BPMemberExpireDateSpecified { get; set; }
        string BPName { get; set; }
        string BPReward { get; set; }
        DateTime BPRewardResetDate { get; set; }
        bool BPRewardResetDateSpecified { get; set; }
        string BPRewardThreshold { get; set; }
        string BPStatus { get; set; }
        string BPType { get; set; }
        string nextBirthdayReward { get; set; }
        IQueuedReward[] queuedReward { get; set; }
    }
}
