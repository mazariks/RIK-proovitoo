using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Event>? Events { get; set; }
    

    public async Task OnGetAsync()
    {
        Events = await _context.Events
            .Include(e => e.EventParticipants)!
            .ThenInclude(ep => ep.Participant)
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt).ToListAsync();
        foreach (var each in Events)
        {
            each.StartsAt = each.StartsAt.ToLocalTime();
        }
    }
}