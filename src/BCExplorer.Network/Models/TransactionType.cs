using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Models
{
    public enum TransactionType
    {
        Unknown = 0,
        PoW_Reward_Coinbase = 1,
        PoS_Reward = 2,
        Money = 3
    }
}
