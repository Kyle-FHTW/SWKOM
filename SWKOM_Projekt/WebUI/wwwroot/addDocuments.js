const apiUrl = 'http://localhost:8081/documents'; // API URL

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

    // Create a new document object (excluding id)
    const newDocument = {
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
        body: JSON.stringify(newDocument) // Do not include 'id' here
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
