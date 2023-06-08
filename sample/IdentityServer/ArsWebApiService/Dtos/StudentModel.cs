using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ArsWebApiService.Dtos
{
    [DataContract()]
    public class StudentModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string No { get; set; }
    }
}
