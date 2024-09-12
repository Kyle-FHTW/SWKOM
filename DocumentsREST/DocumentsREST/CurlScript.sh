#!/bin/bash

# Base URL
BASE_URL="http://localhost:8081"

# Get All Documents
echo "Getting all documents..."
curl -X GET "$BASE_URL/"
echo -e "\n"

# Get Document by ID
echo "Getting document with ID 1..."
curl -X GET "$BASE_URL/documents/1"
echo -e "\n"

# Create a New Document
echo "Creating a new document..."
curl -X POST "$BASE_URL/documents" -H "Content-Type: application/json" -d '{
  "Id": 3,
  "Title": "Document 3",
  "Metadata": "Metadata 3",
  "Description": "Description 3",
  "TagsAssigned": []
}'
echo -e "\n"

# Get All Documents
echo "Getting all documents..."
curl -X GET "$BASE_URL/"
echo -e "\n"

# Update an Existing Document
echo "Updating document with ID 1..."
curl -X PUT "$BASE_URL/documents/1" -H "Content-Type: application/json" -d '{
  "Id": 1,
  "Title": "Updated Document 1",
  "Metadata": "Updated Metadata 1",
  "Description": "Updated Description 1",
  "TagsAssigned": []
}'
echo -e "\n"

# Show Document by ID after Update
echo "Getting document with ID 1 after update..."
curl -X GET "$BASE_URL/documents/1"
echo -e "\n"

# Delete a Document
echo "Deleting document with ID 1..."
curl -X DELETE "$BASE_URL/documents/1"
echo -e "\n"

# Get All Documents
echo "Getting all documents..."
curl -X GET "$BASE_URL/"
echo -e "\n"