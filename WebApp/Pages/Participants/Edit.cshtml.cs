#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages.Participants
{
    public class EditModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public EditModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Participant Participant { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Participant = await _context.Participants.FirstOrDefaultAsync(m => m.Id == id);

            if (Participant == null)
            {
                return NotFound();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            //if F and B fields are filled, then empty B fields and vice versa
            switch (Participant.Type)
            {
                case 'f':
                    if (Participant.FirstName == null || Participant.LastName == null ||
                        Participant.IdentityCode == null)
                    {
                        Message = "Eraisiku puhul on väljad 'EESNIMI', 'PERENIMI' ja 'ISIKUKOOD' kohustuslikud.";
                        return Page();
                    }

                    if (Participant.AdditionalInfo?.Length > 1500)
                    {
                        Message = "Lisainfo teksti pikkus on liiga suur. Max = 1000";
                        return Page();
                    }

                    Participant.Naming = null;
                    Participant.RegisterCode = null;
                    Participant.AmountOfGuests = null;
                    break;

                case 'j':
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

                    Participant.FirstName = null;
                    Participant.LastName = null;
                    Participant.IdentityCode = null;
                    break;
            }

            Participant.CreatedAt = DateTime.SpecifyKind(Participant.CreatedAt, DateTimeKind.Utc);
            Participant.UpdatedAt = DateTime.UtcNow;
            _context.Attach(Participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(Participant.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Index");
        }

        private bool ParticipantExists(Guid id)
        {
            return _context.Participants.Any(e => e.Id == id);
        }
    }
}