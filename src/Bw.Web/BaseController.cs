using AutoMapper;
using Bw.Core.Cqrs.Commands;
using Bw.Core.Cqrs.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Bw.Web;

public class BaseController : Controller
{
    protected const string BaseApiPath = Constants.BaseApiPath;
    private IMapper? _mapper;

    private IMediator _mediator;
    private ICommandProcessor _commandProcessor;
    private IQueryProcessor _queryProcessor;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

    protected IQueryProcessor? QueryProcessor =>
        _queryProcessor ??= HttpContext.RequestServices.GetService<IQueryProcessor>()!;

    protected ICommandProcessor CommandProcessor =>
        _commandProcessor ??= HttpContext.RequestServices.GetService<ICommandProcessor>()!;

    protected IMapper Mapper => (_mapper ??= HttpContext.RequestServices.GetService<IMapper>())!;
}
