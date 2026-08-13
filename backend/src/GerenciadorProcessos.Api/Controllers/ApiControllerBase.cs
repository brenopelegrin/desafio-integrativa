using Microsoft.AspNetCore.Mvc;

namespace GerenciadorProcessos.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
}
