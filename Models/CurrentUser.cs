using System.Collections.Generic;

namespace BreweryApp.Models
{
    public static class CurrentUser
    {
        public static int UserId { get; set; }
        public static int EmployeeId { get; set; }
        public static int RoleId { get; set; }

        public static string FIO { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static string Department { get; set; } = string.Empty;

        public static List<string> Permissions { get; set; } = new List<string>();

        public static bool HasPermission(string permissionName)
        {
            return Permissions.Contains(permissionName);
        }

        public static void Clear()
        {
            UserId = 0;
            EmployeeId = 0;
            RoleId = 0;
            FIO = string.Empty;
            Role = string.Empty;
            Department = string.Empty;
            Permissions.Clear();
        }
    }
}