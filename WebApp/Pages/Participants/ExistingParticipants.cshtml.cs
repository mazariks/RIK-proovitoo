using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Participants;

public class ExistingParticipants : PageModel
{
    private readonly ApplicationDbContext _context;

    public ExistingParticipants(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Participant>? Participants { get; set; }

    [BindProperty] public Guid EventId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        EventId = id;
        Participants = await _context.Participants.Where(p => !p.IsDeleted).ToListAsync();
        return Page();
    }

    [BindProperty] public Guid ParticipantId { get; set; }

    public string? Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Event? currentEvent = await _context.Events.Where(e => e.Id == EventId).FirstOrDefaultAsync();
        Participant? currentParticipant =
            await _context.Participants.Where(p => p.Id == ParticipantId).FirstOrDefaultAsync();
        if (currentParticipant == null)
        {
            //TODO! Peaks olema veateade
            return RedirectToPage("../Index");
        }
        
        if (currentEvent == null)
        {
            //TODO! Peaks olema veateade
            Console.WriteLine("Event missing");
            return RedirectToPage("../Index");
        }

        if (await _context.EventParticipants
                .Where(ep =>
                    ep.ParticipantId == ParticipantId && ep.EventId == EventId && !ep.IsDeleted)
                .AnyAsync())
        {
            Message = "Valitud osaleja on juba üritusele registreeritud.";
            Participants = await _context.Participants.Where(p => !p.IsDeleted).ToListAsync();
            return Page();
        }

        EventParticipant eventParticipant = new EventParticipant()
        {
            Event = currentEvent,
            EventId = EventId,
            Participant = currentParticipant,
            ParticipantId = ParticipantId
        };
        await _context.EventParticipants.AddAsync(eventParticipant);
        await _context.SaveChangesAsync();


        return RedirectToPage("../Index");
    }
}