﻿-- Create the Documents table
CREATE TABLE IF NOT EXISTS "Documents" (
                             "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
                             "Title" text,
                             "Metadata" text,
                             "Description" text,
                             CONSTRAINT "PK_Documents" PRIMARY KEY ("Id")
);

-- Insert initial data into the Documents table
INSERT INTO "Documents" ("Title", "Metadata", "Description")
VALUES ('Document 1', 'metadata 1', 'description 1');

INSERT INTO "Documents" ("Title", "Metadata", "Description")
VALUES ('Document 2', 'metadata 2', 'description 2');