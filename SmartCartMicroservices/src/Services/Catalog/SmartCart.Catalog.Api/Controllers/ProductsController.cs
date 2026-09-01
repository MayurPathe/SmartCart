using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Features.Categories.Commands.CreateProduct;
using SmartCart.Catalog.Application.Features.Categories.Commands.DeleteProduct;
using SmartCart.Catalog.Application.Features.Categories.Commands.UpdateProduct;
using SmartCart.Catalog.Application.Features.Categories.Queries.GetProductById;
using SmartCart.Catalog.Application.Features.Categories.Queries.GetProducts;

namespace SmartCart.Catalog.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request)
    {
        var result =
            await _mediator.Send(
                new CreateProductCommand(request));

        return Created(
            $"/api/products/{result.Id}",
            result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] ProductListRequest request)
    {
        var result =
            await _mediator.Send(
                new GetProductsQuery(request));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result =
            await _mediator.Send(
                new GetProductByIdQuery(id));

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductRequest request)
    {
        var result =
            await _mediator.Send(
                new UpdateProductCommand(
                    id,
                    request));

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(
            new DeleteProductCommand(id));

        return NoContent();
    }
}
