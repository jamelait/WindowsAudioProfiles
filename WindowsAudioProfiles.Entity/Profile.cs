using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAudioProfiles.Entity
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }


        public override string ToString()
        {
            var left = (int)(Left * 100);
            var right = (int)(Right * 100);
            return String.Concat(Name, ": \t", left, ",", right);
        }
    }
}
