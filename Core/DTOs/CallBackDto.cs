using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.DTOs
{
    public class CallBackDto
    {
        public string Object { get; set; }
        public string Action { get; set; }
        public Guid? Id { get; set; }

        public CallBackDto(string _object, string _action = "", Guid? _id = null)
        {
            Object = _object;
            Action = _action;
            Id = _id;
        }

        public static CallBackDto FromString(string query)
        {
            string[] strings = query.Split('_');
            switch (strings.Length)
            {
                case 1: return new CallBackDto(strings[0]);
                case 2: return new CallBackDto(strings[0], strings[1]);
                default:
                    if (Guid.TryParse(strings[2], out Guid id))
                    {
                        return new CallBackDto(strings[0], strings[1], id);
                    }
                    return new CallBackDto(strings[0], strings[1]);
            }
        }
        public override string ToString()
        {
            if(Id == null)
            {
                return $"{Object}_{Action}";
            }
            return $"{Object}_{Action}_{Id}";
        }
    }
}
