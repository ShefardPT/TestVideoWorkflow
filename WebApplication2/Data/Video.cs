using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Data
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime DateUploaded { get; set; }
        public string JobId { get; set; }
        public string Mp4 { get; set; }
        public string Webm { get; set; }
        public string Ogg { get; set; }
        public bool IsDone { get; set; }
    }
}
