﻿using L4D2ServerManager.Modules.ServerManager.Server.Enums;

namespace L4D2ServerManager.FunctionApp.Requests;

public class RunServerRequest
{
    public Campaign Campaign { get; set; }
}