using System.ComponentModel;

namespace Flex.Shared.Enums.Securities
{
    public enum ETradePlace
    {
        [Description("HOSE")]
        Hose = 1,

        [Description("HNX")]
        Hnx = 2,

        [Description("OTC")]
        Otc = 3,

        [Description("UPCOM")]
        Upcom = 5,

        [Description("WFT")]
        Wft = 6,

        [Description("CCQ mở")]
        OpenFund = 10,

        [Description("DCCNY")]
        Dccny = 9,

        [Description("Trái phiếu chuyên biệt")]
        SpecializedBond = 7,

        [Description("Tín phiếu")]
        TreasuryBill = 8
    }
}
