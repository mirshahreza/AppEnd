using System.Collections.Generic;
using System.Linq;
using AppEndCommon;

namespace AppEndDynaCode
{
    internal static class DynaCodeHelpers
    {
        #region DTO & Helper Methods
        public static List<DynaClass> GetDynaClasses()
        {
            List<DynaClass> dynaClasses = [];
            foreach (var i in DynaCodeCore.DynaAsm.GetTypes())
            {
                if (Utils.IsRealType(i.Name))
                {
                    List<DynaMethod> dynaMethods = [];
                    foreach (var method in i.GetMethods())
                        if (Utils.IsRealMethod(method.Name))
                            dynaMethods.Add(new(method.Name, DynaCodeSettings.ReadMethodSettings($"{i.Namespace}.{i.Name}.{method.Name}")));

                    DynaClass dynamicController = new(i.Name, dynaMethods) { Namespace = i.Namespace };
                    dynaClasses.Add(dynamicController);
                }
            }
            return [.. dynaClasses.OrderBy(i => i.Namespace + i.Name)];
        }
        #endregion
    }
}
