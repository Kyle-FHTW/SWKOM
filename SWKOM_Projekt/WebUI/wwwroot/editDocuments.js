const apiUrl = 'http://localhost:8081/documents'; // API URL

// Function to load the document data into the form
function loadDocumentData() {
    const urlParams = new URLSearchParams(window.location.search);
    const documentId = urlParams.get('id');

    fetch(`${apiUrl}/${documentId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(doc => {
            document.getElementById('title').value = doc.title;
            document.getElementById('description').value = doc.description;
            document.getElementById('documentForm').dataset.id = doc.id;
        })
        .catch(error => {
            console.error('Error loading document data:', error);
            alert('Failed to load document data. Please try again later.');
        });
}

// Function to update a document via PUT request
function updateDocument() {
    const documentId = document.getElementById('documentForm').dataset.id;
    const title = document.getElementById('title').value;
    const description = document.getElementById('description').value;

    const updatedDocument = {
        id: documentId,
        title,
        description,
    };

    fetch(`${apiUrl}/${documentId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedDocument)
    })
        .then(response => {
            if (response.ok) {
                alert('Document updated successfully!');
                window.location.href = 'Index.html';
            } else {
                return response.json().then(err => alert('Error: ' + err.message));
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while updating the document.');
        });
}

// Ensure this function is called when the page loads
window.onload = loadDocumentData;