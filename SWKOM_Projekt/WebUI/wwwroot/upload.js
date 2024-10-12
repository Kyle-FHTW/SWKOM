async function uploadDocument() {
    // Get form data
    const description = document.getElementById('description').value;
    const fileInput = document.getElementById('file');
    const file = fileInput.files[0];  // Get the selected file

    // Ensure file is selected
    if (!file) {
        alert('Please select a file.');
        return;
    }

    // Client-side check for PDF file type
    if (file.type !== 'application/pdf') {
        alert('Please upload a valid PDF file.');
        return;
    }

    // Client-side check for file size (e.g., 20 MB limit)
    const maxSizeInBytes = 20 * 1024 * 1024; // 20 MB
    if (file.size > maxSizeInBytes) {
        alert('File size exceeds the 20 MB limit. Please upload a smaller file.');
        return;
    }

    // Create FormData object
    const formData = new FormData();
    formData.append('description', description);
    formData.append('file', file);

    try {
        // Send the form data to the server
        const response = await fetch('/documents/upload', {
            method: 'POST',
            body: formData
        });

        // Parse the JSON response
        const result = await response.json();

        if (response.ok) {
            // If upload is successful, redirect to index.html
            window.location.href = 'index.html';
        } else {
            // Handle specific error messages returned by the server
            document.getElementById('result').innerText = `Error: ${result.message || 'Upload failed'}`;
            document.getElementById('result').classList.add('text-danger');
        }
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('result').innerText = 'An error occurred during the upload.';
        document.getElementById('result').classList.add('text-danger');
    }
}
