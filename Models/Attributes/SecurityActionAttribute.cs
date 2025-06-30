using System;

[AttributeUsage(AttributeTargets.Method)]
public class SecurityActionAttribute : Attribute
{
    public int ControllerId { get; }
    public long ActionId { get; }

    public SecurityActionAttribute(int controllerId, long actionId)
    {
        ControllerId = controllerId;
        ActionId = actionId;
    }
}