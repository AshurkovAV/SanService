using System;
using System.Collections.Generic;
using System.Text;

namespace SanatoriumCore.Infrastructure
{
    public interface IError
    {
        bool HasError { get; }
        bool Success { get; }
        IList<object> Errors { get; }
        Exception LastError { get; }
        void AddError(object error);
    }
}
