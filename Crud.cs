using Microsoft.EntityFrameworkCore;

namespace practic_2;

public class Crud
{
    public static async Task<Note> Create(int id, string text, DateTimeOffset createdat, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var note = new Note
        {
            Id = id,
            Text = text,
            CreatedAt = createdat, 
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync(ct);
        return note;
    }

    public static async Task<List<Note>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Notes
            .Where(x => EF.Functions.Like(x.Text, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }

    public static async Task<Note?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Notes.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public static async Task Update(Note note, int id, string text, DateTimeOffset createdat, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        note.Id = id;
        note.Text = text;
        note.CreatedAt = createdat;
        db.Notes.Update(note);
        await db.SaveChangesAsync(ct);
    }

    public static async Task Delete(Note note, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Notes.Remove(note);
        await db.SaveChangesAsync(ct);
    }
}