using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class WebSiteInfo
    {
        public string Label { get; set; }

        public string URL { get; set; }

        public override bool Equals(object obj)
        {
            var info = obj as WebSiteInfo;
            if (info != null)
            {
                return Label == info.Label &&
                    URL == info.URL;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Label)
                    .CombineHashCodeOrDefault(URL);
        }
    }
}