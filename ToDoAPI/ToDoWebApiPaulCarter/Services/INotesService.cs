using ToDoWebApiPaulCarter.Models;

namespace ToDoWebApiPaulCarter.Services;

public interface INotesService
{
    IEnumerable<Note> GetNotes(bool? completed);
    int GetRemainingCount();
    void AddNote(Note note);
    void UpdateNote(int id, Note updatedNote);
    void DeleteNote(int id);
    void ToggleAllNotes();
    void ClearCompletedNotes();
}
