﻿namespace SanatoriumCore.Infrastructure
{
    public interface ITransactionResult<T>
    {
        int Id { get; set; }
        T Data { get; set; }
    }

    public interface ITransactionResult
    {
        int Id { get; set; }
    }
}
