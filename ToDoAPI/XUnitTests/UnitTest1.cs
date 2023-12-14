using Microsoft.EntityFrameworkCore;
using Moq;
using ToDoWebApiPaulCarter.Models;
using ToDoWebApiPaulCarter.Data;
using ToDoWebApiPaulCarter.Services;

namespace XUnitTests;

public class UnitTest1
{
    private Mock<DbSet<Note>> CreateMockSet(List<Note> sourceList)
    {
        var queryable = sourceList.AsQueryable();
        var mockSet = new Mock<DbSet<Note>>();
        mockSet.As<IQueryable<Note>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<Note>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<Note>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<Note>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        return mockSet;
    }

    private Mock<ToDoContext> CreateMockContext(Mock<DbSet<Note>> mockSet)
    {
        var mockContext = new Mock<ToDoContext>();
        mockContext.Setup(c => c.Notes).Returns(mockSet.Object);
        return mockContext;
    }


    [Fact]
    public void GetNotes_WhenCalled_ReturnsAllNotes()
    {
        var testData = new List<Note>
        {
            new Note { ID = 1, Text = "Note 1", IsDone = false },
            new Note { ID = 2, Text = "Note 2", IsDone = true }
        };
        var mockSet = CreateMockSet(testData);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);

        var notes = service.GetNotes(null).ToList();

        Assert.Equal(2, notes.Count);
        Assert.Contains(testData[0], notes);
        Assert.Contains(testData[1], notes);
    }

    [Fact]
    public void GetRemainingCount_WhenCalled_ReturnsCountOfUncompletedNotes()
    {
        var testData = new List<Note>
    {
        new Note { ID = 1, Text = "Note 1", IsDone = false },
        new Note { ID = 2, Text = "Note 2", IsDone = true }
    };
        var mockSet = CreateMockSet(testData);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);

        var count = service.GetRemainingCount();

        Assert.Equal(1, count);
    }

    [Fact]
    public void AddNote_WhenCalled_AddsNote()
    {
        var mockSet = new Mock<DbSet<Note>>();
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);
        var noteToAdd = new Note { Text = "New Note", IsDone = false };

        service.AddNote(noteToAdd);

        mockSet.Verify(s => s.Add(It.IsAny<Note>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void UpdateNote_WhenCalled_UpdatesNote()
    {
        var testData = new List<Note>
    {
        new Note { ID = 1, Text = "Note 1", IsDone = false }
    };
        var mockSet = CreateMockSet(testData);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);
        var updatedNote = new Note { ID = 1, Text = "Updated Note 1", IsDone = true };

        service.UpdateNote(1, updatedNote);

        var note = testData.FirstOrDefault(n => n.ID == 1);
        Assert.NotNull(note);
        Assert.Equal("Updated Note 1", note.Text);
        Assert.True(note.IsDone);
    }

    [Fact]
    public void DeleteNote_WhenCalled_DeletesNote()
    {
        var testNotes = new List<Note>
            {
                new Note { ID = 1, Text = "Test Note 1", IsDone = false },
                new Note { ID = 2, Text = "Test Note 2", IsDone = true }
            };

        var mockSet = CreateMockSet(testNotes);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);

        service.DeleteNote(1);

        mockSet.Verify(m => m.Remove(It.Is<Note>(n => n.ID == 1)), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }
}