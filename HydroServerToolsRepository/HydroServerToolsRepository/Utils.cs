using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsRepository
{
    public class Utils
    {
        public static EntityKey GetEntityKey(EntitySet entitySet, dynamic d)
        {
            //check if entry with this key exists
            var entityKeyValues = new List<KeyValuePair<string, object>>();
            foreach (var member in entitySet.ElementType.KeyMembers)
            {
                var info = d.GetType().GetProperty(member.Name);
                var tempValue = info.GetValue(d, null);
                var pair = new KeyValuePair<string, object>(member.Name, tempValue);
                entityKeyValues.Add(pair);
            }
            var key = new EntityKey(entitySet.EntityContainer.Name + "." + entitySet.Name, entityKeyValues);
            return key;
        }
    }
}
