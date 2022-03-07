using Base.Domain;

namespace Domain;

public class EventParticipant: DomainEntityMetaId
{
    //FK for Event
    public Guid EventId { get; set; }
    public Event? Event { get; set; }
    
    //FK for Participant
    public Guid ParticipantId { get; set; }
    public Participant? Participant { get; set; }

    public bool IsDeleted { get; set; } = false;
}