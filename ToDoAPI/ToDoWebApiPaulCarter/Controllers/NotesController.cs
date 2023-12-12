using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoWebApiPaulCarter.Data;
using ToDoWebApiPaulCarter.Models;
using System.Text.Json;
using System.Diagnostics.Eventing.Reader;

namespace ToDoWebApiPaulCarter.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class NotesController : ControllerBase
    {
        private readonly ToDoContext _context;

        public NotesController(ToDoContext context)
        {
            _context = context;
        }

        [HttpGet("notes")]
        public IEnumerable<Note> Get(bool? completed) //Här kommer antingen null, true eller false
        {
            var dbNotes = _context.Notes;
            if (completed == true)
            {
                //retunera endast de som är true
                var activeNotes = dbNotes.Where(n => n.IsDone == true);
                return activeNotes;

            }
            else if (completed == false) 
            {
                //retunera endast de som är false
                var completeNotes = dbNotes.Where(n => n.IsDone == false);
                return completeNotes;
            }

            //retunera allt
            return dbNotes ;
        }

        [HttpGet("remaining")]
        public int GetRemaining()
        {
            var dbNotes = _context.Notes.Count(n => !n.IsDone);
            return dbNotes;
        }

        [HttpPost("notes")]
        public void Post(Note httpNote)
        {
            var note = httpNote;
            _context.Notes.Add(note);
            _context.SaveChanges();
        }

        [HttpPut("notes/{id}")]
        public void Put(int id, Note updatedNote)
        {
            var note = _context.Notes.FirstOrDefault(n => n.ID == id);
            if (note != null)
            {
                note.IsDone = updatedNote.IsDone;
                _context.SaveChanges();

            }
        }

        [HttpDelete("notes/{id}")]
        public void Delete(int id)
        {
            var note = _context.Notes.FirstOrDefault(n => n.ID == id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                _context.SaveChanges();

            }
        }

        [HttpPost("toggle-all")]
        public void ToggleAll()
        {
            bool allDone = _context.Notes.All(n => n.IsDone);
            if (allDone == false)
            {
                foreach (var note in _context.Notes)
                {
                    note.IsDone = true;
                }
            }
            else
            {
                foreach (var note in _context.Notes)
                {
                    note.IsDone = false;
                }
            }
                    _context.SaveChanges();
        }

        [HttpPost("clear-completed")]
        public void ClearCompleted()
        {

            var note = _context.Notes.Where(n => n.IsDone == true);
            foreach (var n in note)
            {
                _context.Notes.Remove(n);
                _context.SaveChanges();

            }
        }
    }
}

