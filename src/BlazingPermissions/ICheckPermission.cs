using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace BlazingPermissions
{
    public interface ICheckPermissions
    {
        bool HasPermission(string id);
        bool CanWrite();
        bool CanView();

        bool IfHasPermissionRun(string permission, Action action);
        Result<TResult> IfHasPermissionRun<TResult>(string permission, Func<TResult> func);
        Task<Result<TResult>> IfHasPermissionRun<TResult>(string permission, Func<Task<TResult>> func);
        Task<Result> IfHasPermissionRun(string permission, Func<Task> func);

        bool IfCanViewRun(Action action);
        Result<TResult> IfCanViewRun<TResult>(Func<TResult> func);
        Task<Result<TResult>> IfCanViewRun<TResult>(Func<Task<TResult>> func);
        Task<Result> IfCanViewRun(Func<Task> func);

        bool IfCanWriteRun(Action action);
        Result<TResult> IfCanWriteRun<TResult>(Func<TResult> func);
        Task<Result<TResult>> IfCanWriteRun<TResult>(Func<Task<TResult>> func);
        Task<Result> IfCanWriteRun(Func<Task> func);
    }
}