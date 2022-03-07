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

namespace WebApp.Pages.Events
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DeleteModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; }
        
        [BindProperty]
        public bool FullDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Events.Include(e => e.EventParticipants).FirstOrDefaultAsync(m => m.Id == id);

            if (Event == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Events.Include(e => e.EventParticipants)
                .ThenInclude(ep => ep.Participant)
                .FirstOrDefaultAsync( e => e.Id == id);

            if (Event != null && Event.StartsAt > DateTime.Now)
            {
                foreach (var eventParticipant in Event.EventParticipants!)
                {
                    if (FullDelete)
                    {
                        eventParticipant.Participant!.IsDeleted = true;
                        eventParticipant.Participant.UpdatedAt = DateTime.UtcNow;
                    }
                    eventParticipant.IsDeleted = true;
                    eventParticipant.UpdatedAt = DateTime.UtcNow;
                    _context.EventParticipants.Update(eventParticipant);
                }
                Event.CreatedAt = DateTime.SpecifyKind(Event.CreatedAt, DateTimeKind.Utc);
                Event.UpdatedAt = DateTime.UtcNow;
                Event.IsDeleted = true;
                _context.Events.Update(Event);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index");
        }
    }
}
