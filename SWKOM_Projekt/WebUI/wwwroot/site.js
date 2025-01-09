const apiUrl = 'http://localhost:8081/documents'; // API URL

function loadDocuments() {
    const tableBody = document.getElementById('document-table-body');
    const errorElement = document.getElementById('error');

    console.log('Loading documents...'); // Debug log

    // Fetch data from the server
    fetch(apiUrl)
        .then(response => {
            console.log('Response received:', response.status); // Debug log
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(documents => {
            console.log('Documents fetched:', documents); // Debug log

            // Clear the table before loading new data
            tableBody.innerHTML = '';

            // Loop through the documents and create table rows
            documents.forEach(doc => {
                console.log('Processing document:', doc); // Debug log
                const row = document.createElement('tr');

                // Insert cells for each field
                row.innerHTML = `
                    <td>${doc.id}</td>
                    <td>${doc.title}</td>
                    <td>${doc.description}</td>
                `;

                // Create buttons: Delete, Edit, and Download
                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Delete';
                deleteButton.className = 'btn btn-danger btn-sm';
                deleteButton.onclick = () => deleteDocument(doc.id);

                const editButton = document.createElement('button');
                editButton.textContent = 'Edit';
                editButton.className = 'btn btn-warning btn-sm';
                editButton.onclick = () => editDocument(doc);

                const downloadButton = document.createElement('button');
                downloadButton.textContent = 'Download';
                downloadButton.className = 'btn btn-success btn-sm';
                downloadButton.onclick = () => downloadDocument(doc.id);

                const actionCell = document.createElement('td');
                actionCell.appendChild(editButton);
                actionCell.appendChild(deleteButton);
                actionCell.appendChild(downloadButton);
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

function downloadDocument(id) {
    // First, fetch the document metadata
    fetch(`${apiUrl}/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch document metadata.');
            }
            return response.json(); // Parse JSON response
        })
        .then(document => {
            console.log('Document metadata:', document); // Debug log

            // Fetch the file for download
            return fetch(`${apiUrl}/${id}/download`, {
                method: 'GET',
            }).then(response => {
                if (!response.ok) {
                    throw new Error('Failed to download document.');
                }
                return response.blob().then(blob => ({ blob, filename: `${document.title}.pdf` }));
            });
        })
        .then(({ blob, filename }) => {
            console.log('Downloading file:', filename); // Debug log

            // Create a URL for the blob and trigger the download
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = filename; // Use the title as the filename
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Error during download:', error);
            alert('An error occurred while downloading the document.');
        });
}




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

window.onload = loadDocuments;