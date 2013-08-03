using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class PhoneInfo
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public override bool Equals(object obj)
        {
            var info = obj as PhoneInfo;
            if (info != null)
            {
                return Name == info.Name &&
                    Phone == info.Phone;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Name)
                    .CombineHashCodeOrDefault(Phone);
        }
    }
}

