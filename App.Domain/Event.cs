using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Domain;

public class Event : DomainEntityMetaId
{
    [Required(ErrorMessage = "Selle välja minimaalne pikkus on 1 märk.")]
    public string Naming { get; set; } = default!;
    
    [Required(ErrorMessage="See väli on kohustuslik.")]
    public DateTime StartsAt { get; set; } = default!;
    
    [Required(ErrorMessage="Selle välja minimaalne pikkus on 1 märk.")]
    public string Place { get; set; } = default!;
    
    [MaxLength(1000, ErrorMessage="Selle välja maksimaalne pikkus on 1000 märki.")]
    public string? AdditionalInfo { get; set; }

    public bool IsDeleted { get; set; } = false;
    
    public ICollection<EventParticipant>? EventParticipants { get; set; }
}