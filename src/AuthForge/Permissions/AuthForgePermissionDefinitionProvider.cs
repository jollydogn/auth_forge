namespace AuthForge.Permissions;

/// <summary>
/// Base class that consumers inherit from to declare their permissions.
/// 
/// Usage:
/// <code>
/// public class OrderPermissionDefinitionProvider : AuthForgePermissionDefinitionProvider
/// {
///     public override void Define(AuthForgePermissionDefinitionContext context)
///     {
///         var group = context.AddGroup("OrderManagement", "Order Management");
///         
///         var orders = group.AddPermission("Orders", "Orders");
///         orders.AddChild("Orders.Create", "Create Order");
///         orders.AddChild("Orders.Edit",   "Edit Order");
///         orders.AddChild("Orders.Delete", "Delete Order");
///         orders.AddChild("Orders.List",   "List Orders");
///     }
/// }
/// </code>
/// </summary>
public abstract class AuthForgePermissionDefinitionProvider
{
    /// <summary>
    /// Override this method to define your module's permission groups and permissions.
    /// Called once during application startup.
    /// </summary>
    public abstract void Define(AuthForgePermissionDefinitionContext context);
}
