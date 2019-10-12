using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace BlazingPermissions
{
    public class RoleClaimsPermissionCheck : ICheckPermissions
    {
        public RoleClaimsPermissionCheck(ClaimsPrincipal user, IEnumerable<string> viewRoles, IEnumerable<string> writeRoles)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            ViewRoles = viewRoles ?? throw new ArgumentNullException(nameof(viewRoles));
            WriteRoles = writeRoles ?? throw new ArgumentNullException(nameof(writeRoles));
        }

        public ClaimsPrincipal User { get; }
        public IEnumerable<string> ViewRoles { get; }
        public IEnumerable<string> WriteRoles { get; }

        private bool CheckForRoles(IEnumerable<string> roles)
        {
            if (User.Claims.Any())
            {
                foreach (var role in ViewRoles)
                {
                    var hasRole = User.IsInRole(role);
                    if (hasRole)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanView() => CheckForRoles(ViewRoles);

        public bool CanWrite() => CheckForRoles(WriteRoles);

        public bool HasPermission(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return false;
            }
            return CheckForRoles(new [] { id });
        }

        public bool IfCanViewRun(Action action) => HasPermissionRunCore(CanView, action);

        public Result<TResult> IfCanViewRun<TResult>(Func<TResult> function) 
            => HasPermissionRunCore(CanView, function);

        public Task<Result<TResult>> IfCanViewRun<TResult>(Func<Task<TResult>> function) 
            => HasPermissionRunCore(CanView, function);

        public Task<Result> IfCanViewRun(Func<Task> function) => HasPermissionRunCore(CanView, function);

        public bool IfCanWriteRun(Action action) => HasPermissionRunCore(CanWrite, action);

        public Result<TResult> IfCanWriteRun<TResult>(Func<TResult> function) 
            => HasPermissionRunCore(CanWrite, function);

        public Task<Result<TResult>> IfCanWriteRun<TResult>(Func<Task<TResult>> function)
            => HasPermissionRunCore(CanWrite, function);

        public Task<Result> IfCanWriteRun(Func<Task> function) 
            => HasPermissionRunCore(CanWrite, function);


        public bool IfHasPermissionRun(string id, Action action) 
            => HasPermissionRunCore(() => HasPermission(id), action);

        public Result<TResult> IfHasPermissionRun<TResult>(string id, Func<TResult> function) 
            => HasPermissionRunCore(() => HasPermission(id), function);

        public Task<Result<TResult>> IfHasPermissionRun<TResult>(string id, Func<Task<TResult>> function)
            => HasPermissionRunCore(() => HasPermission(id), function);

        public Task<Result> IfHasPermissionRun(string id, Func<Task> function) 
            => HasPermissionRunCore(() => HasPermission(id), function);

        private bool HasPermissionRunCore(Func<bool> testFunction, Action action)
        {
            if(testFunction())
            {
                action();
                return true;
            }
            else
            {
                return false;
            }
        }

        private Result<TResult> HasPermissionRunCore<TResult>(Func<bool> testFunction, Func<TResult> function)
        {
            if(testFunction())
            {
                var result = function();
                return Result.Ok(result);
            }
            else
            {
                return Result.Failure<TResult>("Permission denied.");
            }
        }

        private async Task<Result<TResult>> HasPermissionRunCore<TResult>(Func<bool> testFunction, Func<Task<TResult>> function)
        {
            if(testFunction())
            {
                var result = await function().ConfigureAwait(false);
                return Result.Ok<TResult>(result);
            }
            else
            {
                return Result.Failure<TResult>("Permission denied.");
            }
        }

        private async Task<Result> HasPermissionRunCore(Func<bool> testFunction, Func<Task> function)
        {
            if(testFunction())
            {
                await function().ConfigureAwait(false);
                return Result.Ok();
            }
            else
            {
                return Result.Failure("Permission denied.");
            }
        }
    }
}