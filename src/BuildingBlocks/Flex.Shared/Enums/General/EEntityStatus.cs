using Flex.Shared.Attributes;

namespace Flex.Shared.Enums.General
{
    public enum EEntityStatus
    {
        [EnumValue("P")]
        PENDING,

        [EnumValue("A")]
        ACTIVE,

        [EnumValue("I")]
        INACTIVE,

        [EnumValue("D")]
        DELETED
    }
}
