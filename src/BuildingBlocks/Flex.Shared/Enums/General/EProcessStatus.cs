using Flex.Shared.Attributes;

namespace Flex.Shared.Enums.General
{
    public enum EProcessStatus 
    {
        [EnumValue("PA")]
        PendingCreate,

        [EnumValue("PU")]
        PendingUpdate,

        [EnumValue("PD")]
        PendingDelete,

        [EnumValue("C")]
        Complete,
    }
}
