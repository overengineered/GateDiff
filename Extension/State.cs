using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateShell
{
    public sealed class State
    {
        private readonly string m_root;
        private readonly int m_mruLimit;

        public State(int mruLimit)
        {
            m_root = "SOFTWARE\\" + "overengineered" + "\\" + "GateShell";
            m_mruLimit = mruLimit;
        }

        public void InsertMruItem(string listName, string value)
        {
            var lastSaved = RetrieveItemList(listName)
                .Where(item => !String.Equals(item, value, StringComparison.InvariantCultureIgnoreCase))
                .Take(m_mruLimit - 1)
                .ToList();

            SaveItemList(listName, Enumerable.Concat(Enumerable.Repeat(value, 1), lastSaved));
        }

        public IEnumerable<string> GetMruItems(string listName)
        {
            return RetrieveItemList(listName).Take(m_mruLimit);
        }

        public IEnumerable<string> RetrieveItemList(String name)
        {
            using (RegistryKey mainKey = Registry.CurrentUser.OpenSubKey(m_root, false))
            {
                if (mainKey == null)
                    yield break;

                using (RegistryKey listKey = mainKey.OpenSubKey(name))
                {
                    if (listKey == null)
                        yield break;

                    int index = 0;
                    while (true)
                    {
                        var key = index.ToString();
                        var value = (string) listKey.GetValue(key);
                        if (String.IsNullOrEmpty(value))
                            yield break;

                        yield return value;
                        index++;
                    }
                }
            }
        }

        public void SaveItemList(String name, IEnumerable<string> values)
        {
            using (RegistryKey mainKey = Registry.CurrentUser.CreateSubKey(m_root))
            {
                if (mainKey == null)
                    return;

                using (RegistryKey listKey = mainKey.CreateSubKey(name))
                {
                    if (listKey == null)
                        return;

                    int index = 0;
                    try
                    {
                        foreach (var value in values)
                        {
                            var key = index.ToString();
                            listKey.SetValue(key, value);
                            index++;
                        }
                    }
                    finally
                    {
                        bool throwOnMissingValue = false;
                        listKey.DeleteValue(index.ToString(), throwOnMissingValue);
                    }
                }
            }
        }
    }
}
