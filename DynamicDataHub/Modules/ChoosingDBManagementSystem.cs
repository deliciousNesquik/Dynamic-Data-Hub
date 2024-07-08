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
        public List<string> ManagementSystems = new List<string> { "SQL Server Management Studio", "PostgreSQL 16", "DB Browser for SQLite" };
        public ChoosingDBManagementSystem()
        {

        }

        public List<string> GetDBManagementSystems(){
            List<string> test = new List<string>();
            string registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            try
            {
                // Открываем подключение к ключу реестра
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (rk != null)
                    {
                        // Получаем список всех подключений к этому ключу
                        foreach (string skName in rk.GetSubKeyNames())
                        {
                            using (RegistryKey sk = rk.OpenSubKey(skName))
                            {
                                if (sk != null)
                                {
                                    string displayName = sk.GetValue("DisplayName") as string;
                                    if (!string.IsNullOrEmpty(displayName))
                                    {
                                        Console.WriteLine($"DisplayName: {displayName}");
                                        if (ManagementSystems.Contains(displayName.Trim()))
                                        {
                                            test.Add(displayName);
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
            test = test.Distinct().ToList();
            return test;
        }
    }
}
