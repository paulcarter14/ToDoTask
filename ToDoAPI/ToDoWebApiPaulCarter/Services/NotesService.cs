using ToDoWebApiPaulCarter.Data;
using ToDoWebApiPaulCarter.Models;

namespace ToDoWebApiPaulCarter.Services;

public class NotesService : INotesService
{
    private readonly ToDoContext _context;

    public NotesService(ToDoContext context)
    {
        _context = context;
    }

    public IEnumerable<Note> GetNotes(bool? completed)
    {
        if (completed.HasValue)
        {
            return _context.Notes.Where(n => n.IsDone == completed.Value).ToList();
        }
        else
        {
            return _context.Notes.ToList();
        }
    }

    public int GetRemainingCount()
    {
        return _context.Notes.Count(n => !n.IsDone);
    }

    public void AddNote(Note note)
    {
        _context.Notes.Add(note);
        _context.SaveChanges();
    }

    public void UpdateNote(int id, Note updatedNote)
    {
        var note = _context.Notes.FirstOrDefault(n => n.ID == id);
        if (note != null)
        {
            note.IsDone = updatedNote.IsDone;
            note.Title = updatedNote.Title;
            note.Description = updatedNote.Description;
            _context.SaveChanges();
        }
    }

    public void DeleteNote(int id)
    {
        var note = _context.Notes.FirstOrDefault(n => n.ID == id);
        if (note != null)
        {
            _context.Notes.Remove(note);
            _context.SaveChanges();
        }
    }

    public void ToggleAllNotes()
    {
        bool allDone = _context.Notes.All(n => n.IsDone);
        foreach (var note in _context.Notes)
        {
            note.IsDone = !allDone;
        }
        _context.SaveChanges();
    }

    public void ClearCompletedNotes()
    {
        var completedNotes = _context.Notes.Where(n => n.IsDone).ToList();
        foreach (var note in completedNotes)
        {
            _context.Notes.Remove(note);
        }
        _context.SaveChanges();
    }
}
