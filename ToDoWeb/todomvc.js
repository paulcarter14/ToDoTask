const api = 'https://localhost:7209';

const addForm = document.querySelector('#add-form');
const addTitleInput = document.querySelector('#add-title');
const addDescriptionInput = document.querySelector('#add-description');
const toggleAllButton = document.querySelector('#toggle-all-button');
const todoList = document.querySelector('#todo-list');
const infoRow = document.querySelector('#info-row');
const itemsLeft = document.querySelector('#items-left');
const showAllButton = document.querySelector('#show-all-button');
const showActiveButton = document.querySelector('#show-active-button');
const showCompletedButton = document.querySelector('#show-completed-button');
const clearCompletedButton = document.querySelector('#clear-completed-button');

let completed = null;

function start() {
    loadNotes();
}

async function loadNotes() {
    let url = api + '/notes';
    if (completed !== null) {
        url += '?completed=' + completed;
    }

    const response = await fetch(url);
    const notes = await response.json();

    todoList.replaceChildren();
    for (const note of notes) {
        showNote(note);
    }

    const remainingResponse = await fetch(api + '/remaining');
    const remaining = await remainingResponse.json();
    itemsLeft.textContent = `${remaining} item${remaining !== 1 ? 's' : ''} left`;
}

function showNote(note) {
    const template = document.querySelector('#todo-item-template');
    const li = template.content.firstElementChild.cloneNode(true);

    li.classList.toggle('checked', note.isDone);

    const checkbox = li.querySelector('.note-checkbox');
    checkbox.checked = note.isDone;
    checkbox.onchange = async () => {
        await fetch(api + '/notes/' + note.id, {
            method: 'PUT',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                id: note.id,
                title: note.title,
                description: note.description,
                isDone: checkbox.checked,
            }),
        });
        loadNotes();
    };

    const titleElement = li.querySelector('.note-title');
    titleElement.textContent = note.title;

    const descriptionElement = li.querySelector('.note-description');
    descriptionElement.textContent = note.description;

    const deleteButton = li.querySelector('.delete-button');
    deleteButton.onclick = async () => {
        await fetch(api + '/notes/' + note.id, { method: 'DELETE' });
        loadNotes();
    };

    todoList.append(li);
}

addForm.onsubmit = async event => {
    event.preventDefault();

    const title = addTitleInput.value.trim();
    const description = addDescriptionInput.value.trim();

    if (title && description) {
        const note = { title, description, isDone: false };
        await fetch(api + '/notes', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(note)
        });

        addTitleInput.value = '';
        addDescriptionInput.value = '';
        loadNotes();
    }
};

toggleAllButton.onclick = async () => {
    await fetch(api + '/toggle-all', { method: 'POST' });
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
    await fetch(api + '/clear-completed', { method: 'POST' });
    loadNotes();
};

start();