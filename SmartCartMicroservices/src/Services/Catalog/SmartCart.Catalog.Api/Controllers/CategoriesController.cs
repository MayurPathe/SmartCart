using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Features.Categories.Commands.CreateCategory;
using SmartCart.Catalog.Application.Features.Categories.Queries.GetCategories;

namespace SmartCart.Catalog.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryRequest request)
    {
        var result =
            await _mediator.Send(
                new CreateCategoryCommand(request));

        return Created(
            $"/api/categories/{result.Id}",
            result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result =
            await _mediator.Send(
                new GetCategoriesQuery());

        return Ok(result);
    }
}
