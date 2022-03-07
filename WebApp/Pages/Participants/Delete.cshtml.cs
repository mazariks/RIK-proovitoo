#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages.Participants
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DeleteModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Participant Participant { get; set; }

        [BindProperty] public Guid EventId { get; set; }

        [BindProperty] public bool FullDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? participantId, Guid eventId)
        {
            EventId = eventId;
            if (participantId == null)
            {
                return NotFound();
            }

            Participant = await _context.Participants.FirstOrDefaultAsync(m => m.Id == participantId);

            if (Participant == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? participantId)
        {
            if (participantId == null)
            {
                return NotFound();
            }

            Participant = await _context.Participants.FindAsync(participantId);
            Event currentEvent = await _context.Events.FindAsync(EventId);
            List<EventParticipant> eventParticipants = await _context.EventParticipants.Where(ep =>
                ep.ParticipantId == participantId).ToListAsync();
            if (Participant != null && currentEvent != null)
            {
                EventParticipant eventParticipant = eventParticipants.First(ep => ep.EventId == EventId && !ep.IsDeleted);
                Participant.CreatedAt = DateTime.SpecifyKind(Participant.CreatedAt, DateTimeKind.Utc);
                eventParticipant.CreatedAt = DateTime.SpecifyKind(eventParticipant.CreatedAt, DateTimeKind.Utc);
                eventParticipant.IsDeleted = true;
                eventParticipant.UpdatedAt = DateTime.UtcNow;
                if (FullDelete)
                {
                    foreach (var eachParticipant in eventParticipants)
                    {
                        eachParticipant.CreatedAt = DateTime.SpecifyKind(eachParticipant.CreatedAt, DateTimeKind.Utc);
                        eachParticipant.IsDeleted = true;
                        eventParticipant.UpdatedAt = DateTime.UtcNow;
                    }

                    Participant.UpdatedAt = DateTime.UtcNow;
                    Participant.IsDeleted = true;
                }

                _context.EventParticipants.Update(eventParticipant);
                _context.Participants.Update(Participant);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index");
        }
    }
}