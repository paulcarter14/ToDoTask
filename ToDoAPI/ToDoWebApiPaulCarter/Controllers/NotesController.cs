using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoWebApiPaulCarter.Data;
using ToDoWebApiPaulCarter.Models;
using System.Text.Json;
using System.Diagnostics.Eventing.Reader;
using ToDoWebApiPaulCarter.Services;

namespace ToDoWebApiPaulCarter.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }

        [HttpGet("notes")]
        public ActionResult<IEnumerable<Note>> Get(bool? completed)
        {
            try
            {
                var notes = _notesService.GetNotes(completed);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("remaining")]
        public ActionResult<int> GetRemaining()
        {
            try
            {
                var count = _notesService.GetRemainingCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("notes")]
        public ActionResult Post([FromBody] Note note)
        {
            try
            {
                _notesService.AddNote(note);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPut("notes/{id}")]
        public ActionResult Put(int id, [FromBody] Note updatedNote)
        {
            try
            {
                _notesService.UpdateNote(id, updatedNote);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpDelete("notes/{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _notesService.DeleteNote(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("toggle-all")]
        public ActionResult ToggleAll()
        {
            try
            {
                _notesService.ToggleAllNotes();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("clear-completed")]
        public ActionResult ClearCompleted()
        {
            try
            {
                _notesService.ClearCompletedNotes();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}

