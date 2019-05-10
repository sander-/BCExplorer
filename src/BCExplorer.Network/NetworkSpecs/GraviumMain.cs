using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.NetworkSpecs
{
    public static class Gravium
    {
        static NBitcoin.Network _mainnet;

        public static NBitcoin.Network Main()
        {
            if (_mainnet != null)
                return _mainnet;

            NBitcoin.NetworkBuilder builder = new NBitcoin.NetworkBuilder();

            var graviumMainConsensus = new NBitcoin.Consensus
            {
                SubsidyHalvingInterval = 210000,
                MajorityEnforceBlockUpgrade = 750,
                MajorityRejectBlockOutdated = 950,
                MajorityWindow = 1000,
                BIP34Hash = null,
                PowLimit = new NBitcoin.Target(new NBitcoin.uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60),    // two weeks, 20160 minutes
                PowTargetSpacing = TimeSpan.FromSeconds(10 * 60),               // 10 minutes
                PowAllowMinDifficultyBlocks = false,
                PowNoRetargeting = false,
                RuleChangeActivationThreshold = 1916,   // 95% of 2016
                MinerConfirmationWindow = 2016,         // nPowTargetTimespan / nPowTargetSpacing
                CoinbaseMaturity = 100,                
                LitecoinWorkCalculation = false                
            };

            graviumMainConsensus.BIP34Hash = graviumMainConsensus.HashGenesisBlock;

            _mainnet = builder.SetConsensus(graviumMainConsensus)

                .SetBase58Bytes(NBitcoin.Base58Type.PUBKEY_ADDRESS, new byte[] { (75) })        // GRV / X
                .SetBase58Bytes(NBitcoin.Base58Type.SCRIPT_ADDRESS, new byte[] { (125) })
                .SetBase58Bytes(NBitcoin.Base58Type.SECRET_KEY, new byte[] { (75 + 128) })      // GRV
                .SetBase58Bytes(NBitcoin.Base58Type.ENCRYPTED_SECRET_KEY_NO_EC, new byte[] { 0x01, 0x42 })
                .SetBase58Bytes(NBitcoin.Base58Type.ENCRYPTED_SECRET_KEY_EC, new byte[] { 0x01, 0x43 })
                .SetBase58Bytes(NBitcoin.Base58Type.EXT_PUBLIC_KEY, new byte[] { (0x04), (0x88), (0xB2), (0x1E) })
                .SetBase58Bytes(NBitcoin.Base58Type.EXT_SECRET_KEY, new byte[] { (0x04), (0x88), (0xAD), (0xE4) })
                .SetBase58Bytes(NBitcoin.Base58Type.PASSPHRASE_CODE, new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 })
                .SetBase58Bytes(NBitcoin.Base58Type.CONFIRMATION_CODE, new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A })
                .SetBase58Bytes(NBitcoin.Base58Type.STEALTH_ADDRESS, new byte[] { 0x2a })
                .SetBase58Bytes(NBitcoin.Base58Type.ASSET_ID, new byte[] { 23 })
                .SetBase58Bytes(NBitcoin.Base58Type.COLORED_ADDRESS, new byte[] { 0x13 })
                .SetPort(0)
                .SetRPCPort(0)
                .SetName("grv-main")
                .AddAlias("grv-mainnet")                
                .BuildAndRegister();
            return _mainnet;
        }

    }
}
