global using NUnit.Framework;                  // For NUnit test framework
global using Moq;                              // For mocking (Moq library)
global using AutoMapper;                       // For AutoMapper-related tests
global using System.ComponentModel.DataAnnotations;  // For validation attributes testing
global using System.Collections.Generic;       // For collections like List<>
global using DocumentsREST.BL.DTOs;            // For DTOs used in tests
global using DocumentsREST.DAL.Models;         // For your entity models
global using DocumentsREST.DAL.Repositories;   // For repository tests
global using Microsoft.EntityFrameworkCore;    // For in-memory EF Core testing
global using System.Threading.Tasks;           // For async methods in tests
global using DocumentsREST.BL.Services;        // For service tests
global using DocumentsREST.Mappings;
global using DocumentsREST.DAL;