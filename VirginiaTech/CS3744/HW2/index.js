// Implemented with the assistance of Github Copilot
(function () {
  const TRUNC_LIMIT = 30;
  const TRUNC_TO = 25;

  let overdueFilter = false;

  let tasks = [
    {
      id: 0,
      title: "Throw away the trash",
      dueDate: new Date(2026, 8, 29, 23, 59, 59),
      completed: false,
      completeDate: null,
      createdDate: new Date(2026, 8, 10, 23, 59, 59),
      deleted: false,
      note: "I need to get quarters first at Kroger."
    },
    {
      id: 1,
      title: "UI Assignment 3",
      dueDate: new Date(2026, 9, 27, 23, 59, 59),
      completed: false,
      completeDate: null,
      createdDate: new Date(2026, 9, 2, 23, 59, 59),
      deleted: false,
      note: "I better start early cuz it looks pretty complicated.\r\nLooks like I have to read w3schools.com a lot."
    },
    {
      id: 2,
      title: "Getting Milk",
      dueDate: null,
      completed: true,
      completeDate: new Date(2026, 9, 22, 23, 59, 59),
      createdDate: new Date(2026, 9, 10, 23, 59, 59),
      deleted: false,
      note: "I need to get milk for the kids."
    },
    {
      id: 3,
      title: "Get tickets to Hamilton",
      dueDate: new Date(2026, 9, 12, 23, 59, 59),
      completed: false,
      completeDate: null,
      createdDate: new Date(2026, 9, 12, 23, 59, 59),
      deleted: false,
      note: "I would have to book a flight ticket to ACM UIST conference.\r\nKeep an eye on the cancellation policy. the conference may be cancelled due to the outbreak. :( Although flight tickets are getting cheaper."
    }
  ];

  window.tasks = tasks;

  function truncate(input, number, numberTo) {
    return input.length > number ? input.substring(0, numberTo) + '...' : input;
  }

  function getFormattedDate(date) {
    if (!date || Number.isNaN(date.getTime())) {
      return '';
    }
    const year = date.getFullYear();
    const month = ('0' + (1 + date.getMonth())).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${month}/${day}/${year}`;
  }

  function isOverdue(task) {
    if (!task || task.completed || task.deleted || !task.dueDate) {
      return false;
    }

    const now = new Date();
    const dueDate = new Date(task.dueDate.getTime());
    return now > dueDate;
  }

  function parseDueDate(inputValue) {
    if (!inputValue || !inputValue.trim()) {
      return null;
    }

    const value = inputValue.trim();
    const dateRegex = /^\d{2}\/\d{2}\/\d{4}$/;
    if (!dateRegex.test(value)) {
      return null;
    }

    const [monthText, dayText, yearText] = value.split('/');
    const month = Number.parseInt(monthText, 10);
    const day = Number.parseInt(dayText, 10);
    const year = Number.parseInt(yearText, 10);
    const candidate = new Date(year, month - 1, day, 23, 59, 59);

    if (
      candidate.getFullYear() !== year ||
      candidate.getMonth() !== month - 1 ||
      candidate.getDate() !== day
    ) {
      return null;
    }

    return candidate;
  }

  function isValidDueDateInput(inputValue) {
    return parseDueDate(inputValue) !== null;
  }

  function updateDeleteCompletedButtonState() {
    const button = document.getElementById('deleteCompletedTasks');
    if (!button) {
      return;
    }

    const hasCompletedTask = tasks.some(task => !task.deleted && task.completed);
    button.disabled = !hasCompletedTask;
    button.classList.toggle('disabled', !hasCompletedTask);
  }

  function updateOverdueButtonState() {
    const overdueButton = document.getElementById('overdue');
    if (!overdueButton) {
      return;
    }

    const link = overdueButton.querySelector('a');
    if (link) {
      link.classList.toggle('text-danger', overdueFilter);
    }
  }

  function updateAddTaskButtonState() {
    const addTaskButton = document.querySelector('.addtask');
    const inputContainer = document.getElementById('task-input-container');
    if (!addTaskButton || !inputContainer) {
      return;
    }

    const isOpen = inputContainer.classList.contains('show') || inputContainer.getAttribute('aria-expanded') === 'true';
    addTaskButton.classList.toggle('d-none', isOpen);
  }

  function clearTaskInputForm() {
    const title = document.getElementById('task-title');
    const description = document.getElementById('task-description');
    const dueDate = document.getElementById('due-date');

    if (title) {
      title.value = '';
    }
    if (description) {
      description.value = '';
    }
    if (dueDate) {
      dueDate.value = '';
    }
  }

  function openTaskInputContainer() {
    const inputContainer = document.getElementById('task-input-container');
    if (!inputContainer) {
      return;
    }

    inputContainer.classList.add('show');
    inputContainer.setAttribute('aria-expanded', 'true');

    const addTaskButton = document.querySelector('.addtask');
    if (addTaskButton) {
      addTaskButton.classList.add('d-none');
    }

    updateAddTaskButtonState();
  }

  function closeTaskInputContainer() {
    const inputContainer = document.getElementById('task-input-container');
    if (!inputContainer) {
      return;
    }

    inputContainer.classList.remove('show');
    inputContainer.classList.remove('in');
    inputContainer.setAttribute('aria-expanded', 'false');

    const addTaskButton = document.querySelector('.addtask');
    if (addTaskButton) {
      addTaskButton.classList.remove('d-none');
    }

    updateAddTaskButtonState();
  }

  function renderTask(task) {
    const taskRow = document.createElement('div');
    taskRow.className = 'row my-2 py-2';
    taskRow.id = String(task.id);
    taskRow.dataset.taskId = String(task.id);

    if (task.completed) {
      taskRow.classList.add('text-muted', 'completed');
    } else if (isOverdue(task)) {
      taskRow.classList.add('text-danger', 'overdue');
    } else {
      taskRow.classList.add('not-overdue');
    }

    const checkCol = document.createElement('div');
    checkCol.className = 'col-1 text-center';

    const checkbox = document.createElement('input');
    checkbox.type = 'checkbox';
    checkbox.className = 'form-check-input';
    checkbox.value = String(task.id);
    checkbox.checked = task.completed;
    checkbox.addEventListener('change', function () {
      const currentTask = tasks.find(item => item.id === task.id);
      if (!currentTask) {
        return;
      }

      currentTask.completed = checkbox.checked;
      currentTask.completeDate = checkbox.checked ? new Date() : null;
      renderTasks();
    });
    checkCol.appendChild(checkbox);

    const titleCol = document.createElement('div');
    titleCol.className = 'col-5 text-center';
    titleCol.setAttribute('role', 'button');
    titleCol.setAttribute('data-bs-toggle', 'collapse');
    titleCol.setAttribute('data-bs-target', `#note-${task.id}`);

    const titleContent = document.createElement(task.completed ? 'del' : 'span');
    titleContent.textContent = truncate(task.title, TRUNC_LIMIT, TRUNC_TO);
    titleCol.appendChild(titleContent);

    const dueCol = document.createElement('div');
    dueCol.className = 'col-2 text-center';
    dueCol.textContent = task.dueDate ? getFormattedDate(task.dueDate) : '';

    const completeCol = document.createElement('div');
    completeCol.className = 'col-2 text-center';
    completeCol.textContent = task.completeDate ? getFormattedDate(task.completeDate) : '';

    const toolsCol = document.createElement('div');
    toolsCol.className = 'col-2 text-center';

    const deleteButton = document.createElement('button');
    deleteButton.type = 'button';
    deleteButton.className = 'btn btn-light text-danger btn-xs deletetask mx-1';
    deleteButton.alt = 'Delete the task';
    deleteButton.value = String(task.id);
    deleteButton.setAttribute('aria-label', 'Delete task');
    deleteButton.addEventListener('click', function () {
      if (window.confirm('Are you sure you want to delete this task?')) {
        task.deleted = true;
        renderTasks();
      }
    });

    const deleteIcon = document.createElement('i');
    deleteIcon.className = 'bi bi-trash';
    deleteButton.appendChild(deleteIcon);
    toolsCol.appendChild(deleteButton);

    const emailLink = document.createElement('a');
    emailLink.href = `mailto:?body=${encodeURIComponent(task.note || '')}&subject=${encodeURIComponent(task.title)}`;
    emailLink.target = '_blank';

    const emailButton = document.createElement('button');
    emailButton.type = 'button';
    emailButton.className = 'btn btn-light text-primary btn-xs emailtask mx-1';
    emailButton.alt = 'Send an email';
    emailButton.value = String(task.id);
    emailButton.setAttribute('aria-label', 'Email task');

    const emailIcon = document.createElement('i');
    emailIcon.className = 'bi bi-envelope';
    emailButton.appendChild(emailIcon);
    emailLink.appendChild(emailButton);
    toolsCol.appendChild(emailLink);

    taskRow.appendChild(checkCol);
    taskRow.appendChild(titleCol);
    taskRow.appendChild(dueCol);
    taskRow.appendChild(completeCol);
    taskRow.appendChild(toolsCol);

    const noteContainer = document.createElement('div');
    noteContainer.id = `note-${task.id}`;
    noteContainer.className = 'collapse';

    const card = document.createElement('div');
    card.className = 'card';

    const cardBody = document.createElement('div');
    cardBody.className = 'card-body';

    const noteHeader = document.createElement('h3');
    noteHeader.textContent = task.title;

    const noteText = document.createElement('div');
    noteText.innerHTML = (task.note || '').replace(/\r\n|\r|\n/g, '<br>');

    cardBody.appendChild(noteHeader);
    cardBody.appendChild(noteText);
    card.appendChild(cardBody);
    noteContainer.appendChild(card);

    if (overdueFilter && !isOverdue(task)) {
      taskRow.classList.add('d-none');
      noteContainer.classList.add('d-none');
    }

    return { taskRow, noteContainer };
  }

  function renderTasks() {
    const taskListContainer = document.getElementById('task-list-container');
    if (!taskListContainer) {
      return;
    }

    while (taskListContainer.firstChild) {
      taskListContainer.removeChild(taskListContainer.firstChild);
    }

    tasks.forEach(task => {
      if (task.deleted) {
        return;
      }

      const renderedTask = renderTask(task);
      taskListContainer.appendChild(renderedTask.taskRow);
      taskListContainer.appendChild(renderedTask.noteContainer);
    });

    updateOverdueButtonState();
    updateDeleteCompletedButtonState();
    updateAddTaskButtonState();
  }

  function handleAddTask() {
    const titleInput = document.getElementById('task-title');
    const descriptionInput = document.getElementById('task-description');
    const dueDateInput = document.getElementById('due-date');

    if (!titleInput || !descriptionInput || !dueDateInput) {
      return;
    }

    const title = titleInput.value.trim();
    if (!title) {
      alert('Please enter a task title.');
      titleInput.focus();
      return;
    }

    const enteredDueDate = dueDateInput.value.trim();
    if (enteredDueDate && !isValidDueDateInput(enteredDueDate)) {
      alert('Write the due date in mm/dd/yyyy format');
      dueDateInput.focus();
      return;
    }

    if (overdueFilter) {
      overdueFilter = false;
    }

    const newTask = {
      id: tasks.length,
      title: title,
      dueDate: enteredDueDate ? parseDueDate(enteredDueDate) : null,
      completed: false,
      completeDate: null,
      createdDate: new Date(),
      deleted: false,
      note: descriptionInput.value
    };

    tasks.push(newTask);
    window.tasks = tasks;
    renderTasks();
    clearTaskInputForm();
    closeTaskInputContainer();
  }

  function handleDeleteCompletedTasks() {
    const completedTasks = tasks.filter(task => !task.deleted && task.completed);
    if (completedTasks.length === 0) {
      return;
    }

    tasks = tasks.map(task => {
      if (task.completed && !task.deleted) {
        return { ...task, deleted: true };
      }
      return task;
    });

    window.tasks = tasks;
    renderTasks();
  }

  function bindStaticEvents() {
    const addTaskButton = document.querySelector('.addtask');
    if (addTaskButton) {
      addTaskButton.removeAttribute('data-bs-toggle');
      addTaskButton.removeAttribute('data-bs-target');
      addTaskButton.addEventListener('click', function (event) {
        event.preventDefault();
        if (overdueFilter) {
          overdueFilter = false;
          renderTasks();
        }
        openTaskInputContainer();
      });
    }

    const submitButton = document.getElementById('submit-task');
    if (submitButton) {
      submitButton.addEventListener('click', handleAddTask);
    }

    const cancelButton = document.getElementById('cancel-task');
    if (cancelButton) {
      cancelButton.removeAttribute('data-bs-toggle');
      cancelButton.removeAttribute('data-bs-target');
      cancelButton.addEventListener('click', function (event) {
        event.preventDefault();
        clearTaskInputForm();
        closeTaskInputContainer();
      });
    }

    const deleteCompletedButton = document.getElementById('deleteCompletedTasks');
    if (deleteCompletedButton) {
      deleteCompletedButton.addEventListener('click', handleDeleteCompletedTasks);
    }

    const overdueNavItem = document.getElementById('overdue');
    if (overdueNavItem) {
      const overdueLink = overdueNavItem.querySelector('a');
      if (overdueLink) {
        overdueLink.addEventListener('click', function (event) {
          event.preventDefault();
          overdueFilter = !overdueFilter;
          renderTasks();
        });
      }
    }

    const inputContainer = document.getElementById('task-input-container');
    if (inputContainer) {
      inputContainer.addEventListener('shown.bs.collapse', updateAddTaskButtonState);
      inputContainer.addEventListener('hidden.bs.collapse', updateAddTaskButtonState);
    }
  }

  document.addEventListener('DOMContentLoaded', function () {
    bindStaticEvents();
    renderTasks();
    updateAddTaskButtonState();
  });

  window.truncate = truncate;
  window.renderTasks = renderTasks;
})();
