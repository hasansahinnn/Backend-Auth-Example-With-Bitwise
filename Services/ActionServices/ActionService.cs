using System.Collections.Generic;
using Models;
using Entities;

public class ActionService : IActionService
{
    // Mock: RoleId -> ControllerId -> ActionTotal
    private readonly List<RolePermission> _rolePermissions = new List<RolePermission>
    {
        // Admin: Product controller (List + Detail)
        new RolePermission { RoleId = 1, ControllerId = (int)SecurityControllers.Product, ActionTotal = (long)(ProductActions.List | ProductActions.Detail) },
        // User: Product controller (sadece List)
        new RolePermission { RoleId = 2, ControllerId = (int)SecurityControllers.Product, ActionTotal = (long)ProductActions.List }
    };

    private readonly IUserService _userService;

    public ActionService(IUserService userService)
    {
        _userService = userService;
    }

    public Dictionary<int, long> GetUserPermissions(int userId)
    {
        var user = _userService.GetUserById(userId);
        if (user == null) return new Dictionary<int, long>();
        var dict = new Dictionary<int, long>();
        foreach (var perm in _rolePermissions)
        {
            if (perm.RoleId == user.RoleId)
                dict[perm.ControllerId] = perm.ActionTotal;
        }
        return dict;
    }
}