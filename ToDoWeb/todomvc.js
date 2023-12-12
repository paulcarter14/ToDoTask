const api = 'https://localhost:7209';

// Find all the various DOM elements.
const addForm = document.querySelector('#add-form');
const addInput = document.querySelector('#add-input');
const toggleAllButton = document.querySelector('#toggle-all-button');
const todoList = document.querySelector('#todo-list');
const infoRow = document.querySelector('#info-row');
const itemsLeft = document.querySelector('#items-left');
const showAllButton = document.querySelector('#show-all-button');
const showActiveButton = document.querySelector('#show-active-button');
const showCompletedButton = document.querySelector('#show-completed-button');
const clearCompletedButton = document.querySelector('#clear-completed-button');

// This variable will be set to one of the three values when clicking the various "filter" buttons.
// null: all notes should be shown
// true: only completed notes should be shown
// false: only non-completed ("active") notes should be shown
let completed = null;

function start() {
    loadNotes();
}

async function loadNotes() {
    // Get the notes from the backend, possibly with a query parameter for only loading completed or non-completed notes.
    let url = api + '/notes';
    if (completed === true) {
        url += '?completed=true';
    }
    else if (completed === false) {
        url += '?completed=false';
    }

    const response = await fetch(url);
    const notes = await response.json();

    // If there are notes, show the bottom bar.
    // Note that we don't actually hide it again if all notes are removed.
    if (notes.length > 0) {
        infoRow.hidden = false;
    }

    // Every time something changes, remove all the notes and create them again.
    // This is bad for performance, but makes it a lot easier to ensure that the data is properly synced with the backend.
    todoList.replaceChildren();
    for (const note of notes) {
        showNote(note)
    }

    // Show the number of notes.
    const remainingResponse = await fetch(api + '/remaining');
    const remaining = await remainingResponse.json();
    const noun = remaining === 1 ? 'item' : 'items';
    itemsLeft.textContent = remaining + ' ' + noun + ' left';
}

function showNote(note) {
    // Get the note template and copy it.
    const template = document.querySelector('#todo-item-template');
    const li = template.content.firstElementChild.cloneNode(true);
    
    // Add a class if it's checked, for CSS purposes.
    if (note.isDone) {
        li.classList.add('checked');
    }

    // Activate the checkbox for the note.
    const checkbox = li.querySelector('.note-checkbox');
    checkbox.checked = note.isDone;
    checkbox.onchange = async () => {
        // Send the updated note to the server.
        // Note that the PUT request requires all values, even those that have not changed.
        await fetch(api + '/notes/' + note.id, {
            method: 'PUT',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                id: note.id,
                text: note.text,
                isDone: checkbox.checked,
            }),
        });
        loadNotes();
    };

    // Add the note text itself.
    const span = li.querySelector('.note-text');
    span.textContent = note.text;

    // Activate the delete button.
    const deleteButton = li.querySelector('.delete-button');
    deleteButton.onclick = async () => {
        await fetch(api + '/notes/' + note.id, {method: 'DELETE'});
        loadNotes();
    };

    // Add to the document.
    todoList.append(li);
}

addForm.onsubmit = async event => {
    event.preventDefault();

    const noteText = addInput.value;
    if (noteText.trim() !== '') {
        // Clear the input after adding a note.
        addInput.value = '';

        // Send the note to the backend.
        const note = {
            text: noteText,
            isDone: false,
        };
        await fetch(api + '/notes', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(note),
        });

        // Get all the notes, including the new one.
        loadNotes();
    }
};

// Relatively simple code below for the various "global" buttons.
toggleAllButton.onclick = async () => {
    await fetch(api + '/toggle-all', {method: 'POST'});
    loadNotes();
};

showAllButton.onclick = () => {
    completed = null;
    showAllButton.classList.add('selected');
    showActiveButton.classList.remove('selected');
    showCompletedButton.classList.remove('selected');
    loadNotes();
};

showActiveButton.onclick = () => {
    completed = false;
    showAllButton.classList.remove('selected');
    showActiveButton.classList.add('selected');
    showCompletedButton.classList.remove('selected');
    loadNotes();
};

showCompletedButton.onclick = () => {
    completed = true;
    showAllButton.classList.remove('selected');
    showActiveButton.classList.remove('selected');
    showCompletedButton.classList.add('selected');
    loadNotes();
};

clearCompletedButton.onclick = async () => {
    await fetch(api + '/clear-completed', {method: 'POST'});
    loadNotes();
};

start();