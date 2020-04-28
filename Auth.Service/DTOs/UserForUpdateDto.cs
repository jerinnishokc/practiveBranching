using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.DTOs
{
    public class UserForUpdateDto
    {
        public string id { get; set; }
        public string username { get; set; }

        public string role { get; set; }
        public string[] Events { get; set; }

        public string FeedbackStatus { get; set; }
    }
}
