using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Model
{
    public static class FileExistsResponse
    {
        static Dictionary<FILE_EXISTS_RESPONSE, string> dictionary;
        static FileExistsResponse()
        {
            dictionary = Enum.GetValues(typeof(FILE_EXISTS_RESPONSE))
                    .Cast<FILE_EXISTS_RESPONSE>()
                    .Select(value => new
                    {
                        value,
                        (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description
                    }).ToDictionary(v => v.value, v => v.Description);
        }

        public static Dictionary<FILE_EXISTS_RESPONSE, string> GetDictionary => dictionary;
    }
}
