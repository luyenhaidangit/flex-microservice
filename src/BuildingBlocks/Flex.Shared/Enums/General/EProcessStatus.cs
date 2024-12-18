using Flex.Shared.Attributes;

namespace Flex.Shared.Enums.General
{
    public enum EProcessStatus 
    {
        [EnumValue("PC")]
        PendingCreate,

        [EnumValue("PU")]
        PendingUpdate,

        [EnumValue("PD")]
        PendingDelete,

        [EnumValue("C")]
        Completed
    }
}
