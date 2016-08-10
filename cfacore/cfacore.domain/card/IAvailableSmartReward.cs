using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using core.synchronization.Automation;

namespace cfacore.domain.card
{
    [ITable]
    public interface IAvailableSmartReward
    {
        [Column]
        DateTime createDate { get; set; }
        [Column]
        bool createDateSpecified { get; set; }
        [Column]
        DateTime eligibleDate { get; set; }
        [Column]
        bool eligibleDateSpecified { get; set; }
        [Column]
        int rewardProgramId { get; set; }
        [Column]
        int tierId { get; set; }
        [Column]
        string tierName { get; set; }
    }
}
