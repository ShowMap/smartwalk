using System.Collections.Generic;

namespace SmartWalk.iOS.Views.Common
{
    public class GroupContainer : List<object>
    {
        public GroupContainer(object[] items)
        {
            this.AddRange(items);
        }

        public string Key { get; set; }
    }
}