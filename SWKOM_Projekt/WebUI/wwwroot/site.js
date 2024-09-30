const apiUrl = 'http://localhost:8081/documents'; // API URL

// Function to load and display the data in the table
function loadDocuments() {
    const tableBody = document.getElementById('document-table-body');
    const errorElement = document.getElementById('error');

    // Fetch data from the server
    fetch(apiUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(documents => {
            // Clear the table before loading new data
            tableBody.innerHTML = '';

            // Loop through the documents and create table rows
            documents.forEach(doc => {
                const row = document.createElement('tr');

                // Insert cells for each field
                row.innerHTML = `
                    <td>${doc.id}</td>
                    <td>${doc.title}</td>
                    <td>${doc.metadata}</td>
                    <td>${doc.description}</td>
                `;
                // Create a delete button with Bootstrap styling
                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Delete';
                deleteButton.className = 'btn btn-danger btn-sm'; // Bootstrap classes
                deleteButton.onclick = () => deleteDocument(doc.id); // Attach delete function

                const actionCell = document.createElement('td');
                actionCell.appendChild(deleteButton);
                row.appendChild(actionCell);

                // Append row to the table body
                tableBody.appendChild(row);
            });
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
            errorElement.textContent = 'Failed to load documents. Please try again later.';
            errorElement.style.display = 'block';
        });
}

// Function to add a new document via POST request
function addDocument() {
    // Get form values
    const title = document.getElementById('title').value;
    const metadata = document.getElementById('metadata').value;
    const description = document.getElementById('description').value;

    // Validate the title (optional, but we'll alert if empty)
    if (title.trim() === '') {
        alert('Please enter a title for the document');
        return;
    }

    // Generate a new ID for the document (for example, using the current timestamp)
    const newId = Date.now(); // Simple unique ID generation

    // Create a new document object
    const newDocument = {
        id: newId,  // Assign the generated ID
        title: title,
        metadata: metadata,
        description: description,
    };

    // Send the document object to the API
    fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newDocument)
    })
        .then(response => {
            if (response.ok) {
                alert('Document added successfully!');
                // Redirect to the index page after successful submission
                window.location.href = 'Index.html';
            } else {
                return response.json().then(err => {
                    alert('Error: ' + err.message);
                });
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while submitting the document.');
        });
}

// Function to delete a document
function deleteDocument(id) {
    fetch(`${apiUrl}/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                alert('Document deleted successfully!');
                loadDocuments(); // Refresh the document list
            } else {
                response.json().then(err => {
                    alert('Error: ' + err.message);
                });
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while deleting the document.');
        });
}

// Load documents when the page is loaded
window.onload = loadDocuments;