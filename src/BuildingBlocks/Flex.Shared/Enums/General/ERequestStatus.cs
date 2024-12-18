using Flex.Shared.Attributes;

namespace Flex.Shared.Enums.General
{
    public enum ERequestStatus 
    {
        [EnumValue("D")]
        DRAFT,

        [EnumValue("A")]
        APPROVED,

        [EnumValue("R")]
        REJECTED 
    }
}
