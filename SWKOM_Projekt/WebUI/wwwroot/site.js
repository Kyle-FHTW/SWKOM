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

                // Create an edit button with Bootstrap styling
                const editButton = document.createElement('button');
                editButton.textContent = 'Edit';
                editButton.className = 'btn btn-warning btn-sm'; // Bootstrap classes
                editButton.onclick = () => editDocument(doc); // Attach edit function

                const actionCell = document.createElement('td');
                actionCell.appendChild(editButton);
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

// Function to edit a document
function editDocument(doc) {
    // Redirect to editDocuments.html with the document ID as a query parameter
    window.location.href = `editDocuments.html?id=${doc.id}`;
}

// Load documents when the page is loaded
window.onload = loadDocuments;