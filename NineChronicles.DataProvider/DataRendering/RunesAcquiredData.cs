namespace NineChronicles.DataProvider.DataRendering
{
    using System;
    using System.Linq;
    using Libplanet.Assets;
    using Nekoyume.Action;
    using Nekoyume.Helper;
    using Nekoyume.Model.State;
    using NineChronicles.DataProvider.Store.Models;
    using static Lib9c.SerializeKeys;

    public static class RunesAcquiredData
    {
        public static RunesAcquiredModel GetRunesAcquiredInfo(
            ActionBase.ActionEvaluation<ActionBase> ev,
            IClaimStakeReward claimStakeReward,
            DateTimeOffset blockTime
        )
        {
            var plainValue = (Bencodex.Types.Dictionary)claimStakeReward.PlainValue;
            var avatarAddress = plainValue[AvatarAddressKey].ToAddress();
            var id = ((GameAction)claimStakeReward).Id;

#pragma warning disable CS0618
            var runeCurrency = Currency.Legacy(RuneHelper.StakeRune.Ticker, 0, minters: null);
#pragma warning restore CS0618
            var prevRuneBalance = ev.PreviousStates.GetBalance(
                avatarAddress,
                runeCurrency);
            var outputRuneBalance = ev.OutputStates.GetBalance(
                avatarAddress,
                runeCurrency);
            var acquiredRune = outputRuneBalance - prevRuneBalance;

            var runesAcquiredModel = new RunesAcquiredModel()
            {
                Id = id.ToString(),
                ActionType = claimStakeReward.ToString()!.Split('.').LastOrDefault()?.Replace(">", string.Empty),
                TickerType = RuneHelper.StakeRune.Ticker,
                BlockIndex = ev.BlockIndex,
                AgentAddress = ev.Signer.ToString(),
                AvatarAddress = avatarAddress.ToString(),
                AcquiredRune = Convert.ToDecimal(acquiredRune.GetQuantityString()),
                Date = DateOnly.FromDateTime(blockTime.DateTime),
                TimeStamp = blockTime,
            };

            return runesAcquiredModel;
        }
    }
}
