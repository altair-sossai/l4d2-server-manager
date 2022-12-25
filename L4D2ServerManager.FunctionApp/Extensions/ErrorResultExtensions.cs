﻿using System.Net;
using System.Net.Http;
using System.Web.Http;
using L4D2ServerManager.FunctionApp.Errors;
using Microsoft.AspNetCore.Mvc;

namespace L4D2ServerManager.FunctionApp.Extensions;

public static class ErrorResultExtensions
{
	public static IActionResult ResponseMessageResult(this ErrorResult errorResult)
	{
		return errorResult.StatusCode switch
		{
			HttpStatusCode.BadRequest => new BadRequestObjectResult(errorResult),
			HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(errorResult),
			_ => new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.InternalServerError))
			{
				Response = { Content = new StringContent(errorResult.Message) }
			}
		};
	}
}