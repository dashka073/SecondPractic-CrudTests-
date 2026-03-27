using Microsoft.EntityFrameworkCore;
using practic_2;

namespace practic2.Tests;
public class CrudTests
{
    public CrudTests()
    {
        using var db = new DataContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateNote()
    {
        //Arrange
        int id = 100;
        string text = "text";
        DateTimeOffset date = DateTimeOffset.Now;

        //Act
        var note = await Crud.Create(id,text,date);

        //Assert
        using var db = new DataContext();
        var noteDb = await db.Notes.FindAsync(id);
        Assert.NotNull(note);               
        Assert.Equal(id, noteDb.Id);       
        Assert.Equal(text, noteDb.Text);   
        Assert.Equal(date, noteDb.CreatedAt);
    }

    [Theory]
    [InlineData("le", new int[] {1,2})]
    [InlineData("ti", new int[] {3})]
    [InlineData("", new int[] {1,2,3})]
    [InlineData("abc", new int[] { })]
    public async Task ReadNote(string text, int[] Ids)
    {
        //Arrange
        using var context = new DataContext();
        var notes = new[]
        {
            new Note{Id = 1, Text = "elephant", CreatedAt = DateTimeOffset.Now},
            new Note{Id = 2, Text = "leo", CreatedAt = DateTimeOffset.Now},
            new Note{Id = 3, Text = "tiger", CreatedAt = DateTimeOffset.Now}
        };
        context.Notes.AddRange(notes);
        await context.SaveChangesAsync();

        //Act
        List<Note> result = await Crud.Read(text);

        //Assert
        Assert.Equal(Ids.Length, result.Count);
        foreach (int id in Ids)
        {
            Assert.Contains(result, n => n.Id == id);
        }
    }

    [Fact]
    public async Task UpdateNote()
    {
        //Arrange
        int Id = 10;
        string oldText = "Random text";
        DateTimeOffset oldDate = DateTimeOffset.Now;
        
        using var db = new DataContext();
        var note = new Note
        { 
            Id = Id, 
            Text = oldText, 
            CreatedAt = oldDate
        };

        db.Notes.Add(note);
        await db.SaveChangesAsync();

        //Act
        string newText = "Noviy text";
        DateTimeOffset newDate = DateTimeOffset.UtcNow.AddHours(1);

        await Crud.Update(note, Id, newText, newDate);

        //Assert
        var updatedNote = await db.Notes.FindAsync(Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newText, updatedNote.Text);
        Assert.Equal(newDate, updatedNote.CreatedAt);
    }

    [Fact]
    public async Task DeleteNote()
    {
        //Arrange
        int id = 2;
        string text = "text";
        DateTimeOffset date = DateTimeOffset.Now;

        Note createdNote = await Crud.Create(id, text, date);

        //Act
        await Crud.Delete(createdNote);

        //Assert
        using var db = new DataContext();
        Note? deletedNote = await db.Notes.FindAsync(id);
        Assert.Null(deletedNote);
    }
}