using Destructurama.Attributed;
using System;

namespace Company.Common.Data
{
    [Serializable]
    public class RegisterRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [NotLogged]
        public string Password { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
