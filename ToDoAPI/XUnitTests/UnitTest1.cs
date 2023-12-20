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
        mockSet.As<IQueryable<Note>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator);
        mockSet.Setup(m => m.Remove(It.IsAny<Note>()))
           .Callback<Note>(note => sourceList.Remove(note));
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
        // Arrange
        var testData = new List<Note>
        {
            new Note { ID = 1, Title = "Title 1", Description = "Description 1", IsDone = false },
            new Note { ID = 2, Title = "Title 2", Description = "Description 2", IsDone = true }
        };
        var mockSet = CreateMockSet(testData);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);

        // Act
        var notes = service.GetNotes(null).ToList();

        // Assert
        Assert.Equal(2, notes.Count);
        Assert.Contains(testData[0], notes);
        Assert.Contains(testData[1], notes);
    }

    [Fact]
    public void GetRemainingCount_WhenCalled_ReturnsCountOfUncompletedNotes()
    {
        // Arrange
        var testData = new List<Note>
        {
            new Note { ID = 1, Title = "Title 1", Description = "Description 1", IsDone = false },
            new Note { ID = 2, Title = "Title 2", Description = "Description 2", IsDone = true }
        };
        var mockSet = CreateMockSet(testData);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);

        // Act
        var count = service.GetRemainingCount();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void AddNote_WhenCalled_AddsNote()
    {
        // Arrange
        var notes = new List<Note>();
        var mockSet = new Mock<DbSet<Note>>();
        mockSet.Setup(m => m.Add(It.IsAny<Note>()))
               .Callback<Note>(note => notes.Add(note));

        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);
        var noteToAdd = new Note { Title = "New Title", Description = "New Description", IsDone = false };
        // Act
        service.AddNote(noteToAdd);

        // Assert
        Assert.Single(notes);
        Assert.Contains(noteToAdd, notes);
        mockContext.Verify(m => m.SaveChanges(), Times.Exactly(1));
    }

    [Fact]
    public void UpdateNote_WhenCalled_UpdatesNoteToDone()
    {
        // Arrange
        var testData = new List<Note>
    {
        new Note { ID = 1, Title = "Title 2", Description = "Description 2", IsDone = false }
    };
        var mockSet = CreateMockSet(testData);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);
        var updatedNote = new Note { ID = 1, Title = "Updated Title", Description = "Updated Description", IsDone = true };

        // Act
        service.UpdateNote(1, updatedNote);

        var note = testData.FirstOrDefault(n => n.ID == 1);
        // Assert
        Assert.NotNull(note);
        Assert.True(note.IsDone);
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteNote_WhenCalled_DeletesNote()
    {
        // Arrange
        var testNotes = new List<Note>
    {
        new Note { ID = 1, Title = "Title 1", Description = "Description 1", IsDone = false },
        new Note { ID = 2, Title = "Title 2", Description = "Description 2", IsDone = true }
    };
        var mockSet = CreateMockSet(testNotes);
        var mockContext = CreateMockContext(mockSet);
        var service = new NotesService(mockContext.Object);

        // Act
        service.DeleteNote(1);

        // Assert
        Assert.DoesNotContain(testNotes, n => n.ID == 1);
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }
}