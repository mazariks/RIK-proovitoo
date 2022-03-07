using System.ComponentModel.DataAnnotations;
using Base.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Participant: DomainEntityMetaId
{
    public PaymentType PaymentType { get; set; } = default!;

    public char Type { get; set; } = default!;

    //Fields for 'eraisik'
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? FullName { get; set; }
    public string? IdentityCode { get; set; }
    
    //Fields for 'jurisik'
    public string? Naming { get; set; }
    public string? RegisterCode { get; set; }
    
    [Range(1, Int32.MaxValue, ErrorMessage="Minimaalne väärtus on 1.")]
    public int? AmountOfGuests { get; set; }
    
    [StringLength(5000, ErrorMessage="Selle välja maksimaalne pikkus on 5000 märki.")]
    public string? AdditionalInfo { get; set; }

    public bool IsDeleted { get; set; } = false;
    
    public ICollection<EventParticipant>? EventParticipants { get; set; }
}