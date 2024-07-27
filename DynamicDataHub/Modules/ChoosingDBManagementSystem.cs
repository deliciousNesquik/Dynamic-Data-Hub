using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataHub.Modules
{

    internal class ChoosingDBManagementSystem
    {
        public Dictionary<string, string> ManagementSystems = new Dictionary<string, string>
        {
            {"SQL Server", "SQL Server Management Studio"},
            {"PostgreSQL 16", "PostgreSQL 16"},
            {"DB Browser for SQLite", "SQLite"}
        };

        public ChoosingDBManagementSystem()
        {

        }

        public List<string> GetDBManagementSystems()
        {
            List<string> foundSystems = new List<string>();
            string registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (rk != null)
                    {
                        foreach (string skName in rk.GetSubKeyNames())
                        {
                            using (RegistryKey sk = rk.OpenSubKey(skName))
                            {
                                if (sk != null)
                                {
                                    string displayName = sk.GetValue("DisplayName") as string;
                                    if (!string.IsNullOrEmpty(displayName))
                                    {
                                        foreach (var ms in ManagementSystems)
                                        {
                                            if (displayName.Contains(ms.Key) && !foundSystems.Contains(ms.Value))
                                            {
                                                Console.WriteLine($"Found: {ms.Value} ({displayName})");
                                                foundSystems.Add(ms.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при доступе к реестру: {ex.Message}");
            }

            return foundSystems;
        }
    }
}