using System.Collections.Generic;

public interface IActionService
{
    Dictionary<int, long> GetUserPermissions(int userId);
}