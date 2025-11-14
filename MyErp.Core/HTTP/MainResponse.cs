using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyErp.Core.HTTP
{
    public class MainResponse<T>
    {

        public List<string>? errors { get; set; } = new List<string>();
        public List<T>? acceptedObjects { get; set; } = new List<T>();
        public List<T>? rejectedObjects { get; set; } = new List<T>();


        public string Serialize()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles  // Use IgnoreCycles reference handling
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}
