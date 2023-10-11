using HitchSapB1Lib.Enums;

namespace HitchSapB1Lib.Objects
{
    public class DocumentReference
    {
        public int Number { get; set; }
        public ObjectType Type { get; set; }
    }

    public class LineReference
    {
        public DocumentStatus Status { get; set; }
        public int LineNum { get; set; }
    }
}
