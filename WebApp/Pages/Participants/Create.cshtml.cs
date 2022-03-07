#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Participants
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Guid EventId { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet(Guid id)
        {
            EventId = id;
            return Page();
        }

        [BindProperty] public Participant Participant { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Participant.Type == 'f')
            {
                if (Participant.FirstName == null || Participant.LastName == null ||
                    Participant.IdentityCode == null)
                {
                    Message = "Eraisiku puhul on väljad 'EESNIMI', 'PERENIMI' ja 'ISIKUKOOD' kohustuslikud.";
                    return Page();
                }

                if (Participant.AdditionalInfo?.Length > 1500)
                {
                    Message = "Lisainfo teksti pikkus on liiga suur. Max = 1500.";
                    return Page();
                }

                if (await _context.Participants.Where(p => p.IdentityCode == Participant.IdentityCode && !p.IsDeleted).AnyAsync())
                {
                    Message = "Sama isikukoodiga isik on juba olemas.";
                    return Page();
                }
            }

            if (Participant.Type == 'j')
            {
                if (Participant.Naming == null || Participant.RegisterCode == null ||
                    Participant.AmountOfGuests == null)
                {
                    Message =
                        "Juriidilise isiku puhul on väljad 'NIMETUS', 'REGISTRIKOOD' ja 'OSALEJATE ARV' kohustuslikud.";
                    return Page();
                }

                if (Participant.AdditionalInfo?.Length > 5000)
                {
                    Message = "Lisainfo teksti pikkus on liiga suur. Max = 5000";
                    return Page();
                }
                
                if (await _context.Participants.Where(p => p.RegisterCode == Participant.RegisterCode && !p.IsDeleted).AnyAsync())
                {
                    Message = "Sama registrikoodiga isik on juba olemas.";
                    return Page();
                }
            }
            Event currentEvent = await _context.Events.Where(e => e.Id == EventId).FirstAsync();

            EventParticipant eventParticipant = new EventParticipant()
            {
                Event = currentEvent,
                EventId = EventId,
                Participant = Participant,
                ParticipantId = Participant.Id
            };
            await _context.Participants.AddAsync(Participant);
            await _context.EventParticipants.AddAsync(eventParticipant);

            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}