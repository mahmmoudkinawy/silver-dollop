﻿using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
        IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository ??
            throw new ArgumentNullException(nameof(courseLibraryRepository));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet(Name = "GetAuthors")]
    [HttpHead]
    public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
        [FromQuery] AuthorsResourceParameters authorsResourceParameters)
    {
        var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParameters);

        var previousPageLink = authorsFromRepo.HasPrevious ?
            CreateAuthorsResourceUri(authorsResourceParameters, ResouceUriType.PreviousPage)
            : null;

        var nextPageLink = authorsFromRepo.HasNext ?
            CreateAuthorsResourceUri(authorsResourceParameters, ResouceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = authorsFromRepo.TotalCount,
            pageSize = authorsFromRepo.PageSize,
            totalPages = authorsFromRepo.TotalPages,
            currentPage = authorsFromRepo.CurrentPage,
            previousPageLink,
            nextPageLink
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

        return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
    }

    [HttpGet("{authorId}", Name = "GetAuthor")]
    public IActionResult GetAuthor(Guid authorId)
    {
        var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

        if (authorFromRepo == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
    }

    [HttpPost]
    public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
    {
        var authorEntity = _mapper.Map<Entities.Author>(author);
        _courseLibraryRepository.AddAuthor(authorEntity);
        _courseLibraryRepository.Save();

        var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
        return CreatedAtRoute("GetAuthor",
            new { authorId = authorToReturn.Id },
            authorToReturn);
    }

    [HttpOptions]
    public IActionResult GetAuthorsOptions()
    {
        Response.Headers.Add("Allow", "GET,OPTIONS,POST");
        return Ok();
    }

    [HttpDelete("{authorId}")]
    public ActionResult DeleteAuthor(Guid authorId)
    {
        var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

        if (authorFromRepo == null)
        {
            return NotFound();
        }

        _courseLibraryRepository.DeleteAuthor(authorFromRepo);

        _courseLibraryRepository.Save();

        return NoContent();
    }

    private string CreateAuthorsResourceUri(
        AuthorsResourceParameters authorsResourceParameters,
        ResouceUriType type)
    {
        return type switch
        {
            ResouceUriType.PreviousPage => Url.Link("GetAuthors", new
            {
                pageNumber = authorsResourceParameters.PageNumber - 1,
                pageSize = authorsResourceParameters.PageSize,
                mainCategory = authorsResourceParameters.MainCategory,
                searchQuery = authorsResourceParameters.SearchQuery
            }),
            ResouceUriType.NextPage => Url.Link("GetAuthors", new
            {
                pageNumber = authorsResourceParameters.PageNumber + 1,
                pageSize = authorsResourceParameters.PageSize,
                mainCategory = authorsResourceParameters.MainCategory,
                searchQuery = authorsResourceParameters.SearchQuery
            }),
            _ => Url.Link("GetAuthors", new
            {
                pageNumber = authorsResourceParameters.PageNumber,
                pageSize = authorsResourceParameters.PageSize,
                mainCategory = authorsResourceParameters.MainCategory,
                searchQuery = authorsResourceParameters.SearchQuery
            }),
        };
    }
}
