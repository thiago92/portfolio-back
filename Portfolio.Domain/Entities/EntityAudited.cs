using Portfolio.Domain.Interface.Markers;

namespace Portfolio.Domain.Entities
{
    public abstract class EntityAudited : Entity, IMustAudited
    {
        public Guid CreationUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? ModificationUserId { get; set; }
        public DateTime? ModificationTime { get; set; }
    }
}
