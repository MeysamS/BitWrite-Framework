﻿namespace Bw.Core.BaseModel;

public interface IResult
{
    bool Success { get; }
    string? ErrorMessage { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}